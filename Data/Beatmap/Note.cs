namespace JoshaParser.Data.Beatmap;

/// <summary> Represents a parsed Note object </summary>
public class Note : BeatGridObject
{
    /// <summary> Color </summary>
    public int C { get; set; } = 0;
    /// <summary> Cut Direction </summary>
    public CutDirection D { get; set; } = CutDirection.Any;
    /// <summary> Angle Offset (V3) - Angle applied counter-clockwise to the Cut Direction </summary>
    public int A { get; set; } = 0;
    /// <summary> Rotation Lane (V4) - Specific for 360 or 90 degree characteristic </summary>
    public int R { get; set; } = 0;

    public override string ToString() => $"Beat: {B}, X: {X}, Y: {Y}, Color: {C}, Direction: {D}";
}