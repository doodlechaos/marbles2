// apps/web/marbles-web-client/src/routes/api/profile-picture/+server.ts
// Profile picture upload endpoint using SpacetimeDB HTTP API

import { AwsClient } from 'aws4fetch';
import { json, error } from '@sveltejs/kit';
import type { RequestHandler } from './$types';

// Configuration constants
const PROFILE_STORAGE_PREFIX = 'pfp/';
const MAX_PROFILE_VERSION = 255;
const PFP_MIME = 'image/webp';
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
		((data[offset + 4] & 0x0f) << 10) | (data[offset + 3] << 2) | ((data[offset + 2] & 0xc0) >> 6);
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
	if (getFourCC(data, 0) !== 'RIFF' || getFourCC(data, 8) !== 'WEBP') {
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
		if (chunkId === 'VP8 ') {
			return parseLossyDimensions(data, chunkDataOffset, chunkSize);
		}
		if (chunkId === 'VP8L') {
			return parseLosslessDimensions(data, chunkDataOffset, chunkSize);
		}
		if (chunkId === 'VP8X') {
			return parseExtendedDimensions(data, chunkDataOffset, chunkSize);
		}
		offset = chunkDataOffset + chunkSize + (chunkSize & 1);
	}
	return null;
}

// SpacetimeDB HTTP API functions
interface SpacetimeDBConfig {
	host: string;
	moduleName: string;
}

function getSpacetimeDBConfig(): SpacetimeDBConfig {
	const host = import.meta.env.VITE_SPACETIMEDB_HOST?.trim();
	const moduleName = import.meta.env.VITE_SPACETIMEDB_DB_NAME?.trim();

	if (!host || !moduleName) {
		throw error(500, 'SpacetimeDB connection is not configured');
	}

	return { host, moduleName };
}

function buildSpacetimeDBUrl(config: SpacetimeDBConfig, path: string): string {
	// Normalize host URL - ensure it ends without trailing slash
	let baseUrl = config.host.replace(/\/+$/, '');

	// Convert WebSocket URL to HTTP if needed
	if (baseUrl.startsWith('ws://')) {
		baseUrl = baseUrl.replace('ws://', 'http://');
	} else if (baseUrl.startsWith('wss://')) {
		baseUrl = baseUrl.replace('wss://', 'https://');
	}

	return `${baseUrl}${path}`;
}

interface SqlQueryResult {
	schema: {
		elements: Array<{
			name: { some: string } | null;
			algebraic_type: unknown;
		}>;
	};
	rows: unknown[][];
}

async function executeSpacetimeDBSql(
	config: SpacetimeDBConfig,
	token: string,
	query: string
): Promise<SqlQueryResult[]> {
	const url = buildSpacetimeDBUrl(config, `/v1/database/${config.moduleName}/sql`);

	const response = await fetch(url, {
		method: 'POST',
		headers: {
			Authorization: `Bearer ${token}`,
			'Content-Type': 'text/plain'
		},
		body: query
	});

	if (!response.ok) {
		const text = await response.text().catch(() => '');
		console.error(`[Profile] SpacetimeDB SQL error (${response.status}): ${text}`);
		throw error(response.status === 401 ? 401 : 500, 'Failed to query SpacetimeDB');
	}

	return response.json();
}

async function callSpacetimeDBReducer(
	config: SpacetimeDBConfig,
	token: string,
	reducerName: string,
	args: unknown[] = []
): Promise<void> {
	const url = buildSpacetimeDBUrl(config, `/v1/database/${config.moduleName}/call/${reducerName}`);

	const response = await fetch(url, {
		method: 'POST',
		headers: {
			Authorization: `Bearer ${token}`,
			'Content-Type': 'application/json'
		},
		body: JSON.stringify(args)
	});

	if (!response.ok) {
		const text = await response.text().catch(() => '');
		console.error(`[Profile] SpacetimeDB reducer error (${response.status}): ${text}`);
		throw error(response.status === 401 ? 401 : 500, 'Failed to call SpacetimeDB reducer');
	}
}

interface AccountData {
	accountId: bigint;
	currentVersion: number;
}

async function resolveAccountData(config: SpacetimeDBConfig, token: string): Promise<AccountData> {
	// Query the account table - RLS (Row-Level Security) will filter to the caller's account
	const accountResults = await executeSpacetimeDBSql(config, token, 'SELECT * FROM Account');

	if (!accountResults || accountResults.length === 0 || accountResults[0].rows.length === 0) {
		console.error('[Profile] No account found for this identity');
		throw error(404, 'No account is associated with this identity');
	}

	// Parse account row - find the id field
	const accountSchema = accountResults[0].schema;
	const accountRow = accountResults[0].rows[0];

	// Find the index of the 'id' field in the schema
	const idIndex = accountSchema.elements.findIndex(
		(el) => el.name && 'some' in el.name && el.name.some === 'id'
	);

	if (idIndex === -1) {
		console.error('[Profile] Could not find id field in account schema');
		throw error(500, 'Invalid account schema');
	}

	const accountId = BigInt(accountRow[idIndex] as string | number);

	// Query account_customization table
	const customizationResults = await executeSpacetimeDBSql(
		config,
		token,
		'SELECT * FROM AccountCustomization'
	);

	let currentVersion = 0;

	if (
		customizationResults &&
		customizationResults.length > 0 &&
		customizationResults[0].rows.length > 0
	) {
		const customizationSchema = customizationResults[0].schema;
		const customizationRow = customizationResults[0].rows[0];

		// Find the pfp_version field
		const pfpVersionIndex = customizationSchema.elements.findIndex(
			(el) => el.name && 'some' in el.name && el.name.some === 'pfp_version'
		);

		if (pfpVersionIndex !== -1) {
			currentVersion = Number(customizationRow[pfpVersionIndex]);
		}
	}

	return { accountId, currentVersion };
}

// R2/S3 upload functions
function buildR2TargetUrl(endpoint: string, bucket: string, objectKey: string): string {
	const u = new URL(endpoint);
	const key = objectKey.replace(/^\/+/, '');

	// Does endpoint already include the bucket in the path?
	const pathSegs = u.pathname.split('/').filter(Boolean);
	const hasBucketInPath = pathSegs.length > 0 && pathSegs[0] === bucket;

	// Virtual host style? (bucket.<account>.r2.cloudflarestorage.com)
	const hasBucketInHost = u.hostname.startsWith(`${bucket}.`);

	if (hasBucketInPath) {
		u.pathname = `/${pathSegs.join('/')}/${key}`.replace(/\/{2,}/g, '/');
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
	env: App.Platform['env']
): Promise<void> {
	const endpoint = env.PROFILE_PICTURE_S3_ENDPOINT;
	const bucket = env.PROFILE_PICTURE_S3_BUCKET;
	const accessKeyId = env.PROFILE_PICTURE_S3_ACCESS_KEY_ID;
	const secretAccessKey = env.PROFILE_PICTURE_S3_SECRET_ACCESS_KEY;

	if (!endpoint || !bucket || !accessKeyId || !secretAccessKey) {
		throw error(500, 'Profile picture storage is not fully configured');
	}

	const awsClient = new AwsClient({
		accessKeyId,
		secretAccessKey,
		service: 's3',
		region: 'auto'
	});

	const targetUrl = buildR2TargetUrl(endpoint, bucket, objectKey);
	console.log('[Profile] Uploading to R2:', targetUrl);

	const res = await awsClient.fetch(targetUrl, {
		method: 'PUT',
		body,
		headers: {
			'Content-Type': contentType,
			'Cache-Control': 'public, max-age=31536000, immutable'
		}
	});

	if (!res.ok) {
		const text = await res.text().catch(() => '');
		throw new Error(
			`Failed to upload profile picture to R2 (${res.status}): ${text || 'No response body'}`
		);
	}
}

function buildProfilePictureUrl(
	accountId: bigint,
	version: number,
	env: App.Platform['env']
): string | null {
	const baseUrl = env.PROFILE_PICTURE_BASE_URL?.trim();
	if (!baseUrl || version <= 0) {
		return null;
	}
	const normalizedBase = baseUrl.replace(/\/$/, '');
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
	console.log('[Profile] Starting POST request');

	const env = platform?.env;
	if (!env) {
		throw error(500, 'Platform environment is not available');
	}

	// Extract and validate authorization token
	const authHeader = request.headers.get('authorization') ?? request.headers.get('Authorization');
	if (!authHeader?.startsWith('Bearer ')) {
		throw error(401, 'An ID token is required to upload profile pictures');
	}

	const token = authHeader.slice(7).trim();
	if (!token) {
		throw error(401, 'An ID token is required to upload profile pictures');
	}

	// Parse the request - supports both form-data with image OR JSON with imageUrl
	const contentType = request.headers.get('content-type') ?? '';
	let bytes: Uint8Array;

	if (contentType.includes('multipart/form-data')) {
		// Traditional form upload with WebP image
		const formData = await request.formData();
		const image = formData.get('image');

		if (!(image instanceof File)) {
			throw error(400, 'The request must include an image file');
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
			throw error(400, 'The uploaded image is not a valid WebP file');
		}

		if (dimensions.width !== PFP_IMAGE_SIZE || dimensions.height !== PFP_IMAGE_SIZE) {
			throw error(
				400,
				`Profile pictures must be exactly ${PFP_IMAGE_SIZE}×${PFP_IMAGE_SIZE} pixels`
			);
		}
	} else if (contentType.includes('application/json')) {
		// JSON body with imageUrl - download from OAuth provider
		const body = (await request.json()) as { imageUrl?: string };

		if (!body.imageUrl || typeof body.imageUrl !== 'string') {
			throw error(400, 'Request must include an imageUrl');
		}

		// Validate URL
		try {
			new URL(body.imageUrl);
		} catch {
			throw error(400, 'Invalid imageUrl provided');
		}

		console.log('[Profile] Downloading image from URL:', body.imageUrl);

		try {
			bytes = await downloadImageFromUrl(body.imageUrl);
		} catch (err) {
			console.error('[Profile] Failed to download image:', err);
			throw error(400, 'Failed to download image from URL');
		}

		// Note: Images from OAuth providers are typically JPEG/PNG, not WebP
		// We store them as-is since most browsers/clients can handle these formats
		// If WebP conversion is needed, it would require additional image processing
		console.log(`[Profile] Downloaded ${bytes.length} bytes from URL`);
	} else {
		throw error(400, 'Request must be multipart/form-data or application/json');
	}

	// Get SpacetimeDB configuration
	const stdbConfig = getSpacetimeDBConfig();

	// Resolve account data via SpacetimeDB HTTP API
	let accountId: bigint;
	let currentVersion: number;

	try {
		const accountData = await resolveAccountData(stdbConfig, token);
		accountId = accountData.accountId;
		currentVersion = accountData.currentVersion;
		console.log(`[Profile] Resolved account ${accountId} with pfp version ${currentVersion}`);
	} catch (err) {
		console.error('[Profile] Failed to resolve account data:', err);
		if (err && typeof err === 'object' && 'status' in err) {
			throw err; // Re-throw SvelteKit errors
		}
		throw error(401, 'Unable to verify your SpacetimeDB account');
	}

	// Upload the image to R2
	// Use appropriate extension based on content type
	const isWebp = bytes.length >= 12 && getFourCC(bytes, 8) === 'WEBP';
	const extension = isWebp ? 'webp' : 'jpg'; // Default to jpg for OAuth images
	const mimeType = isWebp ? 'image/webp' : 'image/jpeg';

	const objectKey = `${PROFILE_STORAGE_PREFIX}${accountId.toString()}.${extension}`;
	const nextVersion = Math.min(currentVersion + 1, MAX_PROFILE_VERSION);

	try {
		console.log(`[Profile] Uploading ${bytes.length} bytes to R2 as ${objectKey}`);
		await uploadProfilePictureToR2(bytes.buffer as ArrayBuffer, objectKey, mimeType, env);
		console.log('[Profile] Upload successful');

		// Call the IncrementPfpVersion reducer via HTTP API
		console.log('[Profile] Calling IncrementPfpVersion reducer');
		await callSpacetimeDBReducer(stdbConfig, token, 'IncrementPfpVersion');
		console.log('[Profile] Reducer called successfully');
	} catch (err) {
		console.error('[Profile] Failed to upload profile picture:', err);
		throw error(500, 'Failed to upload profile picture');
	}

	// Build the profile picture URL
	const url = buildProfilePictureUrl(accountId, nextVersion, env);

	console.log('[Profile] Returning profile picture URL:', url);

	return json({
		accountId: accountId.toString(),
		version: nextVersion,
		url
	});
};
