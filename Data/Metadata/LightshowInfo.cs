using JoshaParser.Data.Beatmap;

namespace JoshaParser.Data.Metadata;

/// <summary> Parsed Lightshow and Event Information </summary>
public class LightshowInfo
{
    public List<BasicBeatmapEvent> BasicEvents { get; set; } = [];
    public List<ColorBoostBeatmapEvent> ColorBoostEvents { get; set; } = [];
    public List<WaypointBeatmapEvent> Waypoints { get; set; } = [];
    public SpecialEventKeywordFilters EventKeywordFilters { get; set; } = new();
    //public List<LightColorEventBoxGroup> LightColorGroups { get; set; } = [];
    //public List<LightRotationEventBoxGroup> LightRotationGroups { get; set; } = [];
    //public List<LightTranslationEventBoxGroup> LightTranslationGroups { get; set; } = [];
    //public List<VFXEventBoxGroup> VFXGroups { get; set; } = [];
    public bool UseNormalEventsAsCompatibleEvents { get; set; } = true;

    public override string ToString() =>
        $"Lightshow Info:\n" +
        $"Basic Events: {BasicEvents.Count}\n" +
        $"Color Boost Events: {ColorBoostEvents.Count}\n" +
        $"Waypoints: {Waypoints.Count}\n" +
        $"Event Keyword Filters: {EventKeywordFilters}\n" +
        $"Use Normal Events as Compatible Events: {UseNormalEventsAsCompatibleEvents}";
}