using Microsoft.Extensions.Logging;
using SportsTicker.Entities;
using SportsTicker.Services;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsTicker.Hubs
{
    public class SportsTickerHub(
        ITicker ticker,
        ILogger<SportsTickerHub> logger) : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly ITicker _ticker = ticker;
        private readonly ILogger<SportsTickerHub> _logger = logger;
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnections =
                new();

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            _userConnections.TryAdd(Context.ConnectionId, []);
            return base.OnConnectedAsync();
        }

        public int GetConnectionCount()
        {
            return _userConnections.Count;
        }

        public async Task Start()
        {
            await _ticker.Start();
        }

        public Scores GetAllGames()
        {
            var games = _ticker.GetAllGames();
            return games;
        }
    }
}