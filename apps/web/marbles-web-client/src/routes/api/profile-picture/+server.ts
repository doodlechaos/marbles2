// apps/web/marbles-web-client/src/routes/api/profile-picture/+server.ts
// Profile picture upload endpoint using SpacetimeDB HTTP API and R2 bucket binding

import { json, error } from "@sveltejs/kit";
import type { RequestHandler } from "./$types";
import { AccountCustomization, DbConnection, type ErrorContext } from "../../../module_bindings";
import { Identity } from "spacetimedb";
import { Jimp } from "jimp";

// Configuration constants
const PROFILE_STORAGE_PREFIX = "pfp/";
const MAX_PROFILE_VERSION = 255;
const PFP_MAX_BYTES = 2 * 1024 * 1024; // 2MB

// Accepted image types for upload (will be converted to PNG)
const ACCEPTED_IMAGE_TYPES = [
    "image/png",
    "image/jpeg",
    "image/jpg",
    "image/webp",
    "image/gif",
    "image/bmp",
    "image/tiff",
];

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

async function connectToSpacetimeDb(): Promise<{ connection: DbConnection; identity: Identity }> {
    // env: App.Platform["env"]
    const host = import.meta.env.VITE_SPACETIMEDB_HOST;
    const moduleName = import.meta.env.VITE_SPACETIMEDB_DB_NAME;
    const token = import.meta.env.VITE_SPACETIMEDB_ADMIN_TOKEN;

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
    identity: Identity
): Promise<{ accountId: bigint; currentVersion: number }> {
    console.log(`[Profile] Starting resolve account data for identity: ${identity.toHexString()}`);
    await subscribeAndWait(connection, [`SELECT * FROM Account`]);
    console.log("[Profile] After subscribe and wait");

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
    const query = `SELECT * FROM AccountCustomization`;
    console.log("Starting subscribe for account_customization with Query: ", query);
    await subscribeAndWait(connection, [query]);
    console.log("After subscribe and wait for account_customization");
    const customization = connection.db.accountCustomization.accountId.find(accountId);

    const currentVersion = customization ? Number(customization.pfpVersion) : 0;
    return { accountId, currentVersion };
}

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

/**
 * Download image from URL and return as bytes with detected content type
 */
async function downloadImageFromUrl(
    imageUrl: string
): Promise<{ bytes: Uint8Array; contentType: string }> {
    console.log(`[Profile] Downloading image from URL: ${imageUrl}`);
    const response = await fetch(imageUrl);
    if (!response.ok) {
        throw new Error(`Failed to download image: ${response.status}`);
    }
    const contentType = response.headers.get("content-type") ?? "image/jpeg";
    const buffer = await response.arrayBuffer();
    console.log(`[Profile] Downloaded ${buffer.byteLength} bytes, content-type: ${contentType}`);
    return { bytes: new Uint8Array(buffer), contentType };
}

/**
 * Detect image type from magic bytes (for logging/validation purposes)
 */
function detectImageType(bytes: Uint8Array): string {
    // Check magic bytes
    if (bytes.length >= 8) {
        // PNG: 89 50 4E 47 0D 0A 1A 0A
        if (bytes[0] === 0x89 && bytes[1] === 0x50 && bytes[2] === 0x4e && bytes[3] === 0x47) {
            return "png";
        }
        // JPEG: FF D8 FF
        if (bytes[0] === 0xff && bytes[1] === 0xd8 && bytes[2] === 0xff) {
            return "jpeg";
        }
        // GIF: 47 49 46 38
        if (bytes[0] === 0x47 && bytes[1] === 0x49 && bytes[2] === 0x46 && bytes[3] === 0x38) {
            return "gif";
        }
        // WebP: RIFF....WEBP
        if (
            bytes[0] === 0x52 &&
            bytes[1] === 0x49 &&
            bytes[2] === 0x46 &&
            bytes[3] === 0x46 &&
            bytes[8] === 0x57 &&
            bytes[9] === 0x45 &&
            bytes[10] === 0x42 &&
            bytes[11] === 0x50
        ) {
            return "webp";
        }
        // BMP: 42 4D
        if (bytes[0] === 0x42 && bytes[1] === 0x4d) {
            return "bmp";
        }
        // TIFF: 49 49 2A 00 or 4D 4D 00 2A
        if (
            (bytes[0] === 0x49 && bytes[1] === 0x49 && bytes[2] === 0x2a && bytes[3] === 0x00) ||
            (bytes[0] === 0x4d && bytes[1] === 0x4d && bytes[2] === 0x00 && bytes[3] === 0x2a)
        ) {
            return "tiff";
        }
    }
    return "unknown";
}

/**
 * Convert image bytes to PNG format using jimp
 */
async function convertToPng(imageBytes: Uint8Array): Promise<Uint8Array> {
    console.log(`[Profile] Converting image (${imageBytes.length} bytes) to PNG`);

    const image = await Jimp.read(Buffer.from(imageBytes));
    const pngBuffer = await image.getBuffer("image/png");

    console.log(`[Profile] Converted to PNG: ${pngBuffer.length} bytes`);
    return new Uint8Array(pngBuffer);
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
    let imageBytes: Uint8Array;
    let imageMimeType: string;
    let userIdentity: Identity | null = null;

    if (contentType.includes("multipart/form-data")) {
        // Form upload - accept any common image format
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

        // Accept common image formats
        if (!ACCEPTED_IMAGE_TYPES.includes(image.type)) {
            throw error(400, `Profile pictures must be one of: ${ACCEPTED_IMAGE_TYPES.join(", ")}`);
        }

        if (image.size > PFP_MAX_BYTES) {
            throw error(
                400,
                `Profile pictures must be smaller than ${PFP_MAX_BYTES / 1024 / 1024}MB`
            );
        }

        imageBytes = new Uint8Array(await image.arrayBuffer());
        imageMimeType = image.type;
        console.log(`[Profile] Received ${imageBytes.length} bytes ${imageMimeType} upload`);
    } else if (contentType.includes("application/json")) {
        // JSON body with imageUrl and identity - download from OAuth provider
        const body = (await request.json()) as { imageUrl?: string; identity?: string };

        if (!body.imageUrl || typeof body.imageUrl !== "string") {
            throw error(400, "Request must include an imageUrl");
        }

        if (!body.identity || typeof body.identity !== "string") {
            throw error(400, "Request must include an identity");
        }

        try {
            new URL(body.imageUrl);
        } catch {
            throw error(400, "Invalid imageUrl provided");
        }

        try {
            userIdentity = Identity.fromString(body.identity);
        } catch {
            throw error(400, "Invalid identity format");
        }

        console.log("[Profile] Downloading image from URL:", body.imageUrl);

        try {
            const downloaded = await downloadImageFromUrl(body.imageUrl);
            imageBytes = downloaded.bytes;
            imageMimeType = downloaded.contentType;
        } catch (err) {
            console.error("[Profile] Failed to download image:", err);
            throw error(400, "Failed to download image from URL");
        }
    } else {
        throw error(400, "Request must be multipart/form-data or application/json");
    }

    if (!userIdentity) {
        throw error(400, "Identity is required");
    }
    console.log(`[Profile] User identity: ${userIdentity.toHexString()}`);

    // Detect original image type for logging
    const detectedType = detectImageType(imageBytes);
    console.log(`[Profile] Detected original image type: ${detectedType}`);

    // Convert image to PNG format
    let pngBytes: Uint8Array;
    try {
        pngBytes = await convertToPng(imageBytes);
    } catch (err) {
        console.error("[Profile] Failed to convert image to PNG:", err);
        throw error(400, "Failed to process image. Please ensure it's a valid image file.");
    }

    // Resolve account data via SpacetimeDB connection using admin token
    let connection: DbConnection | null = null;
    let accountId = 0n;
    let currentVersion = 0;

    try {
        console.log("[Profile] Attempting to connect to SpacetimeDB with admin token...");
        const connectionResult = await connectToSpacetimeDb();
        console.log(
            "[Profile] Connected to SpacetimeDB as admin, identity:",
            connectionResult.identity.toHexString()
        );
        connection = connectionResult.connection;
        const accountData = await resolveAccountData(connection, userIdentity!);
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
            throw err;
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

    // Upload the PNG image to R2
    const objectKey = `${PROFILE_STORAGE_PREFIX}${accountId.toString()}.png`;
    const nextVersion = Math.min(currentVersion + 1, MAX_PROFILE_VERSION);

    try {
        console.log(`[Profile] Uploading ${pngBytes.length} bytes PNG to R2 as ${objectKey}`);
        await uploadProfilePictureToR2(bucket, objectKey, pngBytes, "image/png");
        console.log("[Profile] Upload successful");

        console.log("[Profile] Calling IncrementPfpVersion reducer");
        await activeConnection.reducers.incrementPfpVersion({});
        console.log("[Profile] Reducer called successfully");
    } catch (err) {
        console.error("[Profile] Failed to upload profile picture:", err);
        throw error(500, "Failed to upload profile picture");
    } finally {
        activeConnection.disconnect();
    }

    // Build the profile picture URL
    const url = buildProfilePictureUrl(
        import.meta.env.VITE_PFP_CDN_BASE_URL,
        accountId,
        nextVersion,
        "png"
    );

    console.log("[Profile] Returning profile picture URL:", url);

    return json({
        accountId: accountId.toString(),
        version: nextVersion,
        url,
    });
};
