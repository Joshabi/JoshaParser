using JoshaParser.Data.Metadata;
using JoshaParser.Parsers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

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
    /// <summary>
    /// Computes SHA1 hash of info.dat combined with difficulty files in the order listed in info.dat
    /// </summary>
    /// <returns>SHA1 hash as hex string, or empty string if it fails</returns>
    public static string ComputeMapHash(this Beatmap beatmap) {
        try {
            if (string.IsNullOrEmpty(beatmap.SongData.MapPath))
                return string.Empty;

            string infoPath = Directory.GetFiles(beatmap.SongData.MapPath, "info.dat", SearchOption.TopDirectoryOnly)
                               .FirstOrDefault();
            if (infoPath == null)
                return string.Empty;

            using var sha1 = SHA1.Create();
            byte[] infoBytes = File.ReadAllBytes(infoPath);
            List<byte> combinedBytes = [.. infoBytes];

            // Add difficulty files in the order they appear in info.dat
            foreach (var difficulty in beatmap.SongData.DifficultyBeatmaps) {
                if (string.IsNullOrEmpty(difficulty.BeatmapDataFilename))
                    continue;

                string difficultyPath = Path.Combine(beatmap.SongData.MapPath, difficulty.BeatmapDataFilename);
                if (File.Exists(difficultyPath)) {
                    byte[] difficultyBytes = File.ReadAllBytes(difficultyPath);
                    combinedBytes.AddRange(difficultyBytes);
                }
            }

            // Compute hash of combined content
            byte[] hashBytes = sha1.ComputeHash([.. combinedBytes]);

            // Convert to hex string
            StringBuilder sb = new();
            foreach (byte b in hashBytes) {
                sb.Append(b.ToString("x2"));
            }

            string hash = sb.ToString();
            beatmap.SongData.MapHash = hash;
            return hash;
        }
        catch {
            return string.Empty;
        }
    }


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
