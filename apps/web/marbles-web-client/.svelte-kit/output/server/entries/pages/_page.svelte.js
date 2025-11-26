import "clsx";
import { e as escape_html } from "../../chunks/escaping.js";
function Counter($$renderer) {
  let count = 0;
  $$renderer.push(`<button>count is ${escape_html(count)}</button>`);
}
function _page($$renderer) {
  $$renderer.push(`<main><div class="card">`);
  Counter($$renderer);
  $$renderer.push(`<!----></div> <p>Check out <a href="https://github.com/sveltejs/kit#readme" target="_blank" rel="noreferrer">SvelteKit</a>, the official Svelte app framework powered by Vite!</p> <p class="read-the-docs svelte-1uha8ag">Click on the Vite and Svelte logos to learn more</p> <p><a href="/game">Play the Game!</a></p></main>`);
}
export {
  _page as default
};
