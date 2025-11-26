const load = async ({ platform }) => {
  const spacetimedbHost = platform?.env?.SPACETIMEDB_HOST || "ws://localhost:3000";
  const spacetimedbDbName = platform?.env?.SPACETIMEDB_DB_NAME || "marbles2";
  return {
    spacetimedbConfig: {
      host: spacetimedbHost,
      moduleName: spacetimedbDbName
    }
  };
};
export {
  load
};
