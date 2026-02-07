using JoshaParser.Data.Beatmap;
using JoshaParser.Data.Metadata;
using JoshaParser.Serialize;
using JoshaParser.Utils;
using Newtonsoft.Json;
using System.Diagnostics;

namespace JoshaParser.Parsers;

/// <summary> Represents a Beat Saber map with metadata and cache of loaded difficulties </summary>
public class Beatmap(SongInfo metadata, AudioInfo? audioData = null, bool retainRawJSON = true)
{
    public SongInfo SongData { get; } = metadata;
    public AudioInfo? AudioData { get; } = audioData;
    private readonly Dictionary<DifficultyInfo, DifficultyData> _cache = [];
    private readonly bool _retainRawJSON = retainRawJSON;

    /// <summary> Lazy-loaded difficulty data for a specific difficulty </summary>
    public DifficultyData? FetchDifficulty(DifficultyInfo difficultyInfo)
    {
        if (_cache.TryGetValue(difficultyInfo, out DifficultyData data))
            return data;

        string path = Path.Combine(SongData.MapPath, difficultyInfo.BeatmapDataFilename);
        if (!File.Exists(path)) return null;
        DifficultyData? loaded = BeatmapLoader.LoadDifficultyFromFile(path, _retainRawJSON);
        if (loaded != null) {
            BPMContext context = AudioData?.ToBPMContext(SongData.Song.BPM, SongData.SongTimeOffset) ?? BPMContext.CreateBPMContext(SongData.Song.BPM, loaded.RawBPMEvents, SongData.SongTimeOffset);
            loaded.CalculateBeatmapMS(context);
            _cache[difficultyInfo] = loaded;
        }
        return loaded;
    }

    /// <summary> Loads all difficulties into the cache. </summary>
    public void FetchAllDifficulties()
    {
        foreach (DifficultyInfo difficulty in SongData.DifficultyBeatmaps)
            FetchDifficulty(difficulty);
    }

    /// <summary> Clears all cached JSON strings. </summary>
    public void ClearRawJSON()
    {
        SongData.RawJSON = null;
        foreach (var difficulty in _cache.Values)
            difficulty.RawJSON = null;
    }

    public IReadOnlyDictionary<DifficultyInfo, DifficultyData> GetCachedDifficulties() => _cache;
    public void ClearCache() => _cache.Clear();
}

/// <summary> Configuration options for loading Beat Saber maps </summary>
public class BeatmapLoaderConfig
{
    public bool LoadAllDifficulties { get; set; } = true;
    public bool LoadLightshowData { get; set; } = true;
    /// <summary> If true, retains the raw JSON string in memory for re-serialization or accessing unloaded properties. Set to false to reduce memory usage. </summary>
    public bool RetainRawJSON { get; set; } = true;
}

/// <summary> Functionalities for loading and saving Beatmaps </summary>
public static class BeatmapLoader
{
    /// <summary> Loads a map from a JSON string of the info.dat file. </summary>
    public static Beatmap? LoadMapFromDirectory(string folder, BeatmapLoaderConfig? config = null)
    {
        config ??= new BeatmapLoaderConfig();
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

            SongInfo? songInfo = DeserializeFromFile<SongInfo>(infoPath, new BeatmapInfoSerializer(config.RetainRawJSON));
            if (songInfo is null) {
                Trace.WriteLine($"Failed to deserialize info.dat: {infoPath}");
                return null;
            }

            songInfo.MapPath = folder;
            string audioPath = Path.Combine(folder, songInfo.Song.AudioDataFilename);
            AudioInfo? audioInfo = File.Exists(audioPath)
                ? DeserializeFromFile<AudioInfo>(audioPath, new BeatmapAudioInfoSerializer())
                : null;
            Beatmap map = new(songInfo, audioInfo, config.RetainRawJSON);

            if (config.LoadAllDifficulties)
                map.FetchAllDifficulties();

            map.ComputeMapHash();

            return map;
        } catch (Exception ex) {
            Trace.WriteLine($"Error loading map from directory {folder}: {ex.Message}");
            return null;
        }
    }

    /// <summary> Loads a song info from a JSON string of the info.dat file. </summary>
    public static SongInfo? LoadSongInfoFromString(string jsonString, bool retainRawJSON = true)
        => Deserialize<SongInfo>(jsonString, new BeatmapInfoSerializer(retainRawJSON));

    /// <summary> Loads a difficulty from a JSON string of the difficulty.dat file. </summary>
    public static DifficultyData? LoadDifficultyFromString(string jsonString, bool retainRawJSON = true)
        => Deserialize<DifficultyData>(jsonString, new BeatmapSerializer(retainRawJSON));

    /// <summary> Loads a difficulty directly from a file using streaming deserialization. </summary>
    public static DifficultyData? LoadDifficultyFromFile(string filePath, bool retainRawJSON = true)
        => DeserializeFromFile<DifficultyData>(filePath, new BeatmapSerializer(retainRawJSON));

    /// <summary> Loads audio and BPM data from a JSON string of the audio.dat file. </summary>
    public static AudioInfo? LoadAudioDataFromString(string jsonString)
        => Deserialize<AudioInfo>(jsonString, new BeatmapAudioInfoSerializer());

    private static T? Deserialize<T>(string json, JsonConverter converter)
    {
        try {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { Converters = { converter } });
        } catch (JsonException ex) {
            Trace.WriteLine($"JSON Deserialization error: {ex.Message}");
            return default;
        }
    }

    /// <summary> Deserializes directly from a file stream to reduce memory allocations. </summary>
    private static T? DeserializeFromFile<T>(string filePath, JsonConverter converter)
    {
        try {
            using var fileStream = File.OpenRead(filePath);
            using var streamReader = new StreamReader(fileStream);
            using var jsonReader = new JsonTextReader(streamReader);
            var serializer = new JsonSerializer();
            serializer.Converters.Add(converter);
            return serializer.Deserialize<T>(jsonReader);
        } catch (JsonException ex) {
            Trace.WriteLine($"JSON Deserialization error: {ex.Message}");
            return default;
        }
    }

    /// <summary> Saves a map to the specified directory. </summary>
    public static bool SaveMapToDirectory(Beatmap beatmap, string folder)
    {
        try {
            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }

            string infoPath = Path.Combine(folder, "Info.dat");
            File.WriteAllText(infoPath, SaveSongInfoToString(beatmap.SongData));

            foreach (KeyValuePair<DifficultyInfo, DifficultyData> difficulty in beatmap.GetCachedDifficulties()) {
                string path = Path.Combine(folder, difficulty.Key.BeatmapDataFilename);
                string json = SaveDifficultyToString(difficulty.Value);
                File.WriteAllText(path, json);
            }

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
