
// this file is generated — do not edit it


declare module "svelte/elements" {
	export interface HTMLAttributes<T> {
		'data-sveltekit-keepfocus'?: true | '' | 'off' | undefined | null;
		'data-sveltekit-noscroll'?: true | '' | 'off' | undefined | null;
		'data-sveltekit-preload-code'?:
			| true
			| ''
			| 'eager'
			| 'viewport'
			| 'hover'
			| 'tap'
			| 'off'
			| undefined
			| null;
		'data-sveltekit-preload-data'?: true | '' | 'hover' | 'tap' | 'off' | undefined | null;
		'data-sveltekit-reload'?: true | '' | 'off' | undefined | null;
		'data-sveltekit-replacestate'?: true | '' | 'off' | undefined | null;
	}
}

export {};


declare module "$app/types" {
	export interface AppTypes {
		RouteId(): "/" | "/api" | "/api/profile-picture" | "/api/stripe-webhook" | "/game";
		RouteParams(): {
			
		};
		LayoutParams(): {
			"/": Record<string, never>;
			"/api": Record<string, never>;
			"/api/profile-picture": Record<string, never>;
			"/api/stripe-webhook": Record<string, never>;
			"/game": Record<string, never>
		};
		Pathname(): "/" | "/api" | "/api/" | "/api/profile-picture" | "/api/profile-picture/" | "/api/stripe-webhook" | "/api/stripe-webhook/" | "/game" | "/game/";
		ResolvedPathname(): `${"" | `/${string}`}${ReturnType<AppTypes['Pathname']>}`;
		Asset(): "/unity-webgl/Build/unity-webgl.data.unityweb" | "/unity-webgl/Build/unity-webgl.framework.js.unityweb" | "/unity-webgl/Build/unity-webgl.loader.js" | "/unity-webgl/Build/unity-webgl.wasm.unityweb" | "/unity-webgl/TemplateData/webmemd-icon.png" | string & {};
	}
}