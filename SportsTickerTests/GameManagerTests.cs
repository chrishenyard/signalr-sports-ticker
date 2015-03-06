using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsTicker.Services;
using SportsTickerTests.Builders;

namespace SportsTickerTests {
	[TestClass]
	public class UnitTest1 {
		[TestMethod]
		public void GameManager_BeginningOfGame_ThenPeriodIsFirstQuarter() {
			var gm = new GameManagerBuilder()
				.WithElapsedSeconds(60)
				.Build();

			gm.Period.ElapsedTime();

			Assert.AreEqual(BreakType.None, gm.Period.BreakType);
			Assert.AreEqual(gm.GameClock.Quarter, 1);
			Assert.IsInstanceOfType(gm.Period, typeof(Quarter));
		}

		[TestMethod]
		public void GameManager_AfterFirstQuater_ThenPeriodIsTimeout() {
			var gm = new GameManagerBuilder()
				.WithElapsedSeconds(301)
				.WithQuarter(1)
				.Build();

			gm.Period.ElapsedTime();

			Assert.AreEqual(BreakType.QuarterBreak, gm.Period.BreakType);
			Assert.AreEqual(gm.GameClock.Quarter, 2);
			Assert.IsInstanceOfType(gm.Period, typeof(Timeout));
		}

		[TestMethod]
		public void GameManager_AfterSecondQuater_ThenPeriodIsHalftime() {
			var gm = new GameManagerBuilder()
				.WithElapsedSeconds(301)
				.WithQuarter(2)
				.Build();

			gm.Period.ElapsedTime();

			Assert.AreEqual(BreakType.HalfBreak, gm.Period.BreakType);
			Assert.AreEqual(gm.GameClock.Quarter, 3);
			Assert.IsInstanceOfType(gm.Period, typeof(Timeout));
		}

		[TestMethod]
		public void GameManager_AfterHalftime_ThenPeriodIsThirdQuarter() {
			var gm = new GameManagerBuilder()
				.WithElapsedSeconds(301)
				.WithQuarter(2)
				.Build();

			gm.Period.ElapsedTime();

			Assert.AreEqual(BreakType.HalfBreak, gm.Period.BreakType);
			Assert.AreEqual(gm.GameClock.Quarter, 3);
			Assert.IsInstanceOfType(gm.Period, typeof(Timeout));

			gm.GameClock.Clock.ElapsedSeconds = 181;
			gm.Period.ElapsedTime();

			Assert.AreEqual(BreakType.None, gm.Period.BreakType);
			Assert.AreEqual(gm.GameClock.Quarter, 3);
			Assert.IsInstanceOfType(gm.Period, typeof(Quarter));
		}

		[TestMethod]
		public void GameManager_AfterFourthQuarter_ThenGameIsOver() {
			var gm = new GameManagerBuilder()
				.WithElapsedSeconds(301)
				.WithQuarter(4)
				.Build();

			gm.Period.ElapsedTime();

			Assert.AreEqual(true, gm.GameClock.FinalBuzzer);
		}
	}
}
