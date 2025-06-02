namespace JoshaParser.Data.Beatmap;

/// <summary> Represents a parsed Bomb object </summary>
public class Bomb : BeatGridObject
{
    /// <summary> Rotation Lane (V4) - Specific for 360 or 90 degree characteristic </summary>
    public int R { get; set; } = 0;

    public override string ToString() => $"Beat: {B}, X: {X}, Y: {Y}";
}