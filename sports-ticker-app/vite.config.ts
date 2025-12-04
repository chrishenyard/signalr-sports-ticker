import { reactRouter } from "@react-router/dev/vite";
import tailwindcss from "@tailwindcss/vite";
import { defineConfig, loadEnv } from "vite";
import tsconfigPaths from "vite-tsconfig-paths";

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "");

  return {
    plugins: [tailwindcss(), reactRouter()],
    build: {
      outDir: "build",
      emptyOutDir: true,
      assetsDir: "assets",
      rollupOptions: {
        output: {
          manualChunks: undefined,
          entryFileNames: `assets/index.js`,
          chunkFileNames: `assets/[name]-chunk.js`,
          assetFileNames: `assets/[name].[ext]`,
          format: "es",
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
