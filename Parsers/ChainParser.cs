using JoshaParser.Data.Beatmap;
using Newtonsoft.Json.Linq;

namespace JoshaParser.Parsers;

/// <summary> Used to parse V3 Chains </summary>
public static class ChainV3Parser
{
    public static Chain Deserialize(JToken cToken)
    {
        return new Chain
        {
            C = (int)(cToken["c"] ?? 0),
            B = (float)(cToken["b"] ?? 0),
            X = (int)(cToken["x"] ?? 0),
            Y = (int)(cToken["y"] ?? 0),
            D = (CutDirection)(int)(cToken["d"] ?? 0),
            TB = (float)(cToken["tb"] ?? 0),
            TX = (int)(cToken["tx"] ?? 0),
            TY = (int)(cToken["ty"] ?? 0),
            SC = (int)(cToken["sc"] ?? 0),
            SF = (float)(cToken["s"] ?? 1.0f)
        };
    }

    public static JToken Serialize(Chain chain)
    {
        return new JObject
        {
            ["c"] = chain.C,
            ["b"] = chain.B,
            ["x"] = chain.X,
            ["y"] = chain.Y,
            ["d"] = (int)chain.D,
            ["tb"] = chain.TB,
            ["tx"] = chain.TX,
            ["ty"] = chain.TY,
            ["sc"] = chain.SC,
            ["s"] = chain.SF
        };
    }
}

/// <summary> Used to parse V4 Chains </summary>
public static class ChainV4Parser
{
    public static Chain Deserialize(JToken cToken, JToken cMetaToken, JToken dToken)
    {
        Chain chain = new()
        {
            B = (float)(cToken["hb"] ?? 0),
            TB = (float)(cToken["tb"] ?? 0),
            HR = (float)(cToken["hr"] ?? 0),
            TR = (float)(cToken["tr"] ?? 0),
            I = (int)(cToken["i"] ?? 0),
            CI = (int)(cToken["ci"] ?? 0),
        };
        JToken? headData = dToken[chain.I];
        if (headData is not null) {
            chain.C = (int)(headData["c"] ?? 0);
            chain.X = (int)(headData["x"] ?? 0);
            chain.Y = (int)(headData["y"] ?? 0);
            chain.D = (CutDirection)(int)(headData["d"] ?? 0);
            chain.A = (int)(headData["a"] ?? 0);
        }
        JToken? metaData = cMetaToken[chain.CI];
        if (metaData is not null) {
            chain.TX = (int)(metaData["tx"] ?? 0);
            chain.TY = (int)(metaData["ty"] ?? 0);
            chain.SC = (int)(metaData["c"] ?? 0);
            chain.SF = (float)(metaData["s"] ?? 1.0f);
        }
        return chain;
    }

    public static (JToken cToken, JToken cMetaToken, JToken dToken) Serialize(Chain chain)
    {
        JObject cToken = new()
        {
            ["hb"] = chain.B,
            ["tb"] = chain.TB,
            ["hr"] = chain.HR,
            ["tr"] = chain.TR,
            ["i"] = chain.I,
            ["ci"] = chain.CI
        };
        JObject dToken = new()
        {
            ["x"] = chain.X,
            ["y"] = chain.Y,
            ["c"] = chain.C,
            ["d"] = (int)chain.D,
            ["a"] = chain.A
        };
        JObject cMetaToken = new()
        {
            ["tx"] = chain.TX,
            ["ty"] = chain.TY,
            ["c"] = chain.SC,
            ["s"] = chain.SF
        };
        return (cToken, cMetaToken, dToken);
    }
}