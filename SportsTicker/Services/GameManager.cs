using System.Configuration;
using System.Diagnostics;

namespace SportsTicker.Services {

	public class Watch {
		private Stopwatch sw;
		private long? elapsedSeconds;

		public Watch() {
			sw = new Stopwatch();
		}

		public Watch(long elapsedSeconds) : this() {
			this.elapsedSeconds = elapsedSeconds;
		}

		public long ElapsedSeconds {
            get { return elapsedSeconds.HasValue ? elapsedSeconds.Value : sw.ElapsedMilliseconds / 1000;  }
			set { elapsedSeconds = value;  }
		}

		public void Start() {
			sw.Start();
		}

		public void Restart() {
			sw.Restart();
		}
	}

	public class GameClock {
		public int TickerInterval = 0;
		public int Quarter = 1;
		public int QuarterTime = 0;
		public int QuarterBreakTime = 0;
		public int HalfBreakTime = 0;
		public bool Timeout;
		public bool FinalBuzzer = false;
		public bool InProgress = false;
		public Watch Clock;
	}

	public enum BreakType {
		None,
		QuarterBreak,
		HalfBreak
	}

	public abstract class Period {
		public GameManager GameManager;
		public BreakType BreakType;

		public abstract void ElapsedTime();
	}

	public class Timeout : Period {
		public Timeout(GameManager gameManager) {
			GameManager = gameManager;
			GameManager.GameClock.Timeout = true;
		}

		public override void ElapsedTime() {
			var elaspedTime = GameManager.GameClock.Clock.ElapsedSeconds;
			var breakTime = BreakType == BreakType.QuarterBreak ?
				GameManager.GameClock.QuarterBreakTime :
				GameManager.GameClock.HalfBreakTime;
			var gameClock = GameManager.GameClock;

			if (elaspedTime > breakTime) {
				GameManager.Period = new Quarter(GameManager);
				GameManager.Period.BreakType = BreakType.None;
                gameClock.Clock.Restart();
			}
		}
	}

	public class Quarter : Period {
		public Quarter(GameManager gameManager) {
			GameManager = gameManager;
			GameManager.GameClock.Timeout = false;
		}

		public override void ElapsedTime() {
			var elaspedTime = GameManager.GameClock.Clock.ElapsedSeconds;
			var quarterTime = GameManager.GameClock.QuarterTime;
			var gameClock = GameManager.GameClock;

			if (elaspedTime >= quarterTime) {
				if (gameClock.Quarter == 4) {
					gameClock.FinalBuzzer = true;
					return;
				}

				GameManager.Period = new Timeout(GameManager);

				if (gameClock.Quarter == 1 || gameClock.Quarter == 3)
					GameManager.Period.BreakType = BreakType.QuarterBreak;
				else
					GameManager.Period.BreakType = BreakType.HalfBreak;

				gameClock.Quarter++;
				gameClock.Clock.Restart();
			}
		}
	}

	public class GameManager {
		public GameClock GameClock;
		public Period Period;

		public GameManager() {
			GameClock = new GameClock {
				Quarter = 1,
				TickerInterval = int.Parse(ConfigurationManager.AppSettings["TickerInterval"]) * 1000,
				QuarterTime = int.Parse(ConfigurationManager.AppSettings["QuarterTime"]),
				QuarterBreakTime = int.Parse(ConfigurationManager.AppSettings["QuarterBreakTime"]),
				HalfBreakTime = int.Parse(ConfigurationManager.AppSettings["HalfBreakTime"]),
				Clock = new Watch()
			};

			Period = new Quarter(this);
		}

		public GameManager(GameClock gameClock) {
			GameClock = gameClock;
			Period = new Quarter(this);
		}
	}
}
