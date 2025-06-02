using JoshaParser.Data.Beatmap;
using Newtonsoft.Json.Linq;

namespace JoshaParser.Parsers;

/// <summary> Used to parse V2 Arcs </summary>
public static class ArcV2Parser
{
    public static Arc Deserialize(JToken aToken)
    {
        return new Arc
        {
            C = (int)(aToken["_colorType"] ?? 0),
            B = (float)(aToken["_headTime"] ?? 0),
            X = (int)(aToken["_headLineIndex"] ?? 0),
            Y = (int)(aToken["_headLineLayer"] ?? 0),
            D = (CutDirection)(int)(aToken["_headCutDirection"] ?? 0),
            MU = (float)(aToken["_headControlPointLengthMultiplier"] ?? 0),
            TB = (float)(aToken["_tailTime"] ?? 0),
            TX = (int)(aToken["_tailLineIndex"] ?? 0),
            TY = (int)(aToken["_tailLineLayer"] ?? 0),
            TC = (int)(aToken["_colorType"] ?? 0),
            TD = (CutDirection)(int)(aToken["_tailCutDirection"] ?? 0),
            TMU = (float)(aToken["_tailControlPointLengthMultiplier"] ?? 0),
            M = (int)(aToken["_sliderMidAnchorMode"] ?? 0)
        };
    }

    public static JToken Serialize(Arc arc)
    {
        return new JObject
        {
            ["_colorType"] = arc.C,
            ["_headTime"] = arc.B,
            ["_headLineIndex"] = arc.X,
            ["_headLineLayer"] = arc.Y,
            ["_headCutDirection"] = (int)arc.D,
            ["_headControlPointLengthMultiplier"] = arc.MU,
            ["_tailTime"] = arc.TB,
            ["_tailLineIndex"] = arc.TX,
            ["_tailLineLayer"] = arc.TY,
            ["_tailCutDirection"] = (int)arc.TD,
            ["_tailControlPointLengthMultiplier"] = arc.TMU,
            ["_sliderMidAnchorMode"] = arc.M
        };
    }
}

/// <summary> Used to parse V3 Arcs </summary>
public static class ArcV3Parser
{
    public static Arc Deserialize(JToken aToken)
    {
        return new Arc
        {
            C = (int)(aToken["c"] ?? 0),
            B = (float)(aToken["b"] ?? 0),
            X = (int)(aToken["x"] ?? 0),
            Y = (int)(aToken["y"] ?? 0),
            D = (CutDirection)(int)(aToken["d"] ?? 0),
            MU = (float)(aToken["mu"] ?? 0),
            TB = (float)(aToken["tb"] ?? 0),
            TX = (int)(aToken["tx"] ?? 0),
            TY = (int)(aToken["ty"] ?? 0),
            TC = (int)(aToken["c"] ?? 0),
            TD = (CutDirection)(int)(aToken["tc"] ?? 0),
            TMU = (float)(aToken["tmu"] ?? 0),
            M = (int)(aToken["m"] ?? 0)
        };
    }

    public static JToken Serialize(Arc arc)
    {
        return new JObject
        {
            ["c"] = arc.C,
            ["b"] = arc.B,
            ["x"] = arc.X,
            ["y"] = arc.Y,
            ["d"] = (int)arc.D,
            ["mu"] = arc.MU,
            ["tb"] = arc.TB,
            ["tx"] = arc.TX,
            ["ty"] = arc.TY,
            ["tc"] = (int)arc.TD,
            ["tmu"] = arc.TMU,
            ["m"] = arc.M
        };
    }
}

/// <summary> Used to parse V4 Arcs </summary>
public static class ArcV4Parser
{
    public static Arc Deserialize(JToken aToken, JToken aMetaToken, JToken dToken)
    {
        Arc arc = new()
        {
            B = (float)(aToken["hb"] ?? 0),
            TB = (float)(aToken["tb"] ?? 0),
            HR = (float)(aToken["hr"] ?? 0),
            TR = (float)(aToken["tr"] ?? 0),
            I = (int)(aToken["hi"] ?? 0),
            TI = (int)(aToken["ti"] ?? 0),
            AI = (int)(aToken["ai"] ?? 0)
        };
        JToken? headData = dToken[arc.I];
        if (headData is not null) {
            arc.C = (int)(headData["c"] ?? 0);
            arc.X = (int)(headData["x"] ?? 0);
            arc.Y = (int)(headData["y"] ?? 0);
            arc.D = (CutDirection)(int)(headData["d"] ?? 0);
            arc.A = (int)(headData["a"] ?? 0);
        }
        JToken? tailData = dToken[arc.TI];
        if (tailData is not null) {
            arc.TX = (int)(tailData["x"] ?? 0);
            arc.TY = (int)(tailData["y"] ?? 0);
            arc.TC = (int)(tailData["c"] ?? 0);
            arc.TD = (CutDirection)(int)(tailData["d"] ?? 0);
        }
        JToken? metaData = aMetaToken[arc.AI];
        if (metaData is not null) {
            arc.MU = (float)(metaData["m"] ?? 0);
            arc.TMU = (float)(metaData["tm"] ?? 0);
            arc.M = (int)(metaData["a"] ?? 0);
        }
        return arc;
    }

    public static (JToken aToken, JToken aMetaToken, JToken headToken, JToken tailToken) Serialize(Arc arc)
    {
        JObject aToken = new()
        {
            ["hb"] = arc.B,
            ["tb"] = arc.TB,
            ["hr"] = arc.HR,
            ["tr"] = arc.TR,
            ["hi"] = arc.I,
            ["ti"] = arc.TI,
            ["ai"] = arc.AI
        };
        JObject headData = new()
        {
            ["x"] = arc.X,
            ["y"] = arc.Y,
            ["c"] = arc.C,
            ["d"] = (int)arc.D,
            ["a"] = arc.A
        };
        JObject tailData = new()
        {
            ["x"] = arc.TX,
            ["y"] = arc.TY,
            ["c"] = arc.TC,
            ["d"] = (int)arc.TD,
            ["a"] = arc.A,
        };
        JObject metaData = new()
        {
            ["m"] = arc.MU,
            ["tm"] = arc.TMU,
            ["a"] = arc.M
        };
        return (aToken, metaData, headData, tailData);
    }
}