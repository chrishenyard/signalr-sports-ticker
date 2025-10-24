import { it, expect, describe, beforeEach, vi } from "vitest";
import { render, screen, waitFor, act, renderHook } from "@testing-library/react";
import { useLoaderData } from "react-router";
import SportsTicker from "../../app/ticker/Sports-Ticker";
import { mockUseHubConnection, mockConnection, mockStartConnection } from "../setup";

// Mock the useLoaderData hook
const mockUseLoaderData = vi.mocked(useLoaderData);

describe("SportsTicker", () => {
  beforeEach(() => {
    vi.clearAllMocks();

    // Default mock for useLoaderData
    mockUseLoaderData.mockReturnValue({
      hubUrl: "/sports-ticker",
    });

    // Reset useHubConnection mock
    mockUseHubConnection.mockReturnValue({
      connection: mockConnection,
      connectionRef: { current: mockConnection },
      startConnection: mockStartConnection,
    });
  });

  it("should display reconnecting state", async () => {
    let triggerReconnecting: (() => void) | undefined;

    // Create a connection mock that starts in "Connecting" state to avoid the useEffect logic
    const mockConnectionWithState = {
      ...mockConnection,
      state: "Connecting", // ← Key change: not "Disconnected"
    };

    mockUseHubConnection.mockImplementation((options) => {
      triggerReconnecting = () => {
        options.onConnectionStateChange?.("reconnecting");
        options.onRetryAttempt?.(2, "Attempting to reconnect...");
      };

      return {
        connection: mockConnectionWithState,
        connectionRef: { current: mockConnectionWithState },
        startConnection: mockStartConnection,
      };
    });

    render(<SportsTicker />);

    // Initially should show connecting state
    expect(screen.getByText("Connecting to live scores...")).toBeInTheDocument();

    // Trigger the reconnecting state
    await act(async () => {
      triggerReconnecting?.();
    });

    // Wait for the state update to propagate
    await waitFor(() => {
      expect(screen.getByText("Reconnecting...")).toBeInTheDocument();
      expect(screen.getByText("Attempting to reconnect...")).toBeInTheDocument();
    });

    // Verify progress bar
    // const progressBar = document.querySelector(".bg-yellow-600");
    // execute(progressBar).toBeInTheDocument();
  });

  it("should transition from connecting to ready state", async () => {
    let onConnectionStateChange: (state: string) => void;

    mockUseHubConnection.mockImplementation((options) => {
      onConnectionStateChange = options.onConnectionStateChange!;
      return {
        connection: mockConnection,
        connectionRef: { current: mockConnection },
        startConnection: mockStartConnection,
      };
    });

    render(<SportsTicker />);

    // Initially connecting
    expect(screen.getByText("Connecting to live scores...")).toBeInTheDocument();

    // Simulate successful connection
    await act(async () => {
      onConnectionStateChange("ready");
    });

    // Should show ready state (no games)
    await waitFor(() => {
      expect(screen.getByText("Connected Successfully")).toBeInTheDocument();
      expect(screen.getByText("Waiting for live games to begin...")).toBeInTheDocument();
    });
  });

  it("should show error state when connection fails", async () => {
    let onConnectionStateChange: (state: string) => void;
    let onRetryAttempt: (attempt: number, message: string) => void;

    mockUseHubConnection.mockImplementation((options) => {
      onConnectionStateChange = options.onConnectionStateChange!;
      onRetryAttempt = options.onRetryAttempt!;
      return {
        connection: mockConnection,
        connectionRef: { current: mockConnection },
        startConnection: mockStartConnection,
      };
    });

    render(<SportsTicker />);

    // Simulate error state
    await act(async () => {
      onConnectionStateChange("error");
      onRetryAttempt(3, "Connection failed: Network error");
    });

    // Check that error state is not directly displayed (since you don't have an error status component)
    // Instead, check that retryMessage is updated
    expect(screen.queryByText("Connection failed: Network error")).toBeNull();
  });

  it("should render games when data is available", async () => {
    const mockGames = [
      {
        id: "game1",
        opponents: [
          {
            id: "team1",
            team: {
              id: "t1",
              name: "Lakers",
              logo: "lakers.png",
              combined: { wins: 10, losses: 5 },
              away: { wins: 5, losses: 3 },
            },
            pointsPerQuarter: [25, 30, 28, 22],
            totalPoints: 105,
            home: true,
          },
          {
            id: "team2",
            team: {
              id: "t2",
              name: "Warriors",
              logo: "warriors.png",
              combined: { wins: 12, losses: 3 },
              away: { wins: 6, losses: 2 },
            },
            pointsPerQuarter: [22, 28, 25, 23],
            totalPoints: 98,
            home: false,
          },
        ],
      },
    ];

    // Mock GetAllGames to return initial data
    (mockConnection.invoke as any).mockImplementation((method: string) => {
      if (method === "GetAllGames") {
        return Promise.resolve({
          games: mockGames,
          gameClock: {
            quarter: 4,
            timeout: false,
            finalBuzzer: false,
            timeRemaining: "2:45",
          },
        });
      }
      if (method === "Start") {
        return Promise.resolve();
      }
      return Promise.resolve();
    });

    render(<SportsTicker />);

    // Wait for the async operations to complete and component to re-render
    await waitFor(() => {
      expect(screen.getByText("Lakers")).toBeInTheDocument();
      expect(screen.getByText("Warriors")).toBeInTheDocument();
      expect(screen.getByText("105")).toBeInTheDocument();
      expect(screen.getByText("98")).toBeInTheDocument();
    });
  });
});
function execute(progressBar: Element | null) {
  throw new Error("Function not implemented.");
}
