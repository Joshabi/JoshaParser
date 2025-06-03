namespace JoshaParser.Data.Beatmap;

/// <summary> Beatmap Object in Time </summary>
public abstract class BeatObject
{
    /// <summary> Object timing in Beats </summary>
    public float B { get; set; } = 0f;
    /// <summary> Object timing in Milliseconds </summary>
    public float MS { get; set; } = 0f;
}

/// <summary> Beatmap Object with Position </summary>
public abstract class BeatGridObject : BeatObject
{
    /// <summary> Object Lane Index </summary>
    public int X { get; set; } = 0;
    /// <summary> Object Layer Index </summary>
    public int Y { get; set; } = 0;
}
