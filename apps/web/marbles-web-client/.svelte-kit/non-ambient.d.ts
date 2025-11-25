
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
		RouteId(): "/" | "/auth" | "/auth/callback" | "/auth/silent-callback" | "/game";
		RouteParams(): {
			
		};
		LayoutParams(): {
			"/": Record<string, never>;
			"/auth": Record<string, never>;
			"/auth/callback": Record<string, never>;
			"/auth/silent-callback": Record<string, never>;
			"/game": Record<string, never>
		};
		Pathname(): "/" | "/auth" | "/auth/" | "/auth/callback" | "/auth/callback/" | "/auth/silent-callback" | "/auth/silent-callback/" | "/game" | "/game/";
		ResolvedPathname(): `${"" | `/${string}`}${ReturnType<AppTypes['Pathname']>}`;
		Asset(): "/Build/static.data.unityweb" | "/Build/static.framework.js.unityweb" | "/Build/static.loader.js" | "/Build/static.wasm.unityweb" | "/index.html" | "/TemplateData/webmemd-icon.png" | string & {};
	}
}