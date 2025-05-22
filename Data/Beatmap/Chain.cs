namespace JoshaParser.Data.Beatmap;

/// <summary> Represents a parsed Chain object </summary>
public class Chain : Slider
{
    /// <summary> Segment Count </summary>
    public int SC { get; set; }
    /// <summary> Squish Factor </summary>
    public float SF { get; set; }
    /// <summary> Chain Data Metadata (V4) </summary>
    public int? CI { get; set; }

    public override string ToString()
    {
        return base.ToString() +
               $"\nSliceCount: {SC}, SquishFactor: {SF}" +
               $"{(CI.HasValue ? $", Chain Metadata Index: {CI}" : "")}";
    }
}