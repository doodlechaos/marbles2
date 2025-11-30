import { w as head } from "../../../chunks/index.js";
function _page($$renderer, $$props) {
  $$renderer.component(($$renderer2) => {
    let { data } = $$props;
    head("4p1id7", $$renderer2, ($$renderer3) => {
      $$renderer3.title(($$renderer4) => {
        $$renderer4.push(`<title>Unity Web Player | MarblesUnityClient</title>`);
      });
      $$renderer3.push(`<meta name="viewport" content="width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes"/>`);
    });
    $$renderer2.push(`<div class="unity-container svelte-4p1id7"><canvas id="unity-canvas" tabindex="-1" class="svelte-4p1id7"></canvas></div>`);
  });
}
export {
  _page as default
};
