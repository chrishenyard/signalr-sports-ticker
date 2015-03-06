using System.Collections.Generic;

namespace SportsTicker.Entities {
	public class Game {
		public string Id { get; set; }
		public List<Opponent> Opponents { get; set; }
	}
}
