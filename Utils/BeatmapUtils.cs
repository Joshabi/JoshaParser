using JoshaParser.Data.Metadata;

namespace JoshaParser.Utils;

/// <summary> Utilities class for generic BeatmapData </summary>
public static class BeatmapUtils
{
    /// <summary> Calculates the Jump Distance (JD) based on BPM, NJS, and Offset </summary>
    public static float CalculateJD(float bpm, float njs, float njsoffset)
    {
        float hjd = 4;
        float bps = 60 / bpm;
        if (njs <= 0.01) njs = 10;
        while (njs * bps * hjd > 17.999) hjd /= 2;
        hjd += njsoffset;
        if (hjd < 0.25f) hjd = 0.25f;
        return njs * bps * hjd * 2;
    }

    /// <summary> Calculates the Reaction Time (RT) based on JD and NJS </summary>
    public static float CalculateReactionTime(float njs, float jd) => Math.Abs(njs) < 0.001f ? 0f : jd / (njs * 2) * 1000;
}

/// <summary> Extensions class for generic BeatmapData </summary>
public static class BeatmapExtensions
{
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
            "4.0.1" => BeatmapInfoRevision.V401,
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
            BeatmapInfoRevision.V401 => "4.0.1",
            _ => "Unknown",
        };
    }
}
