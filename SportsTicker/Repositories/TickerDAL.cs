using SportsTicker.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace SportsTicker.Repositories {
	public interface ITickerDAL {
		Task<List<Game>> GetGamesAsync();
		Task ResetGamesAsync();
	}

	public class TickerDAL : ITickerDAL {
		private readonly ITickerFileRepository _tickerFileRepository;
		private readonly string _basePath;
		private readonly string _gamesBasePath;

		public TickerDAL(ITickerFileRepository tickerFileRepository, string basePath) {
			_tickerFileRepository = tickerFileRepository;
			_basePath = Path.Combine(basePath, "app_data");
			_gamesBasePath = Path.Combine(_basePath, "games");
		}

		public async Task<List<Game>> GetGamesAsync() {
			var fileName = Path.Combine(_gamesBasePath, "games.json");
			var games = await _tickerFileRepository.GetGamesAsync(fileName);
			return games;
		}

		public async Task ResetGamesAsync() {
			var fileName = Path.Combine(_gamesBasePath, "games.json");
			var games = await _tickerFileRepository.GetGamesAsync(fileName);

			foreach (var game in games) {
				foreach (var opponent in game.Opponents) {
					opponent.PointsPerQuarter = new List<int> { 0, 0, 0, 0 };
					foreach (var player in opponent.Team.Players) {
						player.Stats = new Stats();
					}
				}
			}

			await _tickerFileRepository.SaveGamesAsync(games, fileName);

		}
	}
}
