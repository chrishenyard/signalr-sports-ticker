import type { Route } from "./+types/Index";
import { Link } from "react-router";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "Sports Ticker App" },
    { description: "Live sports ticker using SignalR and React" },
  ];
}

export default function Index() {
  return (
    <div className="py-8">
      <div className="max-w-4xl mx-auto px-4">
        <p>Environment: {import.meta.env.MODE}</p>
        <p>SignalR Server: {import.meta.env.VITE_SIGNALR_HUB_URL}</p>
        <div className="text-center mb-12">
          <h1 className="text-4xl font-bold text-gray-900 mb-4">Sports Ticker App</h1>
          <p className="text-xl text-gray-600 mb-8">
            This app displays live scores for ongoing games using SignalR and React. You can view
            real-time updates, team stats, and game progress.
          </p>
          <Link to="/ticker/sports-ticker">
            <button className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-6 rounded-lg text-lg transition-colors">
              Start Live Ticker
            </button>
          </Link>
        </div>

        <div className="grid md:grid-cols-3 gap-6 mt-12">
          <div className="bg-white rounded-lg shadow-md p-6">
            <h3 className="text-lg font-semibold mb-3">Real-time Updates</h3>
            <p className="text-gray-600">Get live score updates as they happen during the game.</p>
          </div>
          <div className="bg-white rounded-lg shadow-md p-6">
            <h3 className="text-lg font-semibold mb-3">Team Statistics</h3>
            <p className="text-gray-600">View comprehensive team stats and standings.</p>
          </div>
          <div className="bg-white rounded-lg shadow-md p-6">
            <h3 className="text-lg font-semibold mb-3">Player Performance</h3>
            <p className="text-gray-600">Track top performers and individual player stats.</p>
          </div>
        </div>
      </div>
    </div>
  );
}
