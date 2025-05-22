using JoshaParser.Data.Metadata;
using JoshaParser.Serialize;
using Newtonsoft.Json;

namespace JoshaParser.Parsers;

/// <summary> Responsible for loading a mapset </summary>
public static class BeatmapLoader
{
    /// <summary> Loads a map from a JSON string of the info.dat file. </summary>
    public static SongInfo? LoadMapFromDirectory(string folder)
    {
        string? infoPath = Directory.EnumerateFiles(folder, "*", SearchOption.TopDirectoryOnly)
            .FirstOrDefault(f => string.Equals(Path.GetFileName(f), "info.dat", StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrWhiteSpace(infoPath) || !File.Exists(infoPath)) return null;

        SongInfo? songInfo = Deserialize<SongInfo>(File.ReadAllText(infoPath), new BeatmapInfoSerializer());
        if (songInfo is null) return null;

        songInfo.MapPath = folder;
        LoadDifficulties(ref songInfo);
        return songInfo;
    }

    /// <summary> Loads a song info from a JSON string of the info.dat file. </summary>
    public static SongInfo LoadSongInfoFromString(string jsonString)
        => Deserialize<SongInfo>(jsonString, new BeatmapInfoSerializer()) ?? new SongInfo();

    /// <summary> Loads a difficulty from a JSON string of the difficulty.dat file. </summary>
    public static DifficultyData LoadDifficultyFromString(string jsonString)
        => Deserialize<DifficultyData>(jsonString, new BeatmapSerializer()) ?? new();

    /// <summary> Loads audio and BPM data from a JSON string of the audio.dat file. </summary>
    public static AudioInfo LoadAudioDataFromString(string jsonString)
        => Deserialize<AudioInfo>(jsonString, new BeatmapAudioInfoSerializer()) ?? new();

    /// <summary> Loads all difficulties for a given SongData object. </summary>
    public static void LoadDifficulties(ref SongInfo songData)
    {
        foreach (var diffInfo in songData.DifficultyBeatmaps)
        {
            string path = Path.Combine(songData.MapPath, diffInfo.BeatmapDataFilename);
            if (!File.Exists(path)) continue;

            string json = File.ReadAllText(path);
            diffInfo.DifficultyData = LoadDifficultyFromString(json);
        }
    }

    private static T? Deserialize<T>(string json, JsonConverter converter)
        => JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings {
            Converters = { converter }
        });
}
