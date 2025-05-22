using JoshaParser.Data.Beatmap;
using Newtonsoft.Json.Linq;

namespace JoshaParser.Data.Metadata;

/// <summary> Parsed Difficulty Information </summary>
public class DifficultyData
{
    public BeatmapRevision Version { get; set; } = BeatmapRevision.Unknown;
    public List<BPMEvent> RawBPMEvents { get; set; } = [];
    public List<Note> Notes { get; set; } = [];
    public List<Bomb> Bombs { get; set; } = [];
    public List<Obstacle> Obstacles { get; set; } = [];
    public List<Arc> Arcs { get; set; } = [];
    public List<Chain> Chains { get; set; } = [];

    public JObject? OriginalJson { get; set; }

    public override string ToString()
    {
        return $"Difficulty Version: {Version}\n" +
               $"Notes: {Notes.Count}\n" +
               $"Bombs: {Bombs.Count}\n" +
               $"Obstacles: {Obstacles.Count}\n" +
               $"Arcs: {Arcs.Count}\n" +
               $"Chains: {Chains.Count}\n" +
               $"Raw BPM Events: {RawBPMEvents.Count}\n";
    }
}

/// <summary> Represents version of BeatmapData </summary>
public enum BeatmapRevision
{
    Unknown = 0,
    V200 = 1,
    V220 = 2,
    V240 = 3,
    V250 = 4,
    V260 = 5,
    V300 = 6,
    V310 = 7,
    V320 = 8,
    V330 = 9,
    V400 = 10,
    V410 = 11
}
