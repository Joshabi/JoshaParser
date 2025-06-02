namespace JoshaParser.Data.Beatmap;

/// <summary> Represents a parsed obstacle object </summary>
public class Obstacle : BeatGridObject
{
    /// <summary> Duration </summary>
    public float D { get; set; } = 1f;
    /// <summary> Width </summary>
    public int W { get; set; } = 1;
    /// <summary> Height </summary>
    public int H { get; set; } = 5;
    /// <summary> Rotation Lane (V4) - Specific for 360 or 90 degree characteristic </summary>
    public int R { get; set; } = 0;
    /// <summary> Type (V2) - Returns Type of wall (Depreciated beyond 2.6.0) </summary>
    public int T => Y == 0 && H == 5 ? 0 : Y == 2 && H == 3 ? 1 : 2;

    public override string ToString() => $"Beat: {B}, Duration: {D}, X: {X}, Y: {Y}, W: {W}, H: {H}, T: {T}";
}
