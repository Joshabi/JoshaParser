namespace JoshaParser.Data.Beatmap;

/// <summary> Represents a parsed Bomb object </summary>
public class Bomb : BeatGridObject
{
    /// <summary> Rotation Lane Index (V4) </summary>
    public int? R { get; set; }
    /// <summary> Metadata Index (V4) </summary>
    public int? I { get; set; }

    public override string ToString()
    {
        return $"Beat: {B}, X: {X}, Y: {Y}" +
               $"{(R.HasValue ? $", Rotation Lane: {R}" : "")}" +
               $"{(I.HasValue ? $", Metadata Index: {I}" : "")}";
    }
}