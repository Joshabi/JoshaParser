using JoshaParser.Data.Beatmap;
using JoshaParser.Data.Metadata;
using Newtonsoft.Json.Linq;

namespace JoshaParser.Parsers.Events;

/// <summary> Parser for basic beatmap events in V2 format </summary>
public static class BasicEventV2Parser
{
    public static BasicBeatmapEvent Deserialize(JToken eToken)
    {
        return new BasicBeatmapEvent
        {
            B = (float)(eToken["_time"] ?? 0),
            T = (int)(eToken["_type"] ?? 0),
            I = (int)(eToken["_value"] ?? 0),
            F = (float)(eToken["_floatValue"] ?? 0)
        };
    }

    public static JToken Serialize(BasicBeatmapEvent basicEvent, BeatmapRevision revision = BeatmapRevision.V250)
    {
        JObject result = new()
        {
            ["_time"] = basicEvent.B,
            ["_type"] = basicEvent.T,
            ["_value"] = basicEvent.I
        };
        if (revision >= BeatmapRevision.V250) result["_floatValue"] = basicEvent.F;
        return result;
    }
}

/// <summary> Parser for basic beatmap events in V3 format </summary>
public static class BasicEventV3Parser
{
    public static BasicBeatmapEvent Deserialize(JToken eToken)
    {
        return new BasicBeatmapEvent
        {
            B = (float)(eToken["b"] ?? 0),
            T = (int)(eToken["et"] ?? 0),
            I = (int)(eToken["i"] ?? 0),
            F = (int)(eToken["f"] ?? 0)
        };
    }

    public static JToken Serialize(BasicBeatmapEvent basicEvent)
    {
        return new JObject
        {
            ["b"] = basicEvent.B,
            ["et"] = basicEvent.T,
            ["i"] = basicEvent.I,
            ["f"] = basicEvent.F
        };
    }
}

/// <summary> Parser for basic beatmap events in V4 format </summary>
public static class BasicEventV4Parser
{
    public static BasicBeatmapEvent Deserialize(JToken eToken, JToken dToken)
    {
        BasicBeatmapEvent basicEvent = new()
        {
            B = (float)(eToken["b"] ?? 0)
        };

        int eventIndex = (int)(eToken["i"] ?? 0);
        JToken? data = dToken?[eventIndex];
        if (data is not null) {
            basicEvent.T = (int)(data["t"] ?? 0);
            basicEvent.I = (int)(data["i"] ?? 0);
            basicEvent.F = (int)(data["f"] ?? 0);
        }

        return basicEvent;
    }

    public static (JToken eObject, JToken dObject) Serialize(BasicBeatmapEvent basicEvent)
    {
        JObject eventJObj = new()
        {
            ["b"] = basicEvent.B
        };

        JObject dataJObj = new()
        {
            ["t"] = basicEvent.T,
            ["i"] = basicEvent.I,
            ["f"] = basicEvent.F
        };

        return (eventJObj, dataJObj);
    }
}