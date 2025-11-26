// @ts-nocheck
import type { PageServerLoad } from "./$types";

export const load = async ({ platform }: Parameters<PageServerLoad>[0]) => {
    // Get SpacetimeDB config from platform env (wrangler.jsonc vars)
    const spacetimedbHost = platform?.env?.SPACETIMEDB_HOST || "ws://localhost:3000";
    const spacetimedbDbName = platform?.env?.SPACETIMEDB_DB_NAME || "marbles2";

    return {
        spacetimedbConfig: {
            host: spacetimedbHost,
            moduleName: spacetimedbDbName,
        },
    };
};
