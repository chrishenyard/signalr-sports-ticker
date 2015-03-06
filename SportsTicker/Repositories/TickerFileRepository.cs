using SportsTicker.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using SportsTicker.Serializers;

namespace SportsTicker.Repositories {
	public interface ITickerFileRepository {
		Task<List<Game>> GetGamesAsync(string fileName);
		Task SaveGamesAsync(List<Game> games, string fileName);
	}

	public class TickerFileRepository : ITickerFileRepository {
		public async Task<List<Game>> GetGamesAsync(string fileName) {
			var games = new List<Game>();

			using (var reader = File.OpenText(fileName)) {
				var text = await reader.ReadToEndAsync();
				games = Json.Deserialize<List<Game>>(text);
			}

			return games;
		}

		public async Task SaveGamesAsync(List<Game> games, string fileName) {
			var text = Json.Serialize(games);

			using (var writer = File.CreateText(fileName)) {
				await writer.WriteAsync(text);
			}
		}
	}
}
