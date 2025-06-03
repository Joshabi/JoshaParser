namespace JoshaParser.Data.Beatmap;

/// <summary> Represents a parsed slider object </summary>
public class Slider : Note
{
    /// <summary> Tail time in Beats </summary>
    public float TB { get; set; } = 0f;
    /// <summary> Tail time in Milliseconds </summary>
    public float TMS { get; set; } = 0f;
    /// <summary> Tail X </summary>
    public int TX { get; set; } = 0;
    /// <summary> Tail Y </summary>
    public int TY { get; set; } = 0;
    /// <summary> Head Rotation Lane (V4) - Specific for 360 or 90 degree characteristic </summary>
    public int HR { get; set; } = 0;
    /// <summary> Tail Rotation Lane (V4) - Specific for 360 or 90 degree characteristic </summary>
    public int TR { get; set; } = 0;

    public override string ToString() => $"Beat: {B}, X: {X}, Y: {Y}, Color: {C}, Direction: {D}, Angle Offset: {A}";
}