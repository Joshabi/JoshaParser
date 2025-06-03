using Newtonsoft.Json;

namespace JoshaParser.Data.Beatmap;

/// <summary> V3 BPM Event </summary>
public class BPMEvent
{
    [JsonProperty("b")] public float B { get; set; }
    [JsonProperty("m")] public float M { get; set; }

    public override string ToString() => $"Beat: {B}, Value: {M}";
}

/// <summary> Represents a parsed Basic Event </summary>
public class BasicBeatmapEvent : BeatObject
{
    /// <summary> Event Types </summary>
    public int T { get; set; } = 1;
    /// <summary> Value </summary>
    public int I { get; set; } = 0;
    /// <summary> Float Value </summary>
    public float F { get; set; } = 0;

    public override string ToString() => $"Beat: {B}, Type: {T}, Value: {I}, Float Value: {F}";
}

/// <summary> Represents a parsed Color Boost Event </summary>
public class ColorBoostBeatmapEvent : BeatObject
{
    /// <summary> Boost Value </summary>
    public int I { get; set; } = 0;

    public override string ToString() => $"Beat: {B}, On: {I}";
}

/// <summary> Represents a parsed Waypoint Event </summary>
/// <remarks> These are used for the TinyTAN figures </remarks>
public class WaypointBeatmapEvent : BeatObject
{
    /// <summary> Line Index / X position </summary>
    public int X { get; set; } = 0;
    /// <summary> Layer Index / Y position </summary>
    public int Y { get; set; } = 0;
    /// <summary> Direction </summary>
    public int D { get; set; } = 0;

    public override string ToString() => $"Beat: {B}, X: {X}, Y: {Y}, Direction: {D}";
}

/// <summary> Represents a keyword-to-events mapping for special events </summary>
public class SpecialEventKeywordMapping
{
    /// <summary> The keyword string </summary>
    public string Keyword { get; set; } = string.Empty;
    /// <summary> The special event types associated with this keyword </summary>
    public List<int> Events { get; set; } = [];

    public override string ToString() => $"Keyword: {Keyword}, Events: {string.Join(", ", Events)}";
}

/// <summary> Represents a collection of special event keyword mappings </summary>
public class SpecialEventKeywordFilters
{
    /// <summary> All keyword mappings in this filter collection </summary>
    public List<SpecialEventKeywordMapping> Keywords { get; set; } = [];

    public override string ToString() => $"Keyword Mappings: {Keywords.Count}";
}