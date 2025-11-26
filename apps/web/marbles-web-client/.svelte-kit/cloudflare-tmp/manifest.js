export const manifest = (() => {
function __memo(fn) {
	let value;
	return () => value ??= (value = fn());
}

return {
	appDir: "_app",
	appPath: "_app",
	assets: new Set(["Build/static.data.unityweb","Build/static.framework.js.unityweb","Build/static.loader.js","Build/static.wasm.unityweb","index.html","TemplateData/webmemd-icon.png"]),
	mimeTypes: {".js":"text/javascript",".html":"text/html",".png":"image/png"},
	_: {
		client: {start:"_app/immutable/entry/start.CLkRPflt.js",app:"_app/immutable/entry/app.B-bYZ_s8.js",imports:["_app/immutable/entry/start.CLkRPflt.js","_app/immutable/chunks/DUWyi8Xa.js","_app/immutable/chunks/CAOkcYHl.js","_app/immutable/chunks/CF5fhvwK.js","_app/immutable/entry/app.B-bYZ_s8.js","_app/immutable/chunks/CAOkcYHl.js","_app/immutable/chunks/ED71nHoW.js","_app/immutable/chunks/lZzistH4.js","_app/immutable/chunks/CF5fhvwK.js","_app/immutable/chunks/B2ZQOyCO.js"],stylesheets:[],fonts:[],uses_env_dynamic_public:false},
		nodes: [
			__memo(() => import('../output/server/nodes/0.js')),
			__memo(() => import('../output/server/nodes/1.js')),
			__memo(() => import('../output/server/nodes/2.js')),
			__memo(() => import('../output/server/nodes/3.js'))
		],
		remotes: {
			
		},
		routes: [
			{
				id: "/",
				pattern: /^\/$/,
				params: [],
				page: { layouts: [0,], errors: [1,], leaf: 2 },
				endpoint: null
			},
			{
				id: "/api/profile-picture",
				pattern: /^\/api\/profile-picture\/?$/,
				params: [],
				page: null,
				endpoint: __memo(() => import('../output/server/entries/endpoints/api/profile-picture/_server.ts.js'))
			},
			{
				id: "/game",
				pattern: /^\/game\/?$/,
				params: [],
				page: { layouts: [0,], errors: [1,], leaf: 3 },
				endpoint: null
			}
		],
		prerendered_routes: new Set([]),
		matchers: async () => {
			
			return {  };
		},
		server_assets: {}
	}
}
})();

export const prerendered = new Set([]);

export const base_path = "";
