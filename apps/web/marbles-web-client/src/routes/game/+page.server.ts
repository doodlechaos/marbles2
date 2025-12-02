import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ platform }) => {
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
