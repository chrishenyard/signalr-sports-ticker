using System.ComponentModel.DataAnnotations;

namespace SportsTicker.Settings;

public interface IGameSettings
{
    int HalfBreakTime { get; set; }
    int QuarterBreakTime { get; set; }
    int QuarterTime { get; set; }
    int TickerInterval { get; set; }
}

public class GameSettings : IGameSettings
{
    public const string Section = "GameSettings";

    [Range(1, 10)]
    public int TickerInterval { get; set; }

    [Range(300, 1000)]
    public int QuarterTime { get; set; }

    [Range(30, 300)]
    public int QuarterBreakTime { get; set; }

    [Range(30, 300)]
    public int HalfBreakTime { get; set; }
}
