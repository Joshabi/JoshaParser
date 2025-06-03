using JoshaParser.Data.Beatmap;
using JoshaParser.Data.Metadata;
using JoshaParser.Parsers.Events;
using JoshaParser.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JoshaParser.Parsers;

/// <summary> DifficultyData parser for V2 </summary>
public static class DifficultyV2Parser
{
    /// <summary> Deserialize DifficultyData from JObject </summary>
    public static void Deserialize(JObject obj, DifficultyData data)
    {
        JArray notes = obj["_notes"] as JArray ?? [];
        JArray obstacles = obj["_obstacles"] as JArray ?? [];
        JArray events = obj["_events"] as JArray ?? [];
        JArray waypoints = obj["_waypoints"] as JArray ?? [];
        JObject keywordFilters = obj["_specialEventsKeywordFilters"] as JObject ?? [];

        data.Lightshow.BasicEvents.AddRange(events.Where(v2event => (int)(v2event["_type"] ?? 0) != 100 && (int)(v2event["_type"] ?? 0) != 5).Select(BasicEventV2Parser.Deserialize));
        data.Lightshow.ColorBoostEvents.AddRange(events.Where(v2event => (int)(v2event["_type"] ?? 0) == 5).Select(ColorBoostEventV2Parser.Deserialize));
        data.Lightshow.Waypoints.AddRange(waypoints.Select(WaypointEventV2Parser.Deserialize));
        data.Lightshow.EventKeywordFilters = KeywordV2Parser.Deserialize(keywordFilters);
        data.RawBPMEvents.AddRange(events.Where(v2event => (int)(v2event["_type"] ?? 0) == 100).Select(v2event => new BPMEvent { B = (float)(v2event["_time"] ?? 0), M = (float)(v2event["_floatValue"] ?? 0) }));
        data.Bombs.AddRange(notes.Where(noteToken => (int)(noteToken["_type"] ?? 0) == 3).Select(BombV2Parser.Deserialize));
        data.Notes.AddRange(notes.Where(noteToken => (int)(noteToken["_type"] ?? 0) != 3).Select(NoteV2Parser.Deserialize));
        data.Obstacles.AddRange(obstacles.Select(ObstacleV2Parser.Deserialize));

        if (data.Version is BeatmapRevision.V260) {
            JToken sliders = obj["_sliders"] ?? new JArray();
            data.Arcs.AddRange(sliders.Select(ArcV2Parser.Deserialize));
        }
    }

    /// <summary> Serializes patchable map data from DifficultyData </summary>
    public static JObject Serialize(DifficultyData data)
    {
        JObject obj = JObject.Parse(data.RawJSON ?? "{}");
        obj["_version"] = data.Version.ToVersionString();

        IOrderedEnumerable<BeatObject> orderedObjects = data.Bombs
            .Cast<BeatObject>()
            .OrderBy(obj => obj.B);
        obj["_notes"] = new JArray(orderedObjects.Select(o => o is Bomb b ? BombV2Parser.Serialize(b) : NoteV2Parser.Serialize((Note)o)));
        obj["_obstacles"] = new JArray(data.Obstacles.Select(o => ObstacleV2Parser.Serialize(o, data.Version)));

        if (data.Version >= BeatmapRevision.V260) obj["_sliders"] = new JArray(data.Arcs.Select(ArcV2Parser.Serialize));

        JArray eventsArray = [];
        foreach (BasicBeatmapEvent basicEvent in data.Lightshow.BasicEvents)
            eventsArray.Add(BasicEventV2Parser.Serialize(basicEvent, data.Version));

        foreach (ColorBoostBeatmapEvent boostEvent in data.Lightshow.ColorBoostEvents)
            eventsArray.Add(ColorBoostEventV2Parser.Serialize(boostEvent));

        foreach (BPMEvent bpmEvent in data.RawBPMEvents) {
            JObject eventObj = new()
            {
                ["_time"] = bpmEvent.B,
                ["_type"] = 100,
                ["_value"] = 0,
                ["_floatValue"] = bpmEvent.M
            };
            eventsArray.Add(eventObj);
        }

        // Sort all events by time
        JArray sortedEvents = new(eventsArray.OrderBy(e => (float?)(e["_time"] ?? 0)).Select(e => e));
        if (sortedEvents.Any()) obj["_events"] = sortedEvents;
        if (data.Version >= BeatmapRevision.V220) obj["_waypoints"] = new JArray(data.Lightshow.Waypoints.Select(WaypointEventV2Parser.Serialize));
        if (data.Version >= BeatmapRevision.V240) obj["_specialEventsKeywordFilters"] = KeywordV2Parser.Serialize(data.Lightshow.EventKeywordFilters);
        return obj;
    }
}

/// <summary> DifficultyData parser for V3 </summary>
public static class DifficultyV3Parser
{
    /// <summary> Deserialize DifficultyData from JObject </summary>
    public static void Deserialize(JObject obj, DifficultyData data)
    {
        JArray notes = obj["colorNotes"] as JArray ?? [];
        JArray bombs = obj["bombNotes"] as JArray ?? [];
        JArray arcs = obj["sliders"] as JArray ?? [];
        JArray chains = obj["burstSliders"] as JArray ?? [];
        JArray obstacles = obj["obstacles"] as JArray ?? [];
        JArray basicEvents = obj["basicBeatmapEvents"] as JArray ?? [];
        JArray colorBoostEvents = obj["colorBoostBeatmapEvents"] as JArray ?? [];
        JArray waypoints = obj["waypoints"] as JArray ?? [];
        JObject keywordFilters = obj["basicEventTypesWithKeywords"] as JObject ?? [];
        List<BPMEvent> bpmChanges = obj["bpmEvents"]?.ToObject<List<BPMEvent>>() ?? [];

        data.Notes.AddRange(notes.Select(NoteV3Parser.Deserialize));
        data.Bombs.AddRange(bombs.Select(BombV3Parser.Deserialize));
        data.Arcs.AddRange(arcs.Select(ArcV3Parser.Deserialize));
        data.Chains.AddRange(chains.Select(ChainV3Parser.Deserialize));
        data.Obstacles.AddRange(obstacles.Select(ObstacleV3Parser.Deserialize));
        data.Lightshow.BasicEvents.AddRange(basicEvents.Select(BasicEventV3Parser.Deserialize));
        data.Lightshow.ColorBoostEvents.AddRange(colorBoostEvents.Select(ColorBoostEventV3Parser.Deserialize));
        data.Lightshow.Waypoints.AddRange(waypoints.Select(WaypointEventV3Parser.Deserialize));
        data.Lightshow.EventKeywordFilters = KeywordV3Parser.Deserialize(keywordFilters);
        data.RawBPMEvents.AddRange(bpmChanges);
    }

    /// <summary> Serializes patchable map data from DifficultyData </summary>
    public static JObject Serialize(DifficultyData data)
    {
        JObject obj = JObject.Parse(data.RawJSON ?? "{}");
        obj["version"] = data.Version.ToVersionString();
        obj["bpmEvents"] = JArray.FromObject(data.RawBPMEvents);
        obj["colorNotes"] = new JArray(data.Notes.Select(NoteV3Parser.Serialize));
        obj["bombNotes"] = new JArray(data.Bombs.Select(BombV3Parser.Serialize));
        obj["sliders"] = new JArray(data.Arcs.Select(ArcV3Parser.Serialize));
        obj["burstSliders"] = new JArray(data.Chains.Select(ChainV3Parser.Serialize));
        obj["obstacles"] = new JArray(data.Obstacles.Select(ObstacleV3Parser.Serialize));
        obj["basicBeatmapEvents"] = new JArray(data.Lightshow.BasicEvents.Select(BasicEventV3Parser.Serialize));
        obj["colorBoostBeatmapEvents"] = new JArray(data.Lightshow.ColorBoostEvents.Select(ColorBoostEventV3Parser.Serialize));
        obj["waypoints"] = new JArray(data.Lightshow.Waypoints.Select(WaypointEventV3Parser.Serialize));
        obj["basicEventTypesWithKeywords"] = KeywordV3Parser.Serialize(data.Lightshow.EventKeywordFilters);
        return obj;
    }
}

/// <summary> DifficultyData parser for V4 </summary>
/// <remarks> This parser is mostly only functional for deserializing at present as I need to think the best re-serialization approach. </remarks>
public static class DifficultyV4Parser
{
    /// <summary> Deserialize DifficultyData from JObject </summary>
    public static void Deserialize(JObject obj, DifficultyData data)
    {
        JArray notes = obj["colorNotes"] as JArray ?? [];
        JArray notesData = obj["colorNotesData"] as JArray ?? [];
        JArray bombs = obj["bombNotes"] as JArray ?? [];
        JArray bombsData = obj["bombNotesData"] as JArray ?? [];
        JArray arcs = obj["arcs"] as JArray ?? [];
        JArray arcsData = obj["arcsData"] as JArray ?? [];
        JArray chains = obj["chains"] as JArray ?? [];
        JArray chainsData = obj["chainsData"] as JArray ?? [];
        JArray obstacles = obj["obstacles"] as JArray ?? [];
        JArray obstaclesData = obj["obstaclesData"] as JArray ?? [];

        // Light and event data comes from the lightshow file, not the difficulty file
        // TO DO: Figure out the smoothest way to handle this.
        // Also spawn rotations are missing

        data.Notes.AddRange(notes.Select(n => NoteV4Parser.Deserialize(n, notesData)));
        data.Bombs.AddRange(bombs.Select(n => BombV4Parser.Deserialize(n, bombsData)));
        data.Arcs.AddRange(arcs.Select(n => ArcV4Parser.Deserialize(n, arcsData, notesData)));
        data.Chains.AddRange(chains.Select(n => ChainV4Parser.Deserialize(n, chainsData, notesData)));
        data.Obstacles.AddRange(obstacles.Select(n => ObstacleV4Parser.Deserialize(n, obstaclesData)));
    }

    /// <summary> Serializes patchable map data from DifficultyData </summary>
    public static JObject Serialize(DifficultyData data)
    {
        JObject obj = JObject.Parse(data.RawJSON ?? "{}");
        obj["version"] = data.Version.ToVersionString();

        JArray colorNotes = [];
        JArray colorNotesData = [];
        Dictionary<string, int> colorNotesDataMap = [];

        JArray bombNotes = [];
        JArray bombNotesData = [];
        Dictionary<string, int> bombNotesDataMap = [];

        JArray obstaclesArr = [];
        JArray obstaclesData = [];
        Dictionary<string, int> obstaclesDataMap = [];

        JArray arcsArr = [];
        JArray arcsData = [];
        Dictionary<string, int> arcsDataMap = [];

        JArray chainsArr = [];
        JArray chainsData = [];
        Dictionary<string, int> chainsDataMap = [];

        // Notes
        foreach (Note note in data.Notes) {
            (JToken? noteToken, JToken? dataToken) = NoteV4Parser.Serialize(note);
            string key = dataToken.ToString(Formatting.None);
            if (!colorNotesDataMap.TryGetValue(key, out int index)) {
                index = colorNotesData.Count;
                colorNotesData.Add(dataToken);
                colorNotesDataMap[key] = index;
            }
            noteToken["i"] = index;
            colorNotes.Add(noteToken);
        }

        // Bombs
        foreach (Bomb bomb in data.Bombs) {
            (JToken? bombToken, JToken? dataToken) = BombV4Parser.Serialize(bomb);
            string key = dataToken.ToString(Formatting.None);
            if (!bombNotesDataMap.TryGetValue(key, out int index)) {
                index = bombNotesData.Count;
                bombNotesData.Add(dataToken);
                bombNotesDataMap[key] = index;
            }
            bombToken["i"] = index;
            bombNotes.Add(bombToken);
        }

        // Obstacles
        foreach (Obstacle obs in data.Obstacles) {
            (JToken? obsToken, JToken? dataToken) = ObstacleV4Parser.Serialize(obs);
            string key = dataToken.ToString(Formatting.None);
            if (!obstaclesDataMap.TryGetValue(key, out int index)) {
                index = obstaclesData.Count;
                obstaclesData.Add(dataToken);
                obstaclesDataMap[key] = index;
            }
            obsToken["i"] = index;
            obstaclesArr.Add(obsToken);
        }

        // Arcs
        foreach (Arc arc in data.Arcs) {
            (JToken? arcToken, JToken? metaToken, JToken? headToken, JToken? tailToken) = ArcV4Parser.Serialize(arc);

            string headKey = headToken.ToString(Formatting.None);
            if (!colorNotesDataMap.TryGetValue(headKey, out int headIndex)) {
                headIndex = colorNotesData.Count;
                colorNotesData.Add(headToken);
                colorNotesDataMap[headKey] = headIndex;
            }

            string tailKey = tailToken.ToString(Formatting.None);
            if (!colorNotesDataMap.TryGetValue(tailKey, out int tailIndex)) {
                tailIndex = colorNotesData.Count;
                colorNotesData.Add(tailToken);
                colorNotesDataMap[tailKey] = tailIndex;
            }

            string metaKey = metaToken.ToString(Formatting.None);
            if (!arcsDataMap.TryGetValue(metaKey, out int metaIndex)) {
                metaIndex = arcsData.Count;
                arcsData.Add(metaToken);
                arcsDataMap[metaKey] = metaIndex;
            }

            arcToken["hi"] = headIndex;
            arcToken["ti"] = tailIndex;
            arcToken["ai"] = metaIndex;
            arcsArr.Add(arcToken);
        }

        // Chains
        foreach (Chain chain in data.Chains) {
            (JToken? chainToken, JToken? metaToken, JToken? dataToken) = ChainV4Parser.Serialize(chain);

            string dataKey = dataToken.ToString(Formatting.None);
            if (!colorNotesDataMap.TryGetValue(dataKey, out int dataIndex)) {
                dataIndex = colorNotesData.Count;
                colorNotesData.Add(dataToken);
                colorNotesDataMap[dataKey] = dataIndex;
            }

            string metaKey = metaToken.ToString(Formatting.None);
            if (!chainsDataMap.TryGetValue(metaKey, out int metaIndex)) {
                metaIndex = chainsData.Count;
                chainsData.Add(metaToken);
                chainsDataMap[metaKey] = metaIndex;
            }

            chainToken["i"] = dataIndex;
            chainToken["ci"] = metaIndex;
            chainsArr.Add(chainToken);
        }

        // Final write to output object
        obj["colorNotes"] = colorNotes;
        obj["colorNotesData"] = colorNotesData;
        obj["bombNotes"] = bombNotes;
        obj["bombNotesData"] = bombNotesData;
        obj["obstacles"] = obstaclesArr;
        obj["obstaclesData"] = obstaclesData;
        obj["arcs"] = arcsArr;
        obj["arcsData"] = arcsData;
        obj["chains"] = chainsArr;
        obj["chainsData"] = chainsData;

        return obj;
    }
}