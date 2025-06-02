using Newtonsoft.Json;

namespace JoshaParser.Data.Beatmap;

/// <summary> Used for V3 BPM Events </summary>
public class BPMEvent
{
    [JsonProperty("b")] public float B { get; set; }
    [JsonProperty("m")] public float M { get; set; }

    public override string ToString() => $"Beat: {B}, Value: {M}";
}

/// <summary> Legacy V2 Event </summary>
public class LegacyEvent
{
    [JsonProperty("_time")] public float Beat { get; protected set; }
    [JsonProperty("_type")] public int Type { get; protected set; }
    [JsonProperty("_value")] public int Value { get; protected set; }
    [JsonProperty("_floatValue")] public float FloatValue { get; protected set; }

    public override string ToString() => $"Beat: {Beat}, Type: {Type}, Value: {Value}, FloatValue: {FloatValue}";
}