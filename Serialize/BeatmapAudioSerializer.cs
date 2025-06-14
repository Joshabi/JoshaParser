﻿using JoshaParser.Data.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace JoshaParser.Serialize;

/// <summary> Handles serialize and deserialize operations for Beatmap Audio Info (V4) </summary>
public class BeatmapAudioInfoSerializer : JsonConverter<AudioInfo>
{
    /// <summary> Handles reading audio.dat json </summary>
    public override AudioInfo ReadJson(JsonReader reader, Type objectType, AudioInfo? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        return new AudioInfo()
        {
            Version = jsonObject["version"]?.ToString() ?? string.Empty,
            SongChecksum = jsonObject["songChecksum"]?.ToString() ?? string.Empty,
            SongSampleCount = jsonObject["songSampleCount"]?.ToObject<int>() ?? 0,
            SongFrequency = jsonObject["songFrequency"]?.ToObject<int>() ?? 0,
            BPMData = jsonObject["bpmData"]?.ToObject<List<BPMDataSegment>>(serializer) ?? [],
            LUFSData = jsonObject["lufsData"]?.ToObject<List<LUFSDataSegment>>(serializer) ?? []
        };
    }

    /// <summary> [Unsupported] Handles writing to audio.dat json </summary>
    public override void WriteJson(JsonWriter writer, AudioInfo? value, JsonSerializer serializer)
    {
        Trace.WriteLine("Serializing back to file is not supported");
        writer.WriteNull();
        return;
    }
}
