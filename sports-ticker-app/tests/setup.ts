import "@testing-library/jest-dom";
import { vi } from "vitest";
import * as React from "react";

// Mock SignalR
vi.mock("@microsoft/signalr", () => ({
  HubConnectionBuilder: vi.fn().mockImplementation(() => ({
    withUrl: vi.fn().mockReturnThis(),
    withAutomaticReconnect: vi.fn().mockReturnThis(),
    configureLogging: vi.fn().mockReturnThis(),
    withKeepAliveInterval: vi.fn().mockReturnThis(),
    withServerTimeout: vi.fn().mockReturnThis(),
    build: vi.fn().mockReturnValue({
      start: vi.fn().mockResolvedValue(undefined),
      stop: vi.fn().mockResolvedValue(undefined),
      invoke: vi.fn().mockResolvedValue({ games: [], gameClock: {} }),
      on: vi.fn(),
      off: vi.fn(),
      onreconnected: vi.fn(),
      onclose: vi.fn(),
      state: "Disconnected",
    }),
  })),
  HubConnectionState: {
    Disconnected: "Disconnected",
    Connected: "Connected",
    Connecting: "Connecting",
    Reconnecting: "Reconnecting",
  },
  LogLevel: {
    Debug: 0,
    Information: 1,
  },
}));

// Mock React Router
vi.mock("react-router", () => ({
  useLoaderData: vi.fn(),
  Link: ({ to, children }: { to: string; children: React.ReactNode }) =>
    React.createElement("a", { href: to }, children),
}));

// Mock useHubConnection hook
const mockConnection = {
  start: vi.fn().mockResolvedValue(undefined),
  stop: vi.fn().mockResolvedValue(undefined),
  invoke: vi.fn().mockResolvedValue({ games: [], gameClock: {} }),
  on: vi.fn(),
  off: vi.fn(),
  onreconnected: vi.fn(),
  onclose: vi.fn(),
  state: "Disconnected",
};

const mockStartConnection = vi.fn().mockResolvedValue(undefined);

export const mockUseHubConnection = vi.fn().mockReturnValue({
  connection: mockConnection,
  connectionRef: { current: mockConnection },
  startConnection: mockStartConnection,
});

vi.mock("~/common/use-hub-connection", () => ({
  default: mockUseHubConnection,
}));

// Export mocks for use in tests
export { mockConnection, mockStartConnection };
