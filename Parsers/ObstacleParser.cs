using JoshaParser.Data.Beatmap;
using JoshaParser.Data.Metadata;
using Newtonsoft.Json.Linq;

namespace JoshaParser.Parsers;

/// <summary> Used to parse V2 Obstacles </summary>
public static class ObstacleV2Parser
{
    public static Obstacle Deserialize(JToken oToken)
    {
        int y = (int)(oToken["_lineLayer"] ?? 0);
        int h = (int)(oToken["_height"] ?? 5);

        if (oToken["_lineLayer"] == null && oToken["_type"] != null) {
            (y, h) = (int)(oToken["_type"] ?? 0) switch
            {
                0 => (0, 5),
                1 => (2, 3),
                _ => (0, 5)
            };
        }

        return new Obstacle
        {
            B = (float)(oToken["_time"] ?? 0),
            D = (float)(oToken["_duration"] ?? 0),
            X = (int)(oToken["_lineIndex"] ?? 0),
            Y = y,
            W = (int)(oToken["_width"] ?? 1),
            H = h,
        };
    }

    public static JToken Serialize(Obstacle obstacle, BeatmapRevision version = BeatmapRevision.V260)
    {
        JObject obj = new()
        {
            ["_time"] = obstacle.B,
            ["_lineIndex"] = obstacle.X,
            ["_type"] = obstacle.Y == 0 && obstacle.H == 5 ? 0 : obstacle.Y == 2 && obstacle.H == 3 ? 1 : 2,
            ["_duration"] = obstacle.D
        };
        if (version is > BeatmapRevision.V250) obj["_lineLayer"] = obstacle.Y;
        obj["_width"] = obstacle.W;
        if (version is > BeatmapRevision.V250) obj["_height"] = obstacle.H;
        return obj;
    }
}

/// <summary> Used to parse V3 Obstacles </summary>
public static class ObstacleV3Parser
{
    public static Obstacle Deserialize(JToken oToken)
    {
        return new Obstacle
        {
            B = (float)(oToken["b"] ?? 0),
            D = (float)(oToken["d"] ?? 0),
            X = (int)(oToken["x"] ?? 0),
            Y = (int)(oToken["y"] ?? 0),
            W = (int)(oToken["w"] ?? 1),
            H = (int)(oToken["h"] ?? 5),
        };
    }

    public static JToken Serialize(Obstacle obstacle)
    {
        return new JObject
        {
            ["b"] = obstacle.B,
            ["d"] = obstacle.D,
            ["x"] = obstacle.X,
            ["y"] = obstacle.Y,
            ["w"] = obstacle.W,
            ["h"] = obstacle.H,
        };
    }
}

/// <summary> Used to parse V4 Obstacles </summary>
public static class ObstacleV4Parser
{
    public static Obstacle Deserialize(JToken oToken, JToken dToken)
    {
        Obstacle obstacle = new()
        {
            B = (float)(oToken["b"] ?? 0),
            R = (int)(oToken["r"] ?? 0),
            I = (int)(oToken["i"] ?? 0),
        };
        JToken? data = dToken[obstacle.I];
        if (data is not null) {
            obstacle.D = (float)(data["d"] ?? 0);
            obstacle.X = (int)(data["x"] ?? 0);
            obstacle.Y = (int)(data["y"] ?? 0);
            obstacle.W = (int)(data["w"] ?? 1);
            obstacle.H = (int)(data["h"] ?? 5);
        }
        return obstacle;
    }

    public static (JToken oObject, JToken dObject) Serialize(Obstacle obstacle)
    {
        JObject obstacleJObj = new()
        {
            ["b"] = obstacle.B,
            ["r"] = obstacle.R,
            ["i"] = obstacle.I,
        };
        JObject dataJObj = new()
        {
            ["d"] = obstacle.D,
            ["x"] = obstacle.X,
            ["y"] = obstacle.Y,
            ["w"] = obstacle.W,
            ["h"] = obstacle.H,
        };
        return (obstacleJObj, dataJObj);
    }
}