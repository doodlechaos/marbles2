// apps/web/marbles-web-client/src/routes/api/profile-picture/+server.ts
// Profile picture upload endpoint using SpacetimeDB HTTP API and R2 bucket binding

import { json, error } from "@sveltejs/kit";
import type { RequestHandler } from "./$types";
import { AccountCustomization, DbConnection, type ErrorContext } from "../../../module_bindings";
import { Identity } from "spacetimedb";

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
    env: App.Platform["env"]
): Promise<{ connection: DbConnection; identity: Identity }> {
    // Use platform.env from wrangler (works with wrangler dev and production)
    const host = env.SPACETIMEDB_HOST;
    const moduleName = env.SPACETIMEDB_DB_NAME;
    const token = env.SPACETIMEDB_ADMIN_TOKEN;

    if (!host || !moduleName) {
        console.error("[Profile] SpacetimeDB connection is not configured:", host, moduleName);
        throw error(500, "SpacetimeDB connection is not configured");
    }

    if (!token) {
        console.error("[Profile] No admin token available for server-side SpacetimeDB connection");
        throw error(500, "SpacetimeDB admin token is not configured");
    }

    console.log("[Profile] Connecting to SpacetimeDB:", host, moduleName, "(using admin token)");

    return new Promise((resolve, reject) => {
        let settled = false;
        const builder = DbConnection.builder()
            .withUri(host)
            .withModuleName(moduleName)
            .withToken(token)
            .onConnect((connection, identity) => {
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

// R2 upload using bucket binding (no S3 credentials needed!)
async function uploadProfilePictureToR2(
    bucket: R2Bucket,
    objectKey: string,
    body: ArrayBuffer | Uint8Array,
    contentType: string
): Promise<void> {
    console.log(
        `[Profile] Uploading to R2 bucket, key: ${objectKey}, size: ${body.byteLength} bytes`
    );

    const result = await bucket.put(objectKey, body, {
        httpMetadata: {
            contentType,
            cacheControl: "public, max-age=31536000, immutable",
        },
    });

    if (!result) {
        throw new Error("R2 put returned null - upload may have failed");
    }

    console.log(`[Profile] R2 upload successful, etag: ${result.etag}`);
}

function buildProfilePictureUrl(
    baseUrl: string | undefined,
    accountId: bigint,
    version: number,
    extension: string
): string | null {
    console.log("[PFP] Base URL:", baseUrl);
    if (!baseUrl?.trim() || version <= 0) {
        return null;
    }
    const normalizedBase = baseUrl.trim().replace(/\/$/, "");
    return `${normalizedBase}/pfp/${accountId.toString()}.${extension}?v=${version}`;
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
        console.error("[Profile] Platform environment is not available");
        throw error(500, "Platform environment is not available");
    }

    // Extract and validate authorization token
    const authHeader = request.headers.get("authorization") ?? request.headers.get("Authorization");
    console.log(
        `[Profile] Auth header present: ${!!authHeader}, starts with Bearer: ${authHeader?.startsWith("Bearer ")}`
    );

    if (!authHeader?.startsWith("Bearer ")) {
        console.error("[Profile] Missing or invalid auth header");
        throw error(401, "An ID token is required to upload profile pictures");
    }

    const token = authHeader.slice(7).trim();
    if (!token) {
        console.error("[Profile] Token is empty after extracting from header");
        throw error(401, "An ID token is required to upload profile pictures");
    }
    console.log(`[Profile] Received token (first 20 chars): ${token.substring(0, 20)}...`);

    // Parse the request - supports both form-data with image OR JSON with imageUrl
    const contentType = request.headers.get("content-type") ?? "";
    let bytes: Uint8Array;
    let userIdentity: Identity | null = null;

    if (contentType.includes("multipart/form-data")) {
        // Traditional form upload with WebP image
        const formData = await request.formData();
        const image = formData.get("image");
        const identityField = formData.get("identity");

        if (!(image instanceof File)) {
            throw error(400, "The request must include an image file");
        }

        if (!identityField || typeof identityField !== "string") {
            throw error(400, "The request must include an identity");
        }

        try {
            userIdentity = Identity.fromString(identityField);
        } catch {
            throw error(400, "Invalid identity format");
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
        // JSON body with imageUrl and identity - download from OAuth provider
        const body = (await request.json()) as { imageUrl?: string; identity?: string };

        if (!body.imageUrl || typeof body.imageUrl !== "string") {
            throw error(400, "Request must include an imageUrl");
        }

        if (!body.identity || typeof body.identity !== "string") {
            throw error(400, "Request must include an identity");
        }

        // Validate URL
        try {
            new URL(body.imageUrl);
        } catch {
            throw error(400, "Invalid imageUrl provided");
        }

        // Parse identity from request
        try {
            userIdentity = Identity.fromString(body.identity);
        } catch {
            throw error(400, "Invalid identity format");
        }

        console.log("[Profile] Downloading image from URL:", body.imageUrl);

        try {
            bytes = await downloadImageFromUrl(body.imageUrl);
        } catch (err) {
            console.error("[Profile] Failed to download image:", err);
            throw error(400, "Failed to download image from URL");
        }

        console.log(`[Profile] Downloaded ${bytes.length} bytes from URL`);
    } else {
        throw error(400, "Request must be multipart/form-data or application/json");
    }

    if (!userIdentity) {
        throw error(400, "Identity is required");
    }
    console.log(`[Profile] User identity: ${userIdentity.toHexString()}`);

    // Resolve account data via SpacetimeDB connection using admin token
    let connection: DbConnection | null = null;
    let accountId = 0n;
    let currentVersion = 0;

    try {
        console.log("[Profile] Attempting to connect to SpacetimeDB with admin token...");
        const connectionResult = await connectToSpacetimeDb(env);
        console.log(
            "[Profile] Connected to SpacetimeDB as admin, identity:",
            connectionResult.identity.toHexString()
        );
        connection = connectionResult.connection;
        // Use the USER's identity (from their request), not the admin identity
        const accountData = await resolveAccountData(connection, userIdentity!); // userIdentity is validated above
        accountId = accountData.accountId;
        currentVersion = accountData.currentVersion;
        console.log(`[Profile] Resolved account ${accountId} with pfp version ${currentVersion}`);
    } catch (err) {
        const errorMessage = err instanceof Error ? err.message : String(err);
        const errorStack = err instanceof Error ? err.stack : undefined;
        console.error("[Profile] Failed to resolve account data:", errorMessage);
        console.error("[Profile] Error stack:", errorStack);
        connection?.disconnect();
        if (err && typeof err === "object" && "status" in err) {
            throw err; // Re-throw SvelteKit errors
        }
        throw error(401, `Unable to verify your SpacetimeDB account: ${errorMessage}`);
    }

    if (!connection) {
        throw error(500, "SpacetimeDB connection is not available");
    }
    const activeConnection = connection;

    // Get R2 bucket binding
    const bucket = env.MARBLES_BUCKET_BINDING;
    if (!bucket) {
        console.error("[Profile] R2 bucket binding not available");
        throw error(500, "Profile picture storage is not configured");
    }

    // Upload the image to R2
    // Use appropriate extension based on content type
    const isWebp = bytes.length >= 12 && getFourCC(bytes, 8) === "WEBP";
    const extension = isWebp ? "webp" : "jpg"; // Default to jpg for OAuth images
    const mimeType = isWebp ? "image/webp" : "image/jpeg";

    const objectKey = `${PROFILE_STORAGE_PREFIX}${accountId.toString()}.${extension}`;
    const nextVersion = Math.min(currentVersion + 1, MAX_PROFILE_VERSION);

    try {
        console.log(`[Profile] Uploading ${bytes.length} bytes to R2 as ${objectKey}`);
        await uploadProfilePictureToR2(bucket, objectKey, bytes, mimeType);
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
    const url = buildProfilePictureUrl(
        env.VITE_PFP_CDN_BASE_URL,
        accountId,
        nextVersion,
        extension
    );

    console.log("[Profile] Returning profile picture URL:", url);

    return json({
        accountId: accountId.toString(),
        version: nextVersion,
        url,
    });
};
