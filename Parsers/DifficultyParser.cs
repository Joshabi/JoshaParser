using JoshaParser.Data.Beatmap;
using JoshaParser.Data.Metadata;
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
        JToken notes = obj["_notes"] ?? new JArray();
        JToken obstacles = obj["_obstacles"] ?? new JArray();
        List<LegacyEvent> events = obj["_events"]?.ToObject<List<LegacyEvent>>() ?? [];  // For old style BPM changes

        data.Bombs.AddRange(notes.Where(noteToken => (int)(noteToken["_type"] ?? 0) == 3).Select(BombV2Parser.Deserialize));
        data.Notes.AddRange(notes.Where(noteToken => (int)(noteToken["_type"] ?? 0) != 3).Select(NoteV2Parser.Deserialize));
        data.Obstacles.AddRange(obstacles.Select(ObstacleV2Parser.Deserialize));
        data.RawBPMEvents.AddRange(events.Where(v2event => v2event.Type == 100).Select(v2event => new BPMEvent { B = v2event.Beat, M = v2event.FloatValue }));

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

        IOrderedEnumerable<BeatObject> orderedObjects = data.Bombs.Cast<BeatObject>().Concat(data.Notes).OrderBy(obj => obj.B);
        obj["_notes"] = new JArray(orderedObjects.Select(o => o is Bomb b ? BombV2Parser.Serialize(b) : NoteV2Parser.Serialize((Note)o)));
        obj["_obstacles"] = new JArray(data.Obstacles.Select(o => ObstacleV2Parser.Serialize(o, data.Version)));

        if (data.Version >= BeatmapRevision.V260)
            obj["_sliders"] = new JArray(data.Arcs.Select(ArcV2Parser.Serialize));

        return obj;
    }
}

/// <summary> DifficultyData parser for V3 </summary>
public static class DifficultyV3Parser
{
    /// <summary> Deserialize DifficultyData from JObject </summary>
    public static void Deserialize(JObject obj, DifficultyData data)
    {
        JToken notes = obj["colorNotes"] ?? new JArray();
        JToken bombs = obj["bombNotes"] ?? new JArray();
        JToken arcs = obj["sliders"] ?? new JArray();
        JToken chains = obj["burstSliders"] ?? new JArray();
        JToken obstacles = obj["obstacles"] ?? new JArray();
        List<BPMEvent> bpmChanges = obj["bpmEvents"]?.ToObject<List<BPMEvent>>() ?? [];

        data.Notes.AddRange(notes.Select(NoteV3Parser.Deserialize));
        data.Bombs.AddRange(bombs.Select(BombV3Parser.Deserialize));
        data.Arcs.AddRange(arcs.Select(ArcV3Parser.Deserialize));
        data.Chains.AddRange(chains.Select(ChainV3Parser.Deserialize));
        data.Obstacles.AddRange(obstacles.Select(ObstacleV3Parser.Deserialize));
        data.RawBPMEvents.AddRange(bpmChanges);
    }

    /// <summary> Serializes patchable map data from DifficultyData </summary>
    public static JObject Serialize(DifficultyData data)
    {
        JObject obj = JObject.Parse(data.RawJSON ?? "{}");
        obj["version"] = data.Version.ToVersionString();
        obj["colorNotes"] = new JArray(data.Notes.Select(NoteV3Parser.Serialize));
        obj["bombNotes"] = new JArray(data.Bombs.Select(BombV3Parser.Serialize));
        obj["sliders"] = new JArray(data.Arcs.Select(ArcV3Parser.Serialize));
        obj["burstSliders"] = new JArray(data.Chains.Select(ChainV3Parser.Serialize));
        obj["obstacles"] = new JArray(data.Obstacles.Select(ObstacleV3Parser.Serialize));
        return obj;
    }
}

/// <summary> DifficultyData parser for V4 </summary>
public static class DifficultyV4Parser
{
    /// <summary> Deserialize DifficultyData from JObject </summary>
    public static void Deserialize(JObject obj, DifficultyData data)
    {
        JToken notes = obj["colorNotes"] ?? new JArray();
        JToken notesData = obj["colorNotesData"] ?? new JArray();
        JToken bombs = obj["bombNotes"] ?? new JArray();
        JToken bombsData = obj["bombNotesData"] ?? new JArray();
        JToken arcs = obj["arcs"] ?? new JArray();
        JToken arcsData = obj["arcsData"] ?? new JArray();
        JToken chains = obj["chains"] ?? new JArray();
        JToken chainsData = obj["chainsData"] ?? new JArray();
        JToken obstacles = obj["obstacles"] ?? new JArray();
        JToken obstaclesData = obj["obstaclesData"] ?? new JArray();

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

        JArray colorNotes = new();
        JArray colorNotesData = new();
        Dictionary<string, int> colorNotesDataMap = new();

        JArray bombNotes = new();
        JArray bombNotesData = new();
        Dictionary<string, int> bombNotesDataMap = new();

        JArray obstaclesArr = new();
        JArray obstaclesData = new();
        Dictionary<string, int> obstaclesDataMap = new();

        JArray arcsArr = new();
        JArray arcsData = new();
        Dictionary<string, int> arcsDataMap = new();

        JArray chainsArr = new();
        JArray chainsData = new();
        Dictionary<string, int> chainsDataMap = new();

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