using JoshaParser.Data.Beatmap;
using JoshaParser.Data.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JoshaParser.Utils;

/// <summary> Extension functionality for BPM/Audio data </summary>
public static class BeatmapAudioDataExtensions
{
    /// <summary> Creates a BPMHandler out of this audio data </summary>
    public static BPMContext ToBPMContext(this AudioInfo audioData, float defaultBPM, float songTimeOffset)
    {
        float songFrequency = audioData.SongFrequency;
        List<BPMEvent> bpmChanges = [];

        foreach (BPMDataSegment segment in audioData.BPMData) {
            float durationSamples = segment.EI - segment.SI;
            float durationSeconds = durationSamples / songFrequency;
            float durationMinutes = durationSeconds / 60f;
            float beatCount = segment.EB - segment.SB;
            float bpm = beatCount / durationMinutes;

            bpmChanges.Add(new BPMEvent
            {
                B = segment.SB,
                M = bpm
            });
        }

        float initialBPM = bpmChanges.FirstOrDefault()?.M ?? defaultBPM;
        return BPMContext.CreateBPMContext(initialBPM, bpmChanges, songTimeOffset);
    }
}

/// <summary> JsonConverter that formats all numeric values in JObjects and JArrays to a specified number of decimal places </summary>
public class JTokenDecimalFormatter(int decimalPlaces = 2) : JsonConverter
{
    private readonly int _decimalPlaces = decimalPlaces;

    public override bool CanConvert(Type objectType) => typeof(JToken).IsAssignableFrom(objectType);

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) => JToken.ReadFrom(reader);

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is JToken token) {
            WriteJToken(writer, token);
        }
    }

    private void WriteJToken(JsonWriter writer, JToken token)
    {
        switch (token.Type) {
            case JTokenType.Object:
            writer.WriteStartObject();
            foreach (JProperty property in token.Children<JProperty>()) {
                writer.WritePropertyName(property.Name);
                WriteJToken(writer, property.Value);
            }
            writer.WriteEndObject();
            break;

            case JTokenType.Array:
            writer.WriteStartArray();
            foreach (JToken item in token.Children()) {
                WriteJToken(writer, item);
            }
            writer.WriteEndArray();
            break;

            case JTokenType.Float:
            double doubleValue = token.Value<double>();
            writer.WriteValue(Math.Round(doubleValue, _decimalPlaces));
            break;

            default:
            token.WriteTo(writer);
            break;
        }
    }
}