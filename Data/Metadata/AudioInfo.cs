namespace JoshaParser.Data.Metadata;

/// <summary> Parsed V4 BPM/Audio.dat </summary>
public class AudioInfo
{
    /// <summary> Format Version </summary>
    public string Version { get; set; } = string.Empty;
    /// <summary> Audio Checksum </summary>
    public string SongChecksum { get; set; } = string.Empty;
    /// <summary> Duration of audio file in samples </summary>
    public int SongSampleCount { get; set; }
    /// <summary> Cached quality of audio file </summary>
    public int SongFrequency { get; set; }
    /// <summary> List of data that alters BPM by region </summary>
    public List<BPMDataSegment> BPMData { get; set; } = [];
    /// <summary> List of normalization data for loudness by region </summary>
    public List<LUFSDataSegment> LUFSData { get; set; } = [];
}

/// <summary> Represents BPM Data Segments </summary>
public class BPMDataSegment
{
    /// <summary> Start Sample Index </summary>
    public int SI { get; set; }
    /// <summary> End Sample Index </summary>
    public int EI { get; set; }
    /// <summary> Start Beat </summary>
    public int SB { get; set; }
    /// <summary> End Beat </summary>
    public int EB { get; set; }
}

/// <summary> Represents LUFS Data Segments </summary>
public class LUFSDataSegment
{
    /// <summary> Start Sample Index </summary>
    public int SI { get; set; }
    /// <summary> End Sample Index </summary>
    public int EI { get; set; }
    /// <summary> Loudness </summary>
    public float L { get; set; }
}