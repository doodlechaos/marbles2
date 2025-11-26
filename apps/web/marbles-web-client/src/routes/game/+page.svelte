<script lang="ts">
  import { onMount } from "svelte";

  // Receive SpacetimeDB config from server
  let { data } = $props();

  let canvas: HTMLCanvasElement;
  let unityInstance: any = null;

  onMount(() => {
    // Inject SpacetimeDB config into window BEFORE Unity loads
    // This allows the Unity jslib plugin to read the config
    (window as any).SPACETIMEDB_CONFIG = {
      host: data.spacetimedbConfig.host,
      moduleName: data.spacetimedbConfig.moduleName,
    };

    console.log("[Game] SpacetimeDB config injected:", (window as any).SPACETIMEDB_CONFIG);

    // Load Unity loader script
    const script = document.createElement("script");
    script.src = "/unity-webgl/Build/unity-webgl.loader.js";
    script.onload = () => {
      // @ts-ignore - createUnityInstance is loaded from the Unity loader
      createUnityInstance(canvas, {
        arguments: [],
        dataUrl: "/unity-webgl/Build/unity-webgl.data.unityweb",
        frameworkUrl: "/unity-webgl/Build/unity-webgl.framework.js.unityweb",
        codeUrl: "/unity-webgl/Build/unity-webgl.wasm.unityweb",
        streamingAssetsUrl: "StreamingAssets",
        companyName: "DefaultCompany",
        productName: "MarblesUnityClient",
        productVersion: "0.1.0",
      })
        .then((instance: any) => {
          unityInstance = instance;
        })
        .catch((message: string) => {
          alert(message);
        });
    };

    document.head.appendChild(script);

    return () => {
      // Cleanup when component unmounts
      if (unityInstance) {
        unityInstance.Quit?.();
      }
    };
  });
</script>

<svelte:head>
  <title>Unity Web Player | MarblesUnityClient</title>
  <meta
    name="viewport"
    content="width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes"
  />
</svelte:head>

<div class="unity-container">
  <canvas id="unity-canvas" bind:this={canvas} tabindex="-1"></canvas>
</div>

<style>
  .unity-container {
    width: 100%;
    height: 100%;
    text-align: center;
    padding: 0;
    border: 0;
    margin: 0;
    position: fixed;
    top: 0;
    left: 0;
  }
  canvas {
    width: 100%;
    height: 100%;
    background: #231f20;
  }
  :global(html, body) {
    height: 100%;
    margin: 0;
    padding: 0;
    overflow: hidden;
  }
</style>
