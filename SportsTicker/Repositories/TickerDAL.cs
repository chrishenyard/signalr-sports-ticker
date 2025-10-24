using SportsTicker.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsTicker.Repositories
{
    public interface ITickerDAL
    {
        Task<List<Game>> GetGamesAsync();
    }

    public class TickerDAL(ITickerFileRepository tickerFileRepository) : ITickerDAL
    {
        private readonly ITickerFileRepository _tickerFileRepository = tickerFileRepository;

        public async Task<List<Game>> GetGamesAsync()
        {
            var fileName = "Json/games.json";
            var games = await _tickerFileRepository.GetGamesAsync(fileName);
            return games;
        }
    }
}
