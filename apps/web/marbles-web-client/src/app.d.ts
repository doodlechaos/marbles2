// See https://svelte.dev/docs/kit/types#app.d.ts
// for information about these interfaces

/// <reference types="@cloudflare/workers-types" />

declare global {
	namespace App {
		interface Platform {
			env: {
				PROFILE_PICTURE_S3_ENDPOINT: string;
				PROFILE_PICTURE_S3_ACCESS_KEY_ID: string;
				PROFILE_PICTURE_S3_SECRET_ACCESS_KEY: string;
				PROFILE_PICTURE_S3_BUCKET: string;
				PROFILE_PICTURE_BASE_URL?: string;
			};
			caches: CacheStorage;
			context: ExecutionContext;
		}
	}
}

// Environment variables type declarations (for public client-side vars)
interface ImportMetaEnv {
	readonly VITE_SPACETIMEDB_HOST: string;
	readonly VITE_SPACETIMEDB_DB_NAME: string;
}

interface ImportMeta {
	readonly env: ImportMetaEnv;
}

export {};
