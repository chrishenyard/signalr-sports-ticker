using SportsTicker.Services;
using System.Collections.Generic;

namespace SportsTicker.Entities
{
    public class Scores
    {
        public List<Game> Games { get; set; } = [];
        public GameClock GameClock { get; set; }
    }
}