using JoshaParser.Data.Beatmap;
using Newtonsoft.Json.Linq;

namespace JoshaParser.Parsers;

/// <summary> Used to parse V2 Notes </summary>
public static class NoteV2Parser
{
    public static Note Deserialize(JToken nToken)
    {
        return new Note
        {
            B = (float)(nToken["_time"] ?? 0),
            X = (int)(nToken["_lineIndex"] ?? 0),
            Y = (int)(nToken["_lineLayer"] ?? 0),
            C = (int)(nToken["_type"] ?? 0),
            D = (CutDirection)(int)(nToken["_cutDirection"] ?? 0),
        };
    }

    public static JToken Serialize(Note note)
    {
        return new JObject
        {
            ["_time"] = note.B,
            ["_lineIndex"] = note.X,
            ["_lineLayer"] = note.Y,
            ["_type"] = note.C,
            ["_cutDirection"] = (int)note.D,
        };
    }
}

/// <summary> Used to parse V3 Notes </summary>
public static class NoteV3Parser
{
    public static Note Deserialize(JToken nToken)
    {
        return new Note
        {
            B = (float)(nToken["b"] ?? 0),
            X = (int)(nToken["x"] ?? 0),
            Y = (int)(nToken["y"] ?? 0),
            C = (int)(nToken["c"] ?? 0),
            D = (CutDirection)(int)(nToken["d"] ?? 0),
            A = (float)(nToken["a"] ?? 0),
        };
    }

    public static JToken Serialize(Note note)
    {
        return new JObject
        {
            ["b"] = note.B,
            ["x"] = note.X,
            ["y"] = note.Y,
            ["c"] = note.C,
            ["d"] = (int)note.D,
            ["a"] = note.A,
        };
    }
}

/// <summary> Used to parse V4 Notes </summary>
public static class NoteV4Parser
{
    public static Note Deserialize(JToken nToken, JToken dToken)
    {
        Note note = new()
        {
            B = (float)(nToken["b"] ?? 0),
            R = (int)(nToken["r"] ?? 0),
            I = (int)(nToken["i"] ?? 0),
        };
        JToken? data = dToken[note.I];
        if (data is not null)
        {
            note.X = (int)(data["x"] ?? 0);
            note.Y = (int)(data["y"] ?? 0);
            note.C = (int)(data["c"] ?? 0);
            note.D = (CutDirection)(int)(data["d"] ?? 0);
            note.A = (float)(data["a"] ?? 0);
        }
        return note;
    }

    public static (JToken nObject, JToken dObject) Serialize(Note note)
    {
        JObject noteJObj = new()
        {
            ["b"] = note.B,
            ["r"] = note.R,
            ["i"] = note.I,
        };
        JObject dataJObj = new()
        {
            ["x"] = note.X,
            ["y"] = note.Y,
            ["c"] = note.C,
            ["d"] = (int)note.D,
            ["a"] = note.A,
        };
        return (noteJObj, dataJObj);
    }
}