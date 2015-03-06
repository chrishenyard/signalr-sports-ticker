using System.Collections.Generic;
using System.Linq;

namespace SportsTicker.Entities {
	public class Opponent {
		public Team Team { get; set; }
		public List<int> PointsPerQuarter;
		public bool Home { get; set; }
		public int TotalPoints {
			get { return PointsPerQuarter.Select(q => q).Sum(); }
		}
	}
}
