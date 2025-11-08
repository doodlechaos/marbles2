import type { Handle } from "@sveltejs/kit";

export const handle: Handle = async ({ event, resolve }) => {
  const response = await resolve(event);

  // Add Content-Encoding header for pre-compressed Unity WebGL files
  if (event.url.pathname.includes(".unityweb")) {
    response.headers.set("Content-Encoding", "br");
    response.headers.set("Content-Type", "application/octet-stream");
    response.headers.set(
      "Cache-Control",
      "public, max-age=31536000, immutable"
    );
  }

  return response;
};
