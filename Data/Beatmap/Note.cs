namespace JoshaParser.Data.Beatmap;

/// <summary> Represents a parsed Note object </summary>
public class Note : BeatGridObject
{
    /// <summary> Color </summary>
    public int C { get; set; }
    /// <summary> Cut Direction </summary>
    public CutDirection D { get; set; }
    /// <summary> Angle Offset (V3) </summary>
    public float A { get; set; }
    /// <summary> Rotation Lane (V3) </summary>
    public int? R { get; set; }
    /// <summary> Metadata Index (V4) </summary>
    public int? I { get; set; }

    public override string ToString()
    {
        return $"Beat: {B}, X: {X}, Y: {Y}, Color: {C}, Direction: {D}" +
               $", Angle Offset: {A}" +
               $"{(R.HasValue ? $", Rotation Lane: {R}" : "")}" +
               $"{(I.HasValue ? $", Metadata Index: {I}" : "")}";
    }
}