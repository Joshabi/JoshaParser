using JoshaParser.Data.Beatmap;
using JoshaParser.Data.Metadata;
using Newtonsoft.Json.Linq;

namespace JoshaParser.Utils;

public static class SerializerUtils
{
    public static void FormatNumbers(JToken token)
    {
        if (token.Type == JTokenType.Object) {
            foreach (JProperty property in ((JObject)token).Properties()) {
                FormatNumbers(property.Value);
            }
        } else if (token.Type == JTokenType.Array) {
            foreach (JToken item in (JArray)token) {
                FormatNumbers(item);
            }
        } else if (token.Type == JTokenType.Float) {
            double value = token.Value<double>();

            JValue replacement = value % 1 == 0 ? new JValue((long)value) : new JValue(Math.Round(value, 3));
            token.Replace(replacement);
        }
    }

    public static int AddOrGetIndex(Dictionary<string, int> dict, List<JObject> list, string jsonString)
    {
        if (dict.TryGetValue(jsonString, out int index))
            return index;

        JObject obj = JObject.Parse(jsonString);
        index = list.Count;
        list.Add(obj);
        dict[jsonString] = index;
        return index;
    }
}

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
