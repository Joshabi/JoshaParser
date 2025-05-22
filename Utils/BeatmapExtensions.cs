using JoshaParser.Data.Metadata;

namespace JoshaParser.Utils;

/// <summary> Extensions class for generic BeatmapData </summary>
public static class BeatmapExtensions {

    /// <summary> Converts string to BeatmapRevision </summary>
    public static BeatmapRevision ToBeatmapRevision(this string revisionString)
    {
        return revisionString switch
        {
            "2.0.0" => BeatmapRevision.V200,
            "2.2.0" => BeatmapRevision.V220,
            "2.4.0" => BeatmapRevision.V240,
            "2.5.0" => BeatmapRevision.V250,
            "2.6.0" => BeatmapRevision.V260,
            "3.0.0" => BeatmapRevision.V300,
            "3.1.0" => BeatmapRevision.V310,
            "3.2.0" => BeatmapRevision.V320,
            "3.3.0" => BeatmapRevision.V330,
            "4.0.0" => BeatmapRevision.V400,
            "4.1.0" => BeatmapRevision.V410,
            _ => BeatmapRevision.Unknown,
        };
    }

    /// <summary> Converts BeatmapRevision to string </summary>
    public static string ToVersionString(this BeatmapRevision revision)
    {
        return revision switch
        {
            BeatmapRevision.V200 => "2.0.0",
            BeatmapRevision.V220 => "2.2.0",
            BeatmapRevision.V240 => "2.4.0",
            BeatmapRevision.V250 => "2.5.0",
            BeatmapRevision.V260 => "2.6.0",
            BeatmapRevision.V300 => "3.0.0",
            BeatmapRevision.V310 => "3.1.0",
            BeatmapRevision.V320 => "3.2.0",
            BeatmapRevision.V330 => "3.3.0",
            BeatmapRevision.V400 => "4.0.0",
            BeatmapRevision.V410 => "4.1.0",
            _ => "Unknown",
        };
    }

    /// <summary> Converts string to BeatmapRevision </summary>
    public static BeatmapInfoRevision ToBeatmapInfoRevision(this string revisionString)
    {
        return revisionString switch
        {
            "2.0.0" => BeatmapInfoRevision.V200,
            "2.1.0" => BeatmapInfoRevision.V210,
            "4.0.0" => BeatmapInfoRevision.V400,
            _ => BeatmapInfoRevision.Unknown,
        };
    }

    /// <summary> Converts BeatmapRevision to string </summary>
    public static string ToVersionString(this BeatmapInfoRevision revision)
    {
        return revision switch
        {
            BeatmapInfoRevision.V200 => "2.0.0",
            BeatmapInfoRevision.V210 => "2.2.0",
            BeatmapInfoRevision.V400 => "4.0.0",
            _ => "Unknown",
        };
    }
}
