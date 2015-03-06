using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SportsTicker.Services;
using SportsTicker.Entities;
using System.Threading.Tasks;

namespace SportsTicker {

	[HubName("sportsTicker")]
	public class SportsTickerHub : Hub {
		private Ticker _ticker;

		public SportsTickerHub() : this(Ticker.Instance) { }

		public SportsTickerHub(Ticker ticker) {
			_ticker = ticker;
		}

		public async Task StartTicker() {
			await _ticker.Start();
        }

		public async Task<Scores> GetAllGames() {
			var games = await _ticker.GetAllGames();
			return games;
		}
	}
}