using JoshaParser.Data.Beatmap;
using Newtonsoft.Json.Linq;

namespace JoshaParser.Parsers;

/// <summary> Used to parse V2 Bombs </summary>
public static class BombV2Parser
{
    public static Bomb Deserialize(JToken bToken)
    {
        return new Bomb
        {
            B = (float)(bToken["_time"] ?? 0),
            X = (int)(bToken["_lineIndex"] ?? 0),
            Y = (int)(bToken["_lineLayer"] ?? 0),
        };
    }

    public static JToken Serialize(Bomb bomb)
    {
        return new JObject
        {
            ["_time"] = bomb.B,
            ["_lineIndex"] = bomb.X,
            ["_lineLayer"] = bomb.Y,
            ["_type"] = 3,
            ["_cutDirection"] = 1
        };
    }
}

/// <summary> Used to parse V3 Bombs </summary>
public static class BombV3Parser
{
    public static Bomb Deserialize(JToken bToken)
    {
        return new Bomb
        {
            B = (float)(bToken["b"] ?? 0),
            X = (int)(bToken["x"] ?? 0),
            Y = (int)(bToken["y"] ?? 0)
        };
    }

    public static JToken Serialize(Bomb bomb)
    {
        return new JObject
        {
            ["b"] = bomb.B,
            ["x"] = bomb.X,
            ["y"] = bomb.Y
        };
    }
}

/// <summary> Used to parse V4 Bombs </summary>
public static class BombV4Parser
{
    public static Bomb Deserialize(JToken bToken, JToken dToken)
    {
        Bomb bomb = new()
        {
            B = (float)(bToken["b"] ?? 0),
            R = (int)(bToken["r"] ?? 0),
            I = (int)(bToken["i"] ?? 0),
        };
        JToken? data = dToken[bomb.I];
        if (data is not null)
        {
            bomb.X = (int)(data["x"] ?? 0);
            bomb.Y = (int)(data["y"] ?? 0);
        }
        return bomb;
    }

    public static (JToken bObject, JToken dObject) Serialize(Bomb note)
    {
        JObject bObject = new()
        {
            ["b"] = note.B,
            ["r"] = note.R,
            ["i"] = note.I
        };
        JObject dObject = new()
        {
            ["x"] = note.X,
            ["y"] = note.Y
        };
        return (bObject, dObject);
    }
}
