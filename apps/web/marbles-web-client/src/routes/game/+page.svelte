<script lang="ts">
  import { onMount } from "svelte";

  let canvas: HTMLCanvasElement;
  let unityInstance: any = null;

  onMount(() => {
    // Load Unity loader script
    const script = document.createElement("script");
    script.src = "/Build/static.loader.js";
    script.onload = () => {
      // @ts-ignore - createUnityInstance is loaded from the Unity loader
      createUnityInstance(canvas, {
        arguments: [],
        dataUrl: "/Build/static.data.unityweb",
        frameworkUrl: "/Build/static.framework.js.unityweb",
        codeUrl: "/Build/static.wasm.unityweb",
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
