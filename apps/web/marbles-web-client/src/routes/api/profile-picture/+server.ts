// apps/web/marbles-web-client/src/routes/api/profile-picture/+server.ts
// Profile picture upload endpoint using SpacetimeDB HTTP API

import { AwsClient } from "aws4fetch";
import { json, error } from "@sveltejs/kit";
import type { RequestHandler } from "./$types";
import {
    Account,
    AccountCustomization,
    DbConnection,
    type ErrorContext,
} from "../../../module_bindings";
import type { Identity } from "spacetimedb";

// Configuration constants
const PROFILE_STORAGE_PREFIX = "pfp/";
const MAX_PROFILE_VERSION = 255;
const PFP_MIME = "image/webp";
const PFP_MAX_BYTES = 512 * 1024; // 512KB
const PFP_IMAGE_SIZE = 256; // 256x256 pixels

// WebP parsing types and utilities
type WebpDimensions = { width: number; height: number };

function readUint32LE(data: Uint8Array, offset: number): number {
    return (
        (data[offset] |
            (data[offset + 1] << 8) |
            (data[offset + 2] << 16) |
            (data[offset + 3] << 24)) >>>
        0
    );
}

function getFourCC(data: Uint8Array, offset: number): string {
    return String.fromCharCode(data[offset], data[offset + 1], data[offset + 2], data[offset + 3]);
}

function parseLossyDimensions(
    data: Uint8Array,
    offset: number,
    size: number
): WebpDimensions | null {
    if (size < 10) return null;
    const frameStart = offset;
    // Check for start code 0x9d012a
    if (
        data[frameStart + 3] !== 0x9d ||
        data[frameStart + 4] !== 0x01 ||
        data[frameStart + 5] !== 0x2a
    ) {
        return null;
    }
    const rawWidth = data[frameStart + 6] | ((data[frameStart + 7] & 0x3f) << 8);
    const rawHeight = data[frameStart + 8] | ((data[frameStart + 9] & 0x3f) << 8);
    return { width: rawWidth + 1, height: rawHeight + 1 };
}

function parseLosslessDimensions(
    data: Uint8Array,
    offset: number,
    size: number
): WebpDimensions | null {
    if (size < 5) return null;
    if (data[offset] !== 0x2f) {
        return null;
    }
    const widthMinusOne = ((data[offset + 2] & 0x3f) << 8) | data[offset + 1];
    const heightMinusOne =
        ((data[offset + 4] & 0x0f) << 10) |
        (data[offset + 3] << 2) |
        ((data[offset + 2] & 0xc0) >> 6);
    return { width: widthMinusOne + 1, height: heightMinusOne + 1 };
}

function parseExtendedDimensions(
    data: Uint8Array,
    offset: number,
    size: number
): WebpDimensions | null {
    if (size < 10) return null;
    const widthMinusOne = data[offset + 4] | (data[offset + 5] << 8) | (data[offset + 6] << 16);
    const heightMinusOne = data[offset + 7] | (data[offset + 8] << 8) | (data[offset + 9] << 16);
    return { width: widthMinusOne + 1, height: heightMinusOne + 1 };
}

function parseWebpDimensions(data: Uint8Array): WebpDimensions | null {
    if (data.length < 30) {
        return null;
    }
    if (getFourCC(data, 0) !== "RIFF" || getFourCC(data, 8) !== "WEBP") {
        return null;
    }
    let offset = 12;
    while (offset + 8 <= data.length) {
        const chunkId = getFourCC(data, offset);
        const chunkSize = readUint32LE(data, offset + 4);
        const chunkDataOffset = offset + 8;
        if (chunkDataOffset + chunkSize > data.length) {
            return null;
        }
        if (chunkId === "VP8 ") {
            return parseLossyDimensions(data, chunkDataOffset, chunkSize);
        }
        if (chunkId === "VP8L") {
            return parseLosslessDimensions(data, chunkDataOffset, chunkSize);
        }
        if (chunkId === "VP8X") {
            return parseExtendedDimensions(data, chunkDataOffset, chunkSize);
        }
        offset = chunkDataOffset + chunkSize + (chunkSize & 1);
    }
    return null;
}

async function subscribeAndWait(connection: DbConnection, queries: string[]): Promise<void> {
    return new Promise((resolve, reject) => {
        let settled = false;
        connection
            .subscriptionBuilder()
            .onApplied(() => {
                if (settled) return;
                settled = true;
                resolve();
            })
            .onError((errorContext: ErrorContext) => {
                if (settled) return;
                settled = true;
                const errorMessage =
                    errorContext instanceof Error
                        ? errorContext.message
                        : JSON.stringify(errorContext);
                console.error("[Profile] Subscription error:", errorMessage);
                reject(new Error(`Failed to subscribe: ${errorMessage}`));
            })
            .subscribe(queries);
    });
}

async function connectToSpacetimeDb(
    token: string
): Promise<{ connection: DbConnection; identity: Identity }> {
    const host = import.meta.env.VITE_SPACETIMEDB_HOST;
    const moduleName = import.meta.env.VITE_SPACETIMEDB_DB_NAME;

    if (!host || !moduleName) {
        throw error(500, "SpacetimeDB connection is not configured");
    }

    return new Promise((resolve, reject) => {
        let settled = false;
        const builder = DbConnection.builder()
            .withUri(host)
            .withModuleName(moduleName)
            .withToken(token)
            .onConnect((connection, identity) => {
                //const identityU256 = identityToU256(identity);
                if (identity === undefined) {
                    if (!settled) {
                        settled = true;
                        reject(new Error("Unable to resolve account identity"));
                    }
                    return;
                }

                if (!settled) {
                    settled = true;
                    resolve({ connection, identity });
                }
            })
            .onConnectError((_ctx, err) => {
                if (!settled) {
                    settled = true;
                    reject(err ?? new Error("Failed to connect to SpacetimeDB"));
                }
            })
            .onDisconnect((_ctx, err) => {
                if (!settled) {
                    settled = true;
                    reject(err ?? new Error("Disconnected before connecting to SpacetimeDB"));
                }
            });

        builder.build();
    });
}

async function resolveAccountData(
    connection: DbConnection,
    identity: Identity //identityU256
): Promise<{ accountId: bigint; currentVersion: number }> {
    console.log(`[Profile] Starting resolve account data for identity: ${identity.toHexString()}`);
    await subscribeAndWait(connection, [
        `SELECT * FROM Account`, // WHERE identity = 0x${identity.toHexString()}`
    ]);
    console.log("[Profile] After subscribe and wait");

    // Debug: Log all accounts in the cache
    const allAccounts = Array.from(connection.db.account.iter());
    console.log(`[Profile] Accounts in cache: ${allAccounts.length}`);
    for (const acc of allAccounts) {
        console.log(`[Profile]   Account ${acc.id}: identity=${acc.identity.toHexString()}`);
    }

    const accountRow = connection.db.account.identity.find(identity);
    if (!accountRow) {
        console.error(
            `[Profile] No account is associated with this identity: ${identity.toHexString()}`
        );
        throw error(469, "No account is associated with this identity");
    }

    const accountId = accountRow.id;
    const query = `SELECT * FROM AccountCustomization`; //Don't filter by WHERE, RLS takes care of it
    console.log("Starting subscribe for account_customization with Query: ", query);
    await subscribeAndWait(connection, [query]);
    console.log("After subscribe and wait for account_customization");
    const customization = connection.db.accountCustomization.accountId.find(accountId) as
        | AccountCustomization
        | undefined;

    const currentVersion = customization ? Number(customization.pfpVersion) : 0;
    return { accountId, currentVersion };
}

// R2/S3 upload functions
function buildR2TargetUrl(endpoint: string, bucket: string, objectKey: string): string {
    const u = new URL(endpoint);
    const key = objectKey.replace(/^\/+/, "");

    // Does endpoint already include the bucket in the path?
    const pathSegs = u.pathname.split("/").filter(Boolean);
    const hasBucketInPath = pathSegs.length > 0 && pathSegs[0] === bucket;

    // Virtual host style? (bucket.<account>.r2.cloudflarestorage.com)
    const hasBucketInHost = u.hostname.startsWith(`${bucket}.`);

    if (hasBucketInPath) {
        u.pathname = `/${pathSegs.join("/")}/${key}`.replace(/\/{2,}/g, "/");
    } else if (hasBucketInHost) {
        u.pathname = `/${key}`;
    } else {
        u.pathname = `/${bucket}/${key}`;
    }

    return u.toString();
}

async function uploadProfilePictureToR2(
    body: ArrayBuffer,
    objectKey: string,
    contentType: string,
    env: App.Platform["env"]
): Promise<void> {
    const endpoint = env.PROFILE_PICTURE_S3_ENDPOINT;
    const bucket = env.PROFILE_PICTURE_S3_BUCKET;
    const accessKeyId = env.PROFILE_PICTURE_S3_ACCESS_KEY_ID;
    const secretAccessKey = env.PROFILE_PICTURE_S3_SECRET_ACCESS_KEY;

    if (!endpoint || !bucket || !accessKeyId || !secretAccessKey) {
        throw error(500, "Profile picture storage is not fully configured");
    }

    const awsClient = new AwsClient({
        accessKeyId,
        secretAccessKey,
        service: "s3",
        region: "auto",
    });

    const targetUrl = buildR2TargetUrl(endpoint, bucket, objectKey);
    console.log("[Profile] Uploading to R2:", targetUrl);

    const res = await awsClient.fetch(targetUrl, {
        method: "PUT",
        body,
        headers: {
            "Content-Type": contentType,
            "Cache-Control": "public, max-age=31536000, immutable",
        },
    });

    if (!res.ok) {
        const text = await res.text().catch(() => "");
        throw new Error(
            `Failed to upload profile picture to R2 (${res.status}): ${text || "No response body"}`
        );
    }
}

function buildProfilePictureUrl(
    accountId: bigint,
    version: number,
    env: App.Platform["env"]
): string | null {
    const baseUrl = env.PROFILE_PICTURE_BASE_URL?.trim();
    if (!baseUrl || version <= 0) {
        return null;
    }
    const normalizedBase = baseUrl.replace(/\/$/, "");
    return `${normalizedBase}/pfp/${accountId.toString()}.webp?v=${version}`;
}

// Download image from URL and return as bytes
async function downloadImageFromUrl(imageUrl: string): Promise<Uint8Array> {
    const response = await fetch(imageUrl);
    if (!response.ok) {
        throw new Error(`Failed to download image from URL: ${response.status}`);
    }
    const buffer = await response.arrayBuffer();
    return new Uint8Array(buffer);
}

// Main request handler
export const POST: RequestHandler = async ({ request, platform }) => {
    console.log("[Profile] Starting POST request");

    const env = platform?.env;
    if (!env) {
        throw error(500, "Platform environment is not available");
    }

    // Extract and validate authorization token
    const authHeader = request.headers.get("authorization") ?? request.headers.get("Authorization");
    if (!authHeader?.startsWith("Bearer ")) {
        throw error(401, "An ID token is required to upload profile pictures");
    }

    const token = authHeader.slice(7).trim();
    if (!token) {
        throw error(401, "An ID token is required to upload profile pictures");
    }
    console.log(`[Profile] Received token (first 20 chars): ${token.substring(0, 20)}...`);

    // Parse the request - supports both form-data with image OR JSON with imageUrl
    const contentType = request.headers.get("content-type") ?? "";
    let bytes: Uint8Array;

    if (contentType.includes("multipart/form-data")) {
        // Traditional form upload with WebP image
        const formData = await request.formData();
        const image = formData.get("image");

        if (!(image instanceof File)) {
            throw error(400, "The request must include an image file");
        }

        if (image.type !== PFP_MIME) {
            throw error(400, `Profile pictures must be uploaded as ${PFP_MIME}`);
        }

        if (image.size > PFP_MAX_BYTES) {
            throw error(400, `Profile pictures must be smaller than ${PFP_MAX_BYTES} bytes`);
        }

        bytes = new Uint8Array(await image.arrayBuffer());

        const dimensions = parseWebpDimensions(bytes);

        if (!dimensions) {
            throw error(400, "The uploaded image is not a valid WebP file");
        }

        if (dimensions.width !== PFP_IMAGE_SIZE || dimensions.height !== PFP_IMAGE_SIZE) {
            throw error(
                400,
                `Profile pictures must be exactly ${PFP_IMAGE_SIZE}×${PFP_IMAGE_SIZE} pixels`
            );
        }
    } else if (contentType.includes("application/json")) {
        // JSON body with imageUrl - download from OAuth provider
        const body = (await request.json()) as { imageUrl?: string };

        if (!body.imageUrl || typeof body.imageUrl !== "string") {
            throw error(400, "Request must include an imageUrl");
        }

        // Validate URL
        try {
            new URL(body.imageUrl);
        } catch {
            throw error(400, "Invalid imageUrl provided");
        }

        console.log("[Profile] Downloading image from URL:", body.imageUrl);

        try {
            bytes = await downloadImageFromUrl(body.imageUrl);
        } catch (err) {
            console.error("[Profile] Failed to download image:", err);
            throw error(400, "Failed to download image from URL");
        }

        // Note: Images from OAuth providers are typically JPEG/PNG, not WebP
        // We store them as-is since most browsers/clients can handle these formats
        // If WebP conversion is needed, it would require additional image processing
        console.log(`[Profile] Downloaded ${bytes.length} bytes from URL`);
    } else {
        throw error(400, "Request must be multipart/form-data or application/json");
    }

    // Resolve account data via SpacetimeDB connection
    let connection: DbConnection | null = null;
    let accountId = 0n;
    let currentVersion = 0;

    try {
        const connectionResult = await connectToSpacetimeDb(token);
        connection = connectionResult.connection;
        const accountData = await resolveAccountData(connection, connectionResult.identity);
        accountId = accountData.accountId;
        currentVersion = accountData.currentVersion;
        console.log(`[Profile] Resolved account ${accountId} with pfp version ${currentVersion}`);
    } catch (err) {
        console.error("[Profile] Failed to resolve account data:", err);
        connection?.disconnect();
        if (err && typeof err === "object" && "status" in err) {
            throw err; // Re-throw SvelteKit errors
        }
        throw error(401, "Unable to verify your SpacetimeDB account");
    }

    if (!connection) {
        throw error(500, "SpacetimeDB connection is not available");
    }
    const activeConnection = connection;

    // Upload the image to R2
    // Use appropriate extension based on content type
    const isWebp = bytes.length >= 12 && getFourCC(bytes, 8) === "WEBP";
    const extension = isWebp ? "webp" : "jpg"; // Default to jpg for OAuth images
    const mimeType = isWebp ? "image/webp" : "image/jpeg";

    const objectKey = `${PROFILE_STORAGE_PREFIX}${accountId.toString()}.${extension}`;
    const nextVersion = Math.min(currentVersion + 1, MAX_PROFILE_VERSION);

    try {
        console.log(`[Profile] Uploading ${bytes.length} bytes to R2 as ${objectKey}`);
        await uploadProfilePictureToR2(bytes.buffer as ArrayBuffer, objectKey, mimeType, env);
        console.log("[Profile] Upload successful");

        console.log("[Profile] Calling IncrementPfpVersion reducer");
        await activeConnection.reducers.incrementPfpVersion();
        console.log("[Profile] Reducer called successfully");
    } catch (err) {
        console.error("[Profile] Failed to upload profile picture:", err);
        throw error(500, "Failed to upload profile picture");
    } finally {
        activeConnection.disconnect();
    }

    // Build the profile picture URL
    const url = buildProfilePictureUrl(accountId, nextVersion, env);

    console.log("[Profile] Returning profile picture URL:", url);

    return json({
        accountId: accountId.toString(),
        version: nextVersion,
        url,
    });
};
