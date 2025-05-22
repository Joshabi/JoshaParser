namespace JoshaParser.Data.Beatmap;

/// <summary> Represents a parsed slider object </summary>
public class Slider : Note
{
    /// <summary> Tail time in Beats </summary>
    public float TB { get; set; }
    /// <summary> Tail time in Milliseconds </summary>
    public float TMS { get; set; }
    /// <summary> Tail X </summary>
    public int TX { get; set; }
    /// <summary> Tail Y </summary>
    public int TY { get; set; }
    /// <summary> Head Rotation Lane (V3) </summary>
    public float? HR { get; set; }
    /// <summary> Tail Rotation Lane (V3) </summary>
    public float? TR { get; set; }

    public override string ToString()
    {
        return $"Beat: {B}, X: {X}, Y: {Y}, Color: {C}, Direction: {D}" +
               $", Angle Offset: {A}" +
               $"{(R.HasValue ? $", Rotation Lane: {R}" : "")}" +
               $"{(I.HasValue ? $", Metadata Index: {I}" : "")}";
    }
}