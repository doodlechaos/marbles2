import { sveltekit } from "@sveltejs/kit/vite";
import { defineConfig } from "vite";
import type { Plugin } from "vite";

// Plugin to serve Unity WebGL files with Brotli encoding
const unityBrotliPlugin = (): Plugin => ({
  name: "unity-brotli",
  configureServer(server) {
    server.middlewares.use((req, res, next) => {
      if (req.url && req.url.includes(".unityweb")) {
        // Intercept the writeHead method to ensure headers are set
        const originalWriteHead = res.writeHead;
        res.writeHead = function (statusCode: number, ...args: any[]) {
          res.setHeader("Content-Encoding", "br");
          res.setHeader("Content-Type", "application/octet-stream");
          return originalWriteHead.apply(res, [statusCode, ...args] as any);
        };
      }
      next();
    });
  },
  configurePreviewServer(server) {
    server.middlewares.use((req, res, next) => {
      if (req.url && req.url.includes(".unityweb")) {
        const originalWriteHead = res.writeHead;
        res.writeHead = function (statusCode: number, ...args: any[]) {
          res.setHeader("Content-Encoding", "br");
          res.setHeader("Content-Type", "application/octet-stream");
          return originalWriteHead.apply(res, [statusCode, ...args] as any);
        };
      }
      next();
    });
  },
});

export default defineConfig({
  plugins: [unityBrotliPlugin(), sveltekit()],
});
