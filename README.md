# signalr-sports-ticker

Sample real-time sports ticker demonstrating a SignalR-backed ASP.NET Core 10 server with a React 19.2 front end. The project shows how to stream live game updates to connected clients using SignalR, asynchronous I/O, and JSON messages.

## What this project does

The solution implements a live sports ticker:
- A backend SignalR hub (hosted in an ASP.NET Core 10 Razor Pages app) pushes score updates, game events, and simple team/player stats in real time.
- A React 19.2 single-page client connects to the hub and renders live updates, letting users watch ongoing game progress without page reloads.
- Messages are transmitted as JSON and handled asynchronously to keep UI and server responsive under load.

Use cases demonstrated:
- Broadcasting score updates to many clients.
- Sending per-game or per-user updates.
- Simple UI built with utility CSS classes to display live ticker, stats, and navigation.

## Tools & technologies

- .NET 10 (ASP.NET Core 10) — server and SignalR hub, Razor Pages for server routing/hosting.
- SignalR — real-time bidirectional communication between server and clients.
- React 19.2 — client UI and hub consumer.
- Tailwind-style utility classes used in the React app for styling.
- Docker — container images and Dockerfile included for producing reproducible images.
- JSON — message format for payloads sent over SignalR.
- Asynchronous programming patterns (async/await) — used throughout to maximize scalability and responsiveness.

## Project layout (high level)

- `SportsTicker/` — ASP.NET Core 10 project (Razor Pages) hosting the SignalR hub and API endpoints.
- `sports-ticker-app/` — React 19.2 front-end application that connects to the SignalR hub. Uses `VITE_SIGNALR_HUB_URL` via `import.meta.env` to locate the server.
- `Dockerfile` — multi-stage Dockerfile to build and publish the server image.

## Quick start

1. Start the backend (from the `SportsTicker` folder):
   - `dotnet run` (or run from Visual Studio 2026).
2. Start the front end (from `sports-ticker-app`):
   - `npm install`
   - `npm run dev` (ensure `VITE_SIGNALR_HUB_URL` points to the running hub)
3. Open the client in your browser and start the live ticker.

See the source for more details on hub endpoints, message shapes, and client-side connection logic.

## Contributing & Notes

- The project demonstrates core concepts: SignalR, async IO, ASP.NET Core 10, React 19.2, and JSON messaging.
- Configure `VITE_SIGNALR_HUB_URL` in your front-end environment to point to your running hub.
- The repository includes a Dockerfile for building the server image for production or containerized testing.

