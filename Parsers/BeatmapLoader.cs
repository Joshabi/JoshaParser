using JoshaParser.Data.Metadata;
using JoshaParser.Serialize;
using Newtonsoft.Json;
using System.Diagnostics;

namespace JoshaParser.Parsers;

/// <summary> Functionalities for loading Beat Saber map data </summary>
public static class BeatmapLoader
{
    /// <summary> Loads a map from a JSON string of the info.dat file. </summary>
    public static SongInfo? LoadMapFromDirectory(string folder)
    {
        try {
            if (!Directory.Exists(folder)) {
                Trace.WriteLine($"Directory does not exist: {folder}");
                return null;
            }

            string? infoPath = Directory.EnumerateFiles(folder, "*", SearchOption.TopDirectoryOnly)
                .FirstOrDefault(f => string.Equals(Path.GetFileName(f), "info.dat", StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrWhiteSpace(infoPath) || !File.Exists(infoPath)) {
                Trace.WriteLine($"Info.dat not found in directory: {folder}");
                return null;
            }

            SongInfo? songInfo = Deserialize<SongInfo>(File.ReadAllText(infoPath), new BeatmapInfoSerializer());
            if (songInfo is null) {
                Trace.WriteLine($"Failed to deserialize info.dat: {infoPath}");
                return null;
            }

            songInfo.MapPath = folder;
            LoadDifficulties(songInfo);
            return songInfo;
        } catch (Exception ex) {
            Trace.WriteLine($"Error loading map from directory {folder}: {ex.Message}");
            return null;
        }
    }

    /// <summary> Loads a song info from a JSON string of the info.dat file. </summary>
    public static SongInfo? LoadSongInfoFromString(string jsonString)
        => Deserialize<SongInfo>(jsonString, new BeatmapInfoSerializer());

    /// <summary> Loads a difficulty from a JSON string of the difficulty.dat file. </summary>
    public static DifficultyData? LoadDifficultyFromString(string jsonString)
        => Deserialize<DifficultyData>(jsonString, new BeatmapSerializer());

    /// <summary> Loads audio and BPM data from a JSON string of the audio.dat file. </summary>
    public static AudioInfo? LoadAudioDataFromString(string jsonString)
        => Deserialize<AudioInfo>(jsonString, new BeatmapAudioInfoSerializer());

    /// <summary> Loads all difficulties for a given SongData object. </summary>
    public static void LoadDifficulties(SongInfo songData)
    {
        foreach (DifficultyInfo diffInfo in songData.DifficultyBeatmaps) {
            try {
                string path = Path.Combine(songData.MapPath, diffInfo.BeatmapDataFilename);
                if (!File.Exists(path)) {
                    Trace.WriteLine($"Could not find difficulty file at: {path}");
                    continue;
                }

                string json = File.ReadAllText(path);
                DifficultyData? d = LoadDifficultyFromString(json);
                if (d == null) {
                    Trace.WriteLine($"Failed to load difficulty data from: {path}");
                    continue;
                }

                diffInfo.DifficultyData = d;
            } catch (Exception ex) {
                Trace.WriteLine($"Error loading difficulty from {diffInfo.BeatmapDataFilename}: {ex.Message}");
            }
        }
    }

    private static T? Deserialize<T>(string json, JsonConverter converter)
    {
        try {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { Converters = { converter } });
        } catch (JsonException ex) {
            Trace.WriteLine($"JSON Deserialization error: {ex.Message}");
            return default;
        }
    }

    /// <summary> Saves a map to the specified directory. </summary>
    public static bool SaveMapToDirectory(SongInfo songInfo, string folder)
    {
        try {
            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }

            string infoPath = Path.Combine(folder, "info.dat");
            File.WriteAllText(infoPath, SaveSongInfoToString(songInfo));

            // Update map path for saving difficulties
            songInfo.MapPath = folder;
            SaveDifficulties(songInfo);

            return true;
        } catch (Exception ex) {
            Trace.WriteLine($"Error saving map: {ex.Message}");
            return false;
        }
    }

    /// <summary> Converts a SongInfo object to a JSON string. </summary>
    public static string SaveSongInfoToString(SongInfo songInfo)
        => Serialize(songInfo, new BeatmapInfoSerializer());

    /// <summary> Converts a DifficultyData object to a JSON string. </summary>
    public static string SaveDifficultyToString(DifficultyData difficultyData)
        => Serialize(difficultyData, new BeatmapSerializer());

    /// <summary> Converts an AudioInfo object to a JSON string. </summary>
    public static string SaveAudioDataToString(AudioInfo audioInfo)
        => Serialize(audioInfo, new BeatmapAudioInfoSerializer());

    /// <summary> Saves all difficulties for a given SongInfo object. </summary>
    public static void SaveDifficulties(SongInfo songInfo)
    {
        try {
            foreach (DifficultyInfo diffInfo in songInfo.DifficultyBeatmaps) {
                if (diffInfo.DifficultyData == null) continue;

                string path = Path.Combine(songInfo.MapPath, diffInfo.BeatmapDataFilename);
                string json = SaveDifficultyToString(diffInfo.DifficultyData);
                File.WriteAllText(path, json);
            }
        } catch (Exception ex) {
            Trace.WriteLine($"Error saving difficulties: {ex.Message}");
            return;
        }
    }

    /// <summary> Serializes an object to a JSON string with formatting and converters. </summary>
    private static string Serialize<T>(T obj, JsonConverter converter)
    {
        try {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings { Converters = { converter } });
        } catch (JsonException ex) {
            Trace.WriteLine($"Error serializing {typeof(T).Name}: {ex.Message}");
            return string.Empty;
        }
    }
}
