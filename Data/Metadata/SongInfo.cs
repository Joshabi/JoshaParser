using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JoshaParser.Data.Metadata;

/// <summary> Parsed Song Info data </summary>
public class SongInfo
{
    public string MapPath { get; set; } = string.Empty;
    public BeatmapInfoRevision Version { get; set; } = BeatmapInfoRevision.Unknown;
    public string SongName { get; set; } = string.Empty;
    public string SongSubName { get; set; } = string.Empty;
    public string SongArtist { get; set; } = string.Empty;
    public string Mapper { get; set; } = string.Empty;
    public AudioData Song { get; set; } = new();
    public float Shuffle { get; set; }
    public float ShufflePeriod { get; set; }
    public float SongTimeOffset { get; set; }
    public string CoverImageFilename { get; set; } = string.Empty;
    public string EnvironmentName { get; set; } = string.Empty;
    public string AllDirectionsEnvironmentName { get; set; } = string.Empty;
    public List<string> EnvironmentNames { get; set; } = [];
    public List<DifficultyInfo> DifficultyBeatmaps { get; set; } = [];
    public List<ContributorInfo> Contributors { get; set; } = [];

    public string? RawJSON { get; set; }

    public override string ToString()
    {
        var difficulties = DifficultyBeatmaps != null && DifficultyBeatmaps.Count > 0
            ? string.Join("\n", DifficultyBeatmaps.Select(d => d.ToString()))
            : "(no difficulties)";

        return
            "\n" +
            $"Map Path:          {MapPath}\n" +
            $"Song Name:         {SongName} {SongSubName}\n" +
            $"Artist:            {SongArtist}\n" +
            $"Mapper:            {Mapper}\n" +
            $"BPM:               {Song.BPM}\n" +
            $"Version:           {Version}\n" +
            $"Cover Image File:  {CoverImageFilename}\n\n" +
            $"Difficulties:\n{difficulties}\n";
    }
}

/// <summary> Represents Mapper/Lighter metadata </summary>
public class BeatmapAuthors
{
    public List<string> Mappers { get; set; } = [];
    public List<string> Lighters { get; set; } = [];
    public override string ToString()
    {
        return $"Mappers: {Mappers}, Lighters:{Lighters}";
    }
}

/// <summary> Represents Song metadata </summary>
public class AudioData
{
    public string SongFilename { get; set; } = string.Empty;
    public float SongDuration { get; set; }
    public string AudioDataFilename { get; set; } = string.Empty;
    public float BPM { get; set; }
    public float LUFS { get; set; }
    public float PreviewStartTime { get; set; }
    public float PreviewDuration { get; set; }
    public string SongPreviewFilename { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Song Filename: {SongFilename}, Duration:{SongDuration}, Audio Data Filename:{AudioDataFilename}, BPM:{BPM}, LUFS:{LUFS}, Preview Start:{PreviewStartTime}, Preview Duration:{PreviewDuration}";
    }
}

/// <summary> Represents difficulty metadata </summary>
public class DifficultyInfo
{
    public string Characteristic { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string DifficultyLabel { get; set; } = string.Empty;
    public BeatmapDifficultyRank Rank { get; set; } = BeatmapDifficultyRank.ExpertPlus;
    public BeatmapAuthors BeatmapAuthors { get; set; } = new();
    public int EnvironmentNameIdx { get; set; }
    public int BeatmapColorSchemeIdx { get; set; }
    public float NoteJumpMovementSpeed { get; set; }
    public float NoteJumpStartBeatOffset { get; set; }
    public string BeatmapDataFilename { get; set; } = string.Empty;
    public string LightshowDataFilename { get; set; } = string.Empty;
    public DifficultyData DifficultyData { get; set; } = new();

    public override string ToString()
    {
        return
            $"----------------------------------------------------\n" +
            $"Characteristic:              {Characteristic}\n" +
            $"Difficulty Label:            {DifficultyLabel}\n" +
            $"Rank:                        {Rank}\n" +
            $"Note Jump Movement Speed:    {NoteJumpMovementSpeed}\n" +
            $"Note Jump Start Beat Offset: {NoteJumpStartBeatOffset}\n" +
            $"Beatmap Data Filename:       {BeatmapDataFilename}\n" +
            $"Lightshow Data Filename:     {LightshowDataFilename}";
    }
}

/// <summary> Represents CustomData for Contributor Metadata </summary>
public class ContributorInfo
{
    [JsonProperty("_name")] public string Name { get; set; } = "";
    [JsonProperty("_role")] public string Role { get; set; } = "";
    [JsonProperty("_iconPath")] public string IconPath { get; set; } = "";
}

/// <summary> Represents version of Song Info Data </summary>
public enum BeatmapInfoRevision
{
    Unknown = 0,
    V200 = 1,
    V210 = 2,
    V400 = 3,
    V401 = 4
}

/// <summary> Difficulty ID Enum </summary>
public enum BeatmapDifficultyRank
{
    Easy = 1,
    Normal = 3,
    Hard = 5,
    Expert = 7,
    ExpertPlus = 9
}
