import { reactRouter } from "@react-router/dev/vite";
import tailwindcss from "@tailwindcss/vite";
import { defineConfig } from "vite";

export default defineConfig({
  plugins: [tailwindcss(), reactRouter()],
  build: {
    outDir: "build/client",
    emptyOutDir: true,
  },
  server: {
    port: 5173,
    host: true,
    proxy: {
      "/sports-ticker": {
        target: "https://localhost:9001",
        changeOrigin: true,
        secure: false,
        ws: true,
      },
    },
  },
});
