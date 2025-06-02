namespace JoshaParser.Data.Beatmap;

/// <summary> Represents a parsed Arc object </summary>
public class Arc : Slider
{
    /// <summary> Head Control Point Length Multiplier </summary>
    public float MU { get; set; } = 1.0f;
    /// <summary> Tail Control Point Length Multiplier </summary>
    public float TMU { get; set; } = 1.0f;
    /// <summary> Mid-Anchor Mode </summary>
    public int M { get; set; } = 0;
    /// <summary> Tail Color </summary>
    public int TC { get; set; } = 0;
    /// <summary> Tail Cut Direction </summary>
    public CutDirection TD { get; set; } = CutDirection.Down;

    public override string ToString() => base.ToString() + $"\nHeadLengthMult: {MU}, TailCutDirection: {TD}, TailLengthMult: {TMU}, MidAnchorMode: {M}";
}