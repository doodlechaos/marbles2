// See https://svelte.dev/docs/kit/types#app.d.ts
// for information about these interfaces

/// <reference types="@cloudflare/workers-types" />

declare global {
    namespace App {
        interface Platform {
            env: {
                // R2 bucket binding for profile pictures
                MARBLES_BUCKET_BINDING: R2Bucket;
                // CDN base URL for profile pictures
/*                 VITE_PFP_CDN_BASE_URL?: string;
                // SpacetimeDB config (from wrangler.jsonc vars)
                SPACETIMEDB_HOST: string;
                SPACETIMEDB_DB_NAME: string;
                // Admin token for server-side SpacetimeDB operations
                // Set via .dev.vars (local) or `wrangler secret put` (production)
                SPACETIMEDB_ADMIN_TOKEN?: string; */
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
