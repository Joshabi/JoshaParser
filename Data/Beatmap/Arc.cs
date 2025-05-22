namespace JoshaParser.Data.Beatmap;

/// <summary> Represents a parsed Arc object </summary>
public class Arc : Slider
{
    /// <summary> Head Control Point Length Multiplier </summary>
    public float MU { get; set; }
    /// <summary> Tail Control Point Length Multiplier </summary>
    public float TMU { get; set; }
    /// <summary> Mid-Anchor Mode </summary>
    public int M { get; set; }
    /// <summary> Tail Color </summary>
    public int TC { get; set; }
    /// <summary> Tail Cut Direction </summary>
    public CutDirection TD { get; set; }
    /// <summary> Tail Data Index (V4) </summary>
    public int? TI { get; set; }
    /// <summary> Arc Data Index (V4) </summary>
    public int? AI { get; set; }

    public override string ToString()
    {
        return base.ToString() +
               $"\nHeadLengthMult: {MU}, TailCutDirection: {TD}, TailLengthMult: {TMU}, MidAnchorMode: {M}" +
               $"{(TI.HasValue ? $", Tail Metadata Index: {TI}" : "")}" +
               $"{(AI.HasValue ? $", Arc Metadata Index: {AI}" : "")}";
    }
}