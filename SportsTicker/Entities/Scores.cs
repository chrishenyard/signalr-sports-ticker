using System.Collections.Generic;
using SportsTicker.Services;

namespace SportsTicker.Entities {
	public class Scores {
		public List<Game> Games = new List<Game>();
		public GameClock GameClock;
	}
}