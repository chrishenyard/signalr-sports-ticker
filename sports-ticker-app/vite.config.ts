import { reactRouter } from "@react-router/dev/vite";
import tailwindcss from "@tailwindcss/vite";
import { defineConfig, loadEnv } from "vite";

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "");

  return {
    plugins: [tailwindcss(), reactRouter()],
    build: {
      outDir: "build/client",
      emptyOutDir: true,
      sourcemap: mode === "development" ? true : false,
      assetsDir: "assets",
      rollupOptions: {
        output: {
          manualChunks: undefined,
          entryFileNames: `assets/[name].js`,
          chunkFileNames: `assets/[name]-chunk.js`,
          assetFileNames: `assets/[name].[ext]`,
        },
      },
    },
    server: {
      port: 5173,
      host: true,
      proxy: {
        "/sports-ticker": {
          target: env.VITE_SIGNALR_HUB_URL || "https://localhost:9001",
          changeOrigin: true,
          secure: false,
          ws: true,
        },
      },
    },
  };
});
