using SportsTicker.Services;

namespace SportsTickerTests.Builders {


	public class GameManagerBuilder {
		private int quarter = 1;
		private int quarterTime = 300;
		private int quarterBreakTime = 60;
		private int halfBreakTime = 180;
		private int elapsedSeconds = 0;

		public GameManager Build() {
			var gameClock = new GameClock {
				Quarter = quarter,
				QuarterTime = quarterTime,
				QuarterBreakTime = quarterBreakTime,
				HalfBreakTime = halfBreakTime,
				Clock = new Watch(elapsedSeconds)
			};

			var gameManager = new GameManager(gameClock);
			return gameManager;
		}

		public GameManagerBuilder WithQuarter(int quarter) {
			this.quarter = quarter;
			return this;
		}

		public GameManagerBuilder WithQuarterTime(int quarterTime) {
			this.quarterTime = quarterTime;
			return this;
		}

		public GameManagerBuilder WithQuarterBreakTime(int quarterBreakTime) {
			this.quarterBreakTime = quarterBreakTime;
			return this;
		}

		public GameManagerBuilder WithHalfBreakTime(int halfBreakTime) {
			this.halfBreakTime = halfBreakTime;
			return this;
		}

		public GameManagerBuilder WithElapsedSeconds(int elapsedSeconds) {
			this.elapsedSeconds = elapsedSeconds;
			return this;
		}
	}
}
