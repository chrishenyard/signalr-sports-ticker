using Microsoft.Extensions.Options;
using SportsTicker.Settings;
using System.Diagnostics;

namespace SportsTicker.Services
{

    public class Watch
    {
        private readonly Stopwatch sw;
        private long? elapsedSeconds;

        public Watch()
        {
            sw = new Stopwatch();
        }

        public Watch(long elapsedSeconds) : this()
        {
            this.elapsedSeconds = elapsedSeconds;
        }

        public long ElapsedSeconds
        {
            get { return elapsedSeconds ?? sw.ElapsedMilliseconds / 1000; }
            set { elapsedSeconds = value; }
        }

        public void Start()
        {
            sw.Start();
        }

        public void Restart()
        {
            sw.Restart();
        }
    }

    public class GameClock
    {
        public int TickerInterval { get; set; } = 0;
        public int Quarter { get; set; } = 1;
        public int QuarterTime { get; set; } = 0;
        public int QuarterBreakTime { get; set; } = 0;
        public int HalfBreakTime { get; set; } = 0;
        public bool Timeout { get; set; }
        public bool FinalBuzzer { get; set; }
        public bool InProgress { get; set; }
        public Watch Clock { get; set; }

        public int TimeRemaining
        {
            get
            {
                if (Timeout)
                {
                    var breakTime = (PeriodBreakType == BreakType.QuarterBreak) ? QuarterBreakTime : HalfBreakTime;
                    var elapsed = Clock.ElapsedSeconds;
                    return (int)(breakTime - elapsed);
                }
                else
                {
                    var elapsed = Clock.ElapsedSeconds;
                    return (int)(QuarterTime - elapsed);
                }
            }
        }

        public BreakType PeriodBreakType { get; set; } = BreakType.None;
    }

    public enum BreakType
    {
        None,
        QuarterBreak,
        HalfBreak
    }

    public abstract class Period
    {
        public GameManager GameManager { get; set; }
        public BreakType BreakType { get; set; }

        public abstract void ElapsedTime();
    }

    public class Timeout : Period
    {
        public Timeout(GameManager gameManager)
        {
            GameManager = gameManager;
            GameManager.GameClock.Timeout = true;
        }

        public override void ElapsedTime()
        {
            var elapsedTime = GameManager.GameClock.Clock.ElapsedSeconds;
            var breakTime = BreakType == BreakType.QuarterBreak ?
                GameManager.GameClock.QuarterBreakTime :
                GameManager.GameClock.HalfBreakTime;
            var gameClock = GameManager.GameClock;

            if (elapsedTime > breakTime)
            {
                GameManager.Period = new Quarter(GameManager);
                GameManager.Period.BreakType = BreakType.None;
                gameClock.Clock.Restart();
            }
        }
    }

    public class Quarter : Period
    {
        public Quarter(GameManager gameManager)
        {
            GameManager = gameManager;
            GameManager.GameClock.Timeout = false;
        }

        public override void ElapsedTime()
        {
            var elapsedTime = GameManager.GameClock.Clock.ElapsedSeconds;
            var quarterTime = GameManager.GameClock.QuarterTime;
            var gameClock = GameManager.GameClock;

            if (elapsedTime >= quarterTime)
            {
                if (gameClock.Quarter == 4)
                {
                    gameClock.FinalBuzzer = true;
                    gameClock.InProgress = false;
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

    public interface IGameManager
    {
        GameClock GameClock { get; set; }
        Period Period { get; set; }
    }

    public class GameManager : IGameManager
    {
        public GameClock GameClock { get; set; }
        public Period Period { get; set; }

        public GameManager(IOptions<GameSettings> gameSettings)
        {
            var settings = gameSettings.Value;

            GameClock = new GameClock
            {
                Quarter = 1,
                TickerInterval = settings.TickerInterval,
                QuarterTime = settings.QuarterTime,
                QuarterBreakTime = settings.QuarterBreakTime,
                HalfBreakTime = settings.HalfBreakTime,
                Clock = new Watch()
            };

            Period = new Quarter(this);
        }

        public GameManager(IGameSettings gameSettings)
        {
            GameClock = new GameClock
            {
                Quarter = 1,
                TickerInterval = gameSettings.TickerInterval,
                QuarterTime = gameSettings.QuarterTime,
                QuarterBreakTime = gameSettings.QuarterBreakTime,
                HalfBreakTime = gameSettings.HalfBreakTime,
                Clock = new Watch()
            };

            Period = new Quarter(this);
        }

        private GameManager() { }
    }
}
