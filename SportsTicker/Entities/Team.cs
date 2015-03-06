using System.Collections.Generic;
using System.Linq;

namespace SportsTicker.Entities {
	public struct Standings {
		public int Wins;
		public int Losses;
	}

	public class Team {
		public string Name { get; set; }
		public List<Player> Players { get; set; }

		public Player HighPoints {
			get { return Players.Where(p => p.Stats.Points == Players.Max(s => s.Stats.Points)).First(); }
		}
		public Player HighRebounds {
			get { return Players.Where(p => p.Stats.Rebounds == Players.Max(s => s.Stats.Rebounds)).First(); }
		}
		public Player HighAssists {
			get { return Players.Where(p => p.Stats.Assists == Players.Max(s => s.Stats.Assists)).First(); }
		}
		public Standings Home { get; set; }
		public Standings Away { get; set; }
		public Standings Combined { get; set; }
		public string Logo { get; set; }
	}
}
