using JoshaParser.Data.Beatmap;
using Newtonsoft.Json.Linq;

namespace JoshaParser.Parsers.Events;

/// <summary> Parser for V2 Special Event Keyword Filters </summary>
public static class KeywordV2Parser
{
    public static SpecialEventKeywordFilters Deserialize(JToken filtersToken)
    {
        SpecialEventKeywordFilters filters = new();
        if (filtersToken["_keywords"] is JArray keywordsArray) {
            foreach (JToken keywordToken in keywordsArray) {
                string keyword = keywordToken["_keyword"]?.ToString() ?? string.Empty;
                JArray? eventsArray = keywordToken["_specialEvents"] as JArray;

                SpecialEventKeywordMapping mapping = new()
                {
                    Keyword = keyword,
                    Events = eventsArray?.ToObject<List<int>>() ?? []
                };

                filters.Keywords.Add(mapping);
            }
        }

        return filters;
    }

    public static JToken Serialize(SpecialEventKeywordFilters filters)
    {
        JArray keywordsArray = [];
        foreach (SpecialEventKeywordMapping mapping in filters.Keywords) {
            keywordsArray.Add(new JObject
            {
                ["_keyword"] = mapping.Keyword,
                ["_specialEvents"] = new JArray(mapping.Events)
            });
        }

        return new JObject
        {
            ["_keywords"] = keywordsArray
        };
    }
}

/// <summary> Parser for V3 Basic Event Types With Keywords </summary>
public static class KeywordV3Parser
{
    public static SpecialEventKeywordFilters Deserialize(JToken filtersToken)
    {
        SpecialEventKeywordFilters filters = new();
        if (filtersToken["d"] is JArray keywordsArray) {
            foreach (JToken keywordToken in keywordsArray) {
                string keyword = keywordToken["k"]?.ToString() ?? string.Empty;
                JArray? eventsArray = keywordToken["e"] as JArray;
                SpecialEventKeywordMapping mapping = new()
                {
                    Keyword = keyword,
                    Events = eventsArray?.ToObject<List<int>>() ?? []
                };

                filters.Keywords.Add(mapping);
            }
        }

        return filters;
    }

    public static JToken Serialize(SpecialEventKeywordFilters filters)
    {
        JArray keywordsArray = [];
        foreach (SpecialEventKeywordMapping mapping in filters.Keywords) {
            keywordsArray.Add(new JObject
            {
                ["k"] = mapping.Keyword,
                ["e"] = new JArray(mapping.Events)
            });
        }

        return new JObject
        {
            ["d"] = keywordsArray
        };
    }
}

/// <summary> Parser for V4 Basic Event Types With Keywords </summary>
public static class KeywordV4Parser
{
    // V4 uses the same format as V3
    public static SpecialEventKeywordFilters Deserialize(JToken filtersToken) => KeywordV3Parser.Deserialize(filtersToken);

    public static JToken Serialize(SpecialEventKeywordFilters filters) => KeywordV3Parser.Serialize(filters);
}