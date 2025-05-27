using JoshaParser.Data.Metadata;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JoshaParser.Utils;

namespace JoshaParser.Serialize;

/// <summary> Handles deserialization of Beatmap info.dat </summary>
public class BeatmapInfoSerializer : JsonConverter<SongInfo>
{
    /// <summary> Handles reading difficulty.dat json </summary>
    public override SongInfo? ReadJson(JsonReader reader, Type objectType, SongInfo? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        SongInfo data = new();
        JObject obj = JObject.Load(reader);
        data.RawJSON = obj.ToString(Formatting.Indented);

        JToken version = obj["version"] ?? obj["_version"] ?? "";
        if (string.IsNullOrEmpty(version.ToString())) return null;
        data.Version = version.ToString().ToBeatmapInfoRevision();

        switch (data.Version)
        {
            case BeatmapInfoRevision.V200 or BeatmapInfoRevision.V210:
                DeserializeV2(obj, ref data);
                break;
            case BeatmapInfoRevision.V400 or BeatmapInfoRevision.V401:
                DeserializeV4(obj, ref data);
                break;
            default: Console.WriteLine("Unsupported Version"); return null;
        }

        // CustomData: Contributors handling (To my knowledge not version specific?)
        JToken? customDataToken = obj["_customData"];
        if (customDataToken != null && customDataToken.Type == JTokenType.Object)
            if (customDataToken["_contributors"] is JArray contributorsArray)
                data.Contributors = contributorsArray.ToObject<List<ContributorInfo>>(serializer) ?? [];
        data.Contributors ??= [];

        return data;
    }

    /// <summary> Handles Deserializing V2 and V2.1 Info Data</summary>
    private static void DeserializeV2(JObject jObject, ref SongInfo data)
    {
        data.Version = jObject["_version"]?.ToString().ToBeatmapInfoRevision() ?? BeatmapInfoRevision.Unknown;
        data.SongName = jObject["_songName"]?.ToString() ?? "";
        data.SongSubName = jObject["_songSubName"]?.ToString() ?? "";
        data.SongArtist = jObject["_songAuthorName"]?.ToString() ?? "";
        data.Mapper = jObject["_levelAuthorName"]?.ToString() ?? "";
        data.Song = new()
        {
            SongFilename = jObject["_songFilename"]?.ToString() ?? "",
            BPM = (float)(jObject["_beatsPerMinute"] ?? 0),
            PreviewStartTime = (float)(jObject["_previewStartTime"] ?? 0),
            PreviewDuration = (float)(jObject["_previewDuration"] ?? 0)
        };
        data.CoverImageFilename = jObject["_coverImageFilename"]?.ToString() ?? "";
        data.EnvironmentNames = jObject["_environmentNames"]?.ToObject<List<string>>() ?? [];
        data.EnvironmentName = jObject["_environmentName"]?.ToString() ?? "";
        data.AllDirectionsEnvironmentName = jObject["_allDirectionsEnvironmentName"]?.ToString() ?? "";
        data.DifficultyBeatmaps = [];

        // Parse DifficultyBeatmaps
        List<JObject> difficultyBeatmapSets = jObject["_difficultyBeatmapSets"]?.ToObject<List<JObject>>() ?? [];
        foreach (JObject difficultyBeatmapSet in difficultyBeatmapSets)
        {
            string? characteristicName = difficultyBeatmapSet["_beatmapCharacteristicName"]?.ToString();
            List<JObject> difficultyBeatmaps = difficultyBeatmapSet["_difficultyBeatmaps"]?.ToObject<List<JObject>>() ?? [];
            foreach (JObject difficultyBeatmap in difficultyBeatmaps)
            {
                data.DifficultyBeatmaps.Add(new DifficultyInfo
                {
                    Characteristic = characteristicName ?? "",
                    Difficulty = difficultyBeatmap["_difficulty"]?.ToString() ?? "",
                    Rank = (BeatmapDifficultyRank)(int)(difficultyBeatmap["_difficultyRank"] ?? 9),
                    BeatmapDataFilename = difficultyBeatmap["_beatmapFilename"]?.ToString() ?? "",
                    NoteJumpMovementSpeed = (float)(difficultyBeatmap["_noteJumpMovementSpeed"] ?? 10),
                    NoteJumpStartBeatOffset = (float)(difficultyBeatmap["_noteJumpStartBeatOffset"] ?? 0),
                    BeatmapColorSchemeIdx = (int)(difficultyBeatmap["_beatmapColorSchemeIdx"] ?? 0),
                    EnvironmentNameIdx = (int)(difficultyBeatmap["_environmentNameIdx"] ?? 0),
                    DifficultyLabel = difficultyBeatmap["_customData"]?["_difficultyLabel"]?.ToString() ?? "",
                });
            }
        }
    }

    /// <summary> Handles Deserializing V4 and V4.0.1 Info Data</summary>
    private static void DeserializeV4(JObject jObject, ref SongInfo data)
    {
        List<DifficultyInfo> difficultyBeatmaps = jObject["difficultyBeatmaps"]?.ToObject<List<DifficultyInfo>>() ?? [];

        data.Version = jObject["version"]?.ToString().ToBeatmapInfoRevision() ?? BeatmapInfoRevision.Unknown;
        data.SongName = jObject["song"]?["title"]?.ToString() ?? "";
        data.SongSubName = jObject["song"]?["subTitle"]?.ToString() ?? "";
        data.SongArtist = jObject["song"]?["author"]?.ToString() ?? "";
        data.Song = new() { 
            SongFilename = jObject["audio"]?["songFilename"]?.ToString() ?? "",
            SongDuration = (float)(jObject["audio"]?["songDuration"] ?? 0),
            AudioDataFilename = jObject["audio"]?["audioDataFilename"]?.ToString() ?? "",
            BPM = (float)(jObject["audio"]?["bpm"] ?? 0),
            LUFS = (float)(jObject["audio"]?["lufs"] ?? 0),
            PreviewStartTime = (float)(jObject["audio"]?["previewStartTime"] ?? 0),
            PreviewDuration = (float)(jObject["audio"]?["previewDuration"] ?? 0),
            SongPreviewFilename = jObject["songPreviewFilename"]?.ToString() ?? ""
        };
        data.CoverImageFilename = jObject["coverImageFilename"]?.ToString() ?? "";
        data.EnvironmentNames = jObject["environmentNames"]?.ToObject<List<string>>() ?? [];
        data.DifficultyBeatmaps = difficultyBeatmaps;
        data.Mapper = data.DifficultyBeatmaps?.FirstOrDefault()?.BeatmapAuthors?.Mappers?.FirstOrDefault() ?? "";

        foreach (DifficultyInfo diff in data.DifficultyBeatmaps ?? [])
        {
            diff.Rank = Enum.TryParse(diff.Difficulty, true, out BeatmapDifficultyRank parsedRank)
                ? parsedRank
                : BeatmapDifficultyRank.ExpertPlus;
        }
    }

    /// <summary> [Unsupported] Writes back to the info.dat file </summary>
    public override void WriteJson(JsonWriter writer, SongInfo? value, JsonSerializer serializer)
    {
        Console.WriteLine("Serializing back to file is not supported");
        writer.WriteNull();
        return;
    }
}
