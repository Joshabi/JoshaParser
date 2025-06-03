using JoshaParser.Data.Beatmap;
using Newtonsoft.Json.Linq;

namespace JoshaParser.Parsers.Events;

/// <summary> Parser for waypoint events in V2 format </summary>
public static class WaypointEventV2Parser
{
    public static WaypointBeatmapEvent Deserialize(JToken wToken)
    {
        return new WaypointBeatmapEvent
        {
            B = (float)(wToken["_time"] ?? 0),
            X = (int)(wToken["_lineIndex"] ?? 0),
            Y = (int)(wToken["_lineLayer"] ?? 0),
            D = (int)(wToken["_offsetDirection"] ?? 0)
        };
    }

    public static JToken Serialize(WaypointBeatmapEvent waypoint)
    {
        return new JObject
        {
            ["_time"] = waypoint.B,
            ["_lineIndex"] = waypoint.X,
            ["_lineLayer"] = waypoint.Y,
            ["_offsetDirection"] = waypoint.D
        };
    }
}

/// <summary> Parser for waypoint events in V3 format </summary>
public static class WaypointEventV3Parser
{
    public static WaypointBeatmapEvent Deserialize(JToken wToken)
    {
        return new WaypointBeatmapEvent
        {
            B = (float)(wToken["b"] ?? 0),
            X = (int)(wToken["x"] ?? 0),
            Y = (int)(wToken["y"] ?? 0),
            D = (int)(wToken["d"] ?? 0)
        };
    }

    public static JToken Serialize(WaypointBeatmapEvent waypoint)
    {
        return new JObject
        {
            ["b"] = waypoint.B,
            ["x"] = waypoint.X,
            ["y"] = waypoint.Y,
            ["d"] = waypoint.D
        };
    }
}

/// <summary> Parser for waypoint events in V4 format </summary>
public static class WaypointEventV4Parser
{
    public static WaypointBeatmapEvent Deserialize(JToken wToken, JToken dToken)
    {
        WaypointBeatmapEvent waypoint = new()
        {
            B = (float)(wToken["b"] ?? 0)
        };

        int waypointIndex = (int)(wToken["i"] ?? 0);
        JToken? data = dToken?[waypointIndex];
        if (data is not null) {
            waypoint.X = (int)(data["x"] ?? 0);
            waypoint.Y = (int)(data["y"] ?? 0);
            waypoint.D = (int)(data["d"] ?? 0);
        }

        return waypoint;
    }

    public static (JToken wObject, JToken dObject) Serialize(WaypointBeatmapEvent waypoint)
    {
        JObject waypointJObj = new()
        {
            ["b"] = waypoint.B
        };

        JObject dataJObj = new()
        {
            ["x"] = waypoint.X,
            ["y"] = waypoint.Y,
            ["d"] = waypoint.D
        };

        return (waypointJObj, dataJObj);
    }
}