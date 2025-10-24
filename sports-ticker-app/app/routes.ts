import { type RouteConfig, index, route } from "@react-router/dev/routes";

export default [
    index("routes/Index.tsx"),
    route("ticker/:hubUrl", "./ticker/Sports-Ticker.tsx"),
] satisfies RouteConfig;
