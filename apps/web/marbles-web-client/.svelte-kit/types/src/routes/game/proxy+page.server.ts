// @ts-nocheck
import type { PageServerLoad } from "./$types";

export const load = async ({ platform }: Parameters<PageServerLoad>[0]) => {
    // Get SpacetimeDB config from platform env (wrangler.jsonc vars)
    const spacetimedbHost = import.meta.env.VITE_SPACETIMEDB_HOST; //|| "ws://localhost:3000";
    const spacetimedbDbName = import.meta.env.VITE_SPACETIMEDB_DB_NAME; // || "marbles2";

    if (!spacetimedbHost || !spacetimedbDbName) {
        throw new Error("SpacetimeDB config not found");
    }

    return {
        spacetimedbConfig: {
            host: spacetimedbHost,
            moduleName: spacetimedbDbName,
        },
    };
};
