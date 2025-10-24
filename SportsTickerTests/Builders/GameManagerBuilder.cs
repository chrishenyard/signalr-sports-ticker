using SportsTicker.Services;
using SportsTicker.Settings;

namespace SportsTickerTests.Builders
{


    public class GameManagerBuilder
    {
        private int quarter = 1;
        private int quarterTime = 300;
        private int quarterBreakTime = 60;
        private int halfBreakTime = 180;
        private int elapsedSeconds = 0;

        public GameManager Build()
        {
            var gameSettings = new GameSettings
            {
                QuarterTime = quarterTime,
                QuarterBreakTime = quarterBreakTime,
                HalfBreakTime = halfBreakTime
            };

            var gameManager = new GameManager(gameSettings);
            gameManager.GameClock.Quarter = quarter;
            gameManager.GameClock.Clock.Restart();
            gameManager.GameClock.Clock.ElapsedSeconds = elapsedSeconds;

            return gameManager;
        }

        public GameManagerBuilder WithQuarter(int quarter)
        {
            this.quarter = quarter;
            return this;
        }

        public GameManagerBuilder WithQuarterTime(int quarterTime)
        {
            this.quarterTime = quarterTime;
            return this;
        }

        public GameManagerBuilder WithQuarterBreakTime(int quarterBreakTime)
        {
            this.quarterBreakTime = quarterBreakTime;
            return this;
        }

        public GameManagerBuilder WithHalfBreakTime(int halfBreakTime)
        {
            this.halfBreakTime = halfBreakTime;
            return this;
        }

        public GameManagerBuilder WithElapsedSeconds(int elapsedSeconds)
        {
            this.elapsedSeconds = elapsedSeconds;
            return this;
        }
    }
}
