namespace JoshaParser.Data.Beatmap;

/// <summary> Represents a parsed obstacle object </summary>
public class Obstacle : BeatGridObject
{
    /// <summary> Duration </summary>
    public float D { get; set; }
    /// <summary> Width </summary>
    public int W { get; set; }
    /// <summary> Height </summary>
    public int H { get; set; }
    /// <summary> Rotation Lane (V3) </summary>
    public int? R { get; set; }
    /// <summary> Metadata Index (V4) </summary>
    public int? I { get; set; }
    /// <summary> Returns Type of wall (Depreciated beyond 2.5.0) </summary>
    public int T { get => Y == 0 && H == 5 ? 0 : Y == 2 && H == 3 ? 1 : 2; } // T - Type (V2)

    public override string ToString()
    {
        return $"Beat: {B}, Duration: {D}, X: {X}, Y: {Y}, W: {W}, H: {H}, T: {T}" +
               $"{(R.HasValue ? $", Rotation Lane: {R}" : "")}" +
               $"{(I.HasValue ? $", Metadata Index: {I}" : "")}";
    }
}
