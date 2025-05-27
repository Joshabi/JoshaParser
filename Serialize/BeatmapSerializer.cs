using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JoshaParser.Data.Metadata;
using JoshaParser.Utils;
using System.Diagnostics;
using JoshaParser.Parsers;

namespace JoshaParser.Serialize;

/// <summary> Handles serialize and deserialize operations for Beatmap versions V2, V3, V4 </summary>
public class BeatmapSerializer : JsonConverter<DifficultyData>
{
    /// <summary> Handles reading difficulty.dat json </summary>
    public override DifficultyData? ReadJson(JsonReader reader, Type objectType, DifficultyData? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        DifficultyData data = new();
        JObject obj = JObject.Load(reader);
        data.RawJSON = obj.ToString(Formatting.Indented);

        JToken version = obj["version"] ?? obj["_version"] ?? "";
        if (string.IsNullOrEmpty(version.ToString())) return null;
        data.Version = version.ToString().ToBeatmapRevision();

        switch (data.Version)
        {
            case BeatmapRevision.Unknown:
                Debug.WriteLine("Unsupported Version"); return null;
            case var v when v < BeatmapRevision.V300:
                DifficultyV2Parser.Deserialize(obj, data);
                break;
            case var v when v is < BeatmapRevision.V400 and >= BeatmapRevision.V300:
                DifficultyV3Parser.Deserialize(obj, data);
                break;
            case var v when v is >= BeatmapRevision.V400:
                DifficultyV4Parser.Deserialize(obj, data);
                break;
        }

        return data;
    }

    /// <summary> Handles writing difficulty.dat json </summary>
    public override void WriteJson(JsonWriter writer, DifficultyData? value, JsonSerializer serializer)
    {
        if (value == null || value.Version >= BeatmapRevision.V400)
        {
            Console.WriteLine("Version is unsupported or provided data is incorrect");
            return;
        }

        JObject? output = value.Version switch
        {
            var v when v < BeatmapRevision.V300 => DifficultyV2Parser.Serialize(value),
            var v when v is < BeatmapRevision.V400 and >= BeatmapRevision.V300 => DifficultyV3Parser.Serialize(value),
            _ => []
        };

        SerializerUtils.FormatNumbers(output);
        output.WriteTo(writer);
    }
}
