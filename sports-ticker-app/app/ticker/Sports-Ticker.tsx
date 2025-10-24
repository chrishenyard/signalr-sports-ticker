import { useEffect, useState, useCallback, useRef } from "react";
import { useLoaderData } from "react-router";
import * as signalR from "@microsoft/signalr";
import { HighlightSpan } from "../components/Highlight-Span";
import { getQuarterOrdinal } from "../utils/lib";
import useHubConnection from "../common/use-hub-connection";
import "./sports-ticker.css";
import type { Route } from "./+types/Sports-Ticker";
import type { Game, Scores, GameClock, HubReconnectingProps } from "./score-types";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "Sports Ticker" },
    {
      name: "description",
      content: "A live-updating sports ticker using ASP.NET Core SignalR and React.",
    },
  ];
}

export async function clientLoader({ params }: Route.ClientLoaderArgs) {
  let hubUrl = params.hubUrl ?? "/sports-ticker";

  if (!hubUrl.startsWith("/")) {
    hubUrl = `/${hubUrl}`;
  }

  return { hubUrl };
}

export default function SportsTicker() {
  const loaderData = useLoaderData();
  const { hubUrl } = loaderData;
  const [status, setStatus] = useState("connecting");
  const [games, setGames] = useState<Game[]>([]);
  const [gameClock, setGameClock] = useState<GameClock>({} as GameClock);
  const [error, setError] = useState<Error | null>(null);
  const [retryAttempt, setRetryAttempt] = useState(0);
  const [retryMessage, setRetryMessage] = useState("");
  const attemptsRef = useRef(0);
  const maxRetries = 5;

  function updateScores(scores: Scores) {
    setGameClock(scores?.gameClock ?? ({} as GameClock));
    setGames((prev: Game[]) => {
      const list = Array.isArray(prev) ? prev : [];
      const map = new Map<string, Game>(list.map((g) => [g.id, g]));
      for (const g of scores?.games ?? []) {
        map.set(g.id, map.has(g.id) ? { ...map.get(g.id), ...g } : g);
      }
      return [...map.values()];
    });
  }

  const handleRetryAttempt = useCallback((attempt: number, message: string) => {
    attemptsRef.current = attempt;
    setRetryAttempt(attempt);
    setRetryMessage(message);
  }, []);

  const cleanUp = useCallback(() => {
    const connection = connectionRef.current;
    const attempts = attemptsRef.current;

    if (!connection) return;

    if (
      connection.state === signalR.HubConnectionState.Connected ||
      (connection.state === signalR.HubConnectionState.Reconnecting && attempts >= maxRetries)
    ) {
      window.removeEventListener("beforeunload", handleBeforeUnload);
      connection.off("UpdateScores");
      connection.stop().catch((err) => {
        setError(err instanceof Error ? err : new Error(String(err)));
      });
      connectionRef.current = null;
    }
  }, []);

  const { connection, connectionRef, startConnection } = useHubConnection({
    hubUrl: hubUrl,
    maxRetries: maxRetries,
    onConnectionStateChange: setStatus,
    onRetryAttempt: handleRetryAttempt,
    onCleanUp: cleanUp,
  });

  const handleBeforeUnload = (event: BeforeUnloadEvent) => {
    event.preventDefault();
    cleanUp();
  };

  if (error) {
    throw error;
  }

  useEffect(() => {
    (async () => {
      try {
        if (connection.state === signalR.HubConnectionState.Disconnected) {
          connection.on("UpdateScores", (s) => updateScores(s));

          await startConnection();

          const initial: Scores = await connection.invoke("GetAllGames");

          setGames(initial?.games ?? []);
          setGameClock(initial?.gameClock ?? ({} as GameClock));
          setStatus("ready");

          await connection.invoke("Start");
        }
      } catch (err) {
        setError(err instanceof Error ? err : new Error(String(err)));
      }
    })();

    window.addEventListener("beforeunload", handleBeforeUnload);

    return () => {
      cleanUp();
    };
  }, [connection]);

  return (
    <div className="max-w-4xl mx-auto px-4 py-6">
      {status === "connecting" && <HubConnecting />}

      {status === "reconnecting" && (
        <HubReconnecting
          retryAttempt={retryAttempt}
          retryMessage={retryMessage}
          maxRetries={maxRetries}
        />
      )}

      {status === "ready" && games.length === 0 && <HubReady />}

      {games.length > 0 && (
        <>
          {games.map((game) => (
            <div
              className={`${gameClock.timeout ? "opacity-50 " : ""} mb-5 mx-auto my-2 max-w-md rounded overflow-hidden shadow-md text-xs`}
              key={game.id}
            >
              <div className="flex bg-gray-200 px-2 py-2">
                <div className="w-5/12 text-left text-red-700">
                  {gameClock.finalBuzzer
                    ? "Final"
                    : gameClock.timeout
                      ? "Timeout"
                      : getQuarterOrdinal(gameClock.quarter) +
                        ` Quarter ${gameClock.timeRemaining}`}
                </div>
                <div className="w-5/12 flex justify-end items-center">
                  <p className="w-8 px-2 text-center">1</p>
                  <p className="w-8 px-2 text-center">2</p>
                  <p className="w-8 px-2 text-center">3</p>
                  <p className="w-8 px-2 text-center">4</p>
                </div>
                <div className="w-1/6 text-gray-700 text-right">Prime</div>
              </div>

              {(game.opponents ?? []).map((op) => (
                <div className="flex px-2 py-2 items-center" key={op.id}>
                  <div className="w-5/12 flex">
                    <img
                      className="w-6 sm:w-10 mr-2 self-center"
                      alt={`${op.team?.name} logo`}
                      src={op.team?.logo}
                    />
                    <div className="flex flex-col">
                      <p className="text-sm font-bold">{op.team?.name}</p>
                      <p className="hidden sm:block text-gray-600">
                        ({op.team?.combined?.wins}-{op.team?.combined?.losses},{" "}
                        {op.team?.away?.wins}-{op.team?.away?.losses} Away)
                      </p>
                    </div>
                  </div>

                  <div className="w-5/12 flex justify-end items-center">
                    {[0, 1, 2, 3].map((q) => (
                      <div key={q}>
                        <p className="w-8 px-1 text-center">
                          <HighlightSpan value={op.pointsPerQuarter?.[q]}>
                            {op.pointsPerQuarter?.[q] > 0 ? op.pointsPerQuarter[q] : 0}
                          </HighlightSpan>
                        </p>
                      </div>
                    ))}
                  </div>

                  <p className="w-1/6 text-lg sm:text-xl font-bold text-right">
                    <HighlightSpan value={op.totalPoints}>
                      {op.totalPoints > 0 ? op.totalPoints : 0}
                    </HighlightSpan>
                  </p>
                </div>
              ))}

              <div className="hidden sm:block border-t border-gray-300">
                <p className="text-center text-gray-500 m-1 uppercase">Top Performers</p>
                <div className="flex">
                  <div className="w-1/2 px-2 py-2">
                    <div className="flex">
                      <img
                        className="w-10 mr-2 self-center"
                        src={game.opponents?.[0]?.team?.highPoints?.headshot}
                      />
                      <div className="flex flex-col">
                        <p className="font-semibold">
                          {game.opponents?.[0]?.team?.highPoints?.firstName}{" "}
                          {game.opponents?.[0]?.team?.highPoints?.lastName}
                        </p>
                        <p className="text-gray-600">
                          {game.opponents?.[0]?.team?.highPoints?.stats?.points} PTS,
                          {game.opponents?.[0]?.team?.highRebounds?.stats?.rebounds} REB,
                          {game.opponents?.[0]?.team?.highAssists?.stats?.assists} AST
                        </p>
                      </div>
                    </div>
                  </div>
                  <div className="w-1/2 px-2 py-2">
                    <div className="flex">
                      <img
                        className="w-10 mr-2 self-center"
                        src={game.opponents?.[1]?.team?.highPoints?.headshot}
                      />
                      <div className="flex flex-col">
                        <p className="font-semibold">
                          {game.opponents?.[1]?.team?.highPoints?.firstName}{" "}
                          {game.opponents?.[1]?.team?.highPoints?.lastName}
                        </p>
                        <p className="text-gray-600">
                          {game.opponents?.[1]?.team?.highPoints?.stats?.points} PTS,
                          {game.opponents?.[1]?.team?.highRebounds?.stats?.rebounds} REB,
                          {game.opponents?.[1]?.team?.highAssists?.stats?.assists} AST
                        </p>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </>
      )}
    </div>
  );
}

function HubConnecting() {
  return (
    <div className="mb-6 p-4 bg-blue-50 border border-blue-200 rounded-lg">
      <div className="flex items-center">
        <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-blue-600 mr-3"></div>
        <div>
          <p className="text-blue-800 font-medium">Connecting to live scores...</p>
          <p className="text-blue-600 text-sm">
            Establishing connection to the sports ticker service
          </p>
        </div>
      </div>
    </div>
  );
}

function HubReconnecting({ retryAttempt, retryMessage, maxRetries }: HubReconnectingProps) {
  return (
    <div className="mb-6 p-4 bg-yellow-50 border border-yellow-200 rounded-lg">
      <div className="flex items-center">
        <div className="animate-pulse rounded-full h-5 w-5 bg-yellow-500 mr-3"></div>
        <div>
          <p className="text-yellow-800 font-medium">Reconnecting...</p>
          <p className="text-yellow-700 text-sm">{retryMessage}</p>
          <div className="mt-2 w-full bg-yellow-200 rounded-full h-2">
            <div
              className="bg-yellow-600 h-2 rounded-full transition-all duration-300"
              style={{ width: `${(retryAttempt / maxRetries) * 100}%` }}
            ></div>
          </div>
        </div>
      </div>
    </div>
  );
}

function HubReady() {
  return (
    <div className="mb-6 p-4 bg-green-50 border border-green-200 rounded-lg">
      <div className="flex items-center">
        <div className="rounded-full h-5 w-5 bg-green-500 mr-3 flex items-center justify-center">
          <svg className="w-3 h-3 text-white" fill="currentColor" viewBox="0 0 20 20">
            <path
              fillRule="evenodd"
              d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
              clipRule="evenodd"
            />
          </svg>
        </div>
        <div>
          <p className="text-green-800 font-medium">Connected Successfully</p>
          <p className="text-green-700 text-sm">Waiting for live games to begin...</p>
        </div>
      </div>
    </div>
  );
}
