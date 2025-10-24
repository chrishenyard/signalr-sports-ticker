using SportsTicker.Services;
using SportsTickerTests.Builders;

namespace SportsTickerTests
{
    [TestClass]
    public class SportsTickerTests
    {
        [TestMethod]
        public void GameManager_BeginningOfGame_ThenPeriodIsFirstQuarter()
        {
            var gm = new GameManagerBuilder()
                .WithElapsedSeconds(60)
                .Build();

            gm.Period.ElapsedTime();

            Assert.AreEqual(BreakType.None, gm.Period.BreakType);
            Assert.AreEqual(1, gm.GameClock.Quarter);
            Assert.IsInstanceOfType<Quarter>(gm.Period);
        }

        [TestMethod]
        public void GameManager_AfterFirstQuater_ThenPeriodIsTimeout()
        {
            var gm = new GameManagerBuilder()
                .WithElapsedSeconds(301)
                .WithQuarter(1)
                .Build();

            gm.Period.ElapsedTime();

            Assert.AreEqual(BreakType.QuarterBreak, gm.Period.BreakType);
            Assert.AreEqual(2, gm.GameClock.Quarter);
            Assert.IsInstanceOfType(gm.Period, typeof(SportsTicker.Services.Timeout));
        }

        [TestMethod]
        public void GameManager_AfterSecondQuater_ThenPeriodIsHalftime()
        {
            var gm = new GameManagerBuilder()
                .WithElapsedSeconds(301)
                .WithQuarter(2)
                .Build();

            gm.Period.ElapsedTime();

            Assert.AreEqual(BreakType.HalfBreak, gm.Period.BreakType);
            Assert.AreEqual(3, gm.GameClock.Quarter);
            Assert.IsInstanceOfType(gm.Period, typeof(SportsTicker.Services.Timeout));
        }

        [TestMethod]
        public void GameManager_AfterHalftime_ThenPeriodIsThirdQuarter()
        {
            var gm = new GameManagerBuilder()
                .WithElapsedSeconds(301)
                .WithQuarter(2)
                .Build();

            gm.Period.ElapsedTime();

            Assert.AreEqual(BreakType.HalfBreak, gm.Period.BreakType);
            Assert.AreEqual(3, gm.GameClock.Quarter);
            Assert.IsInstanceOfType(gm.Period, typeof(SportsTicker.Services.Timeout));

            gm.GameClock.Clock.ElapsedSeconds = 181;
            gm.Period.ElapsedTime();

            Assert.AreEqual(BreakType.None, gm.Period.BreakType);
            Assert.AreEqual(3, gm.GameClock.Quarter);
            Assert.IsInstanceOfType(gm.Period, typeof(Quarter));
        }

        [TestMethod]
        public void GameManager_AfterFourthQuarter_ThenGameIsOver()
        {
            var gm = new GameManagerBuilder()
                .WithElapsedSeconds(301)
                .WithQuarter(4)
                .Build();

            gm.Period.ElapsedTime();

            Assert.IsTrue(gm.GameClock.FinalBuzzer);
        }
    }
}
