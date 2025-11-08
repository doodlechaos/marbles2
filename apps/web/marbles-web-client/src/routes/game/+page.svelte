<script lang="ts">
  import { onMount } from "svelte";

  let canvas: HTMLCanvasElement;
  let unityInstance: any = null;

  onMount(() => {
    const isMobile = /iPhone|iPad|iPod|Android/i.test(navigator.userAgent);

    if (isMobile) {
      // Mobile device style: fill the whole browser client area with the game canvas
      const meta = document.createElement("meta");
      meta.name = "viewport";
      meta.content =
        "width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes";
      document.getElementsByTagName("head")[0].appendChild(meta);

      canvas.style.width = "100%";
      canvas.style.height = "100%";
      canvas.style.position = "fixed";
    }

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
</svelte:head>

<div class="unity-container">
  <canvas
    id="unity-canvas"
    bind:this={canvas}
    width="960"
    height="600"
    tabindex="-1"
  ></canvas>
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
