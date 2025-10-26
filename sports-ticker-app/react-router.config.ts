import type { Config } from "@react-router/dev/config";

export default {
  appDirectory: "app",
  ssr: false,
  buildDirectory: "build",
  future: {
    v8_middleware: false,
  },
} satisfies Config;
