using JoshaParser.Data.Beatmap;
using Newtonsoft.Json.Linq;

namespace JoshaParser.Parsers.Events;

/// <summary> Parser for color boost events in V2 format </summary>
public static class ColorBoostEventV2Parser
{
    public static ColorBoostBeatmapEvent Deserialize(JToken eToken)
    {
        return new ColorBoostBeatmapEvent
        {
            B = (float)(eToken["_time"] ?? 0),
            I = (int)(eToken["_value"] ?? 0)
        };
    }

    public static JToken Serialize(ColorBoostBeatmapEvent boostEvent)
    {
        return new JObject
        {
            ["_time"] = boostEvent.B,
            ["_type"] = 5,
            ["_value"] = boostEvent.I
        };
    }
}

/// <summary> Parser for color boost events in V3 format </summary>
public static class ColorBoostEventV3Parser
{
    public static ColorBoostBeatmapEvent Deserialize(JToken eToken)
    {
        return new ColorBoostBeatmapEvent
        {
            B = (float)(eToken["b"] ?? 0),
            I = (bool)(eToken["o"] ?? false) ? 1 : 0
        };
    }

    public static JToken Serialize(ColorBoostBeatmapEvent boostEvent)
    {
        return new JObject
        {
            ["b"] = boostEvent.B,
            ["o"] = boostEvent.I != 0
        };
    }
}

/// <summary> Parser for color boost events in V4 format </summary>
public static class ColorBoostEventV4Parser
{
    public static ColorBoostBeatmapEvent Deserialize(JToken eToken, JToken dToken)
    {
        ColorBoostBeatmapEvent boostEvent = new()
        {
            B = (float)(eToken["b"] ?? 0)
        };

        int eventIndex = (int)(eToken["i"] ?? 0);
        JToken? data = dToken?[eventIndex];
        if (data is not null)
            boostEvent.I = (int)(data["b"] ?? 0);

        return boostEvent;
    }

    public static (JToken eObject, JToken dObject) Serialize(ColorBoostBeatmapEvent boostEvent)
    {
        JObject eventJObj = new()
        {
            ["b"] = boostEvent.B
        };

        JObject dataJObj = new()
        {
            ["b"] = boostEvent.I
        };

        return (eventJObj, dataJObj);
    }
}