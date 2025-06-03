namespace JoshaParser.Data.Beatmap;

/// <summary> Represents a parsed Chain object </summary>
public class Chain : Slider
{
    /// <summary> Segment Count </summary>
    public int SC { get; set; } = 1;
    /// <summary> Squish Factor </summary>
    public float SF { get; set; } = 0.5f;

    public override string ToString() => $"\nSliceCount: {SC}, SquishFactor: {SF}";
}