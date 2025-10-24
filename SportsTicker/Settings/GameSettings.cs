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

    public int TickerInterval { get; set; }
    public int QuarterTime { get; set; }
    public int QuarterBreakTime { get; set; }
    public int HalfBreakTime { get; set; }
}
