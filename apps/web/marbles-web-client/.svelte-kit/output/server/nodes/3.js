import * as server from '../entries/pages/game/_page.server.ts.js';

export const index = 3;
let component_cache;
export const component = async () => component_cache ??= (await import('../entries/pages/game/_page.svelte.js')).default;
export { server };
export const server_id = "src/routes/game/+page.server.ts";
export const imports = ["_app/immutable/nodes/3.D6V_G3qs.js","_app/immutable/chunks/DFqY95ai.js","_app/immutable/chunks/H8HRPmpP.js","_app/immutable/chunks/C_Sh5Hij.js","_app/immutable/chunks/yg97jW04.js"];
export const stylesheets = ["_app/immutable/assets/3.CtrtHYcH.css"];
export const fonts = [];
