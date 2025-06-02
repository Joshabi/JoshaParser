namespace JoshaParser.Data.Beatmap;

/// <summary> TimeScaler for BPM Changes </summary>
public class BPMTimeScaler
{
    public float T { get; set; }
    public float S { get; set; }
}

/// <summary> BPM Change Event representing V3 BPM Changes </summary>
public class BPMChangeEvent(BPMEvent bpmEvent)
{
    public float B { get; set; } = bpmEvent.B;
    public float M { get; set; } = bpmEvent.M;
    public float P { get; set; } = 0;
    public float O { get; set; } = 0;
    public float NB { get; set; } = bpmEvent.B;
}

// Referenced:
// https://github.com/LightAi39/ChroMapper-AutoModder/blob/main/ChroMapper-LightModding/BeatmapScanner/MapCheck/Timescale.cs
// https://github.com/KivalEvan/BeatSaber-MapCheck/blob/main/src/ts/beatmap/shared/bpm.ts
/// <summary> Context object for BPM of a difficulty </summary>
public class BPMContext
{
    private readonly List<BPMTimeScaler> _timeScale;
    private readonly float _offset;

    public float BPM { get; private set; }
    public List<BPMChangeEvent> BPMChanges { get; private set; }

    /// <summary> Construct BPMContext given BPM Changes and song time offset </summary>
    public BPMContext(float BPM, List<BPMChangeEvent> BPMChanges, float offset)
    {
        this.BPM = BPM;
        _offset = offset;
        this.BPMChanges = GetBPMChangeTime(BPMChanges);
        _timeScale = GetTimeScale(BPMChanges);
    }

    /// <summary> Returns a new BPMContext given a list of V3 BPM events and map offset </summary>
    public static BPMContext CreateBPMContext(float BPM, List<BPMEvent> BPMChanges, float offset)
    {
        List<BPMChangeEvent> changes = [];
        foreach (BPMEvent BPMEvent in BPMChanges) {
            changes.Add(new(BPMEvent));
        }
        return new BPMContext(BPM, changes, offset);
    }

    /// <summary> Calculates NewTime for all BPMChangeEvents </summary>
    public List<BPMChangeEvent> GetBPMChangeTime(List<BPMChangeEvent> BPMChanges)
    {
        List<BPMChangeEvent> alteredBPMChanges = [];
        BPMChangeEvent? temp = null;

        foreach (BPMChangeEvent curBPMChange in BPMChanges) {
            curBPMChange.NB = temp != null
                ? (float)Math.Ceiling(((curBPMChange.B - temp.B) / BPM * temp.M) + temp.NB - 0.01)
                : (float)Math.Ceiling(curBPMChange.B - (_offset * BPM / 60) - 0.01);
            alteredBPMChanges.Add(curBPMChange);
            temp = curBPMChange;
        }

        return alteredBPMChanges;
    }

    /// <summary> Grabs TimeScale based on BPMChanges </summary>
    public List<BPMTimeScaler> GetTimeScale(List<BPMChangeEvent> BPMChanges)
    {
        List<BPMTimeScaler> timeScale = [];
        foreach (BPMChangeEvent bpm in BPMChanges) {
            BPMTimeScaler ibpm = new()
            {
                T = bpm.B,
                S = BPM / bpm.M
            };
            timeScale.Add(ibpm);
        }

        return timeScale;
    }

    /// <summary> Converts a point in beats to seconds accounting for BPM Changes </summary>
    public float ToRealTime(float beat, bool timescale = true)
    {
        if (!timescale) return beat / BPM * 60;

        float calculatedBeat = 0;
        for (int i = _timeScale.Count - 1; i >= 0; i--) {
            if (beat > _timeScale[i].T) {
                calculatedBeat += (beat - _timeScale[i].T) * _timeScale[i].S;
                beat = _timeScale[i].T;
            }
        }

        return (beat + calculatedBeat) / BPM * 60;
    }

    /// <summary> Converts a point in seconds to beats accounting for BPM Changes </summary>
    public float ToBeatTime(float seconds, bool timescale = false)
    {
        if (!timescale) return seconds * BPM / 60;

        float calculatedSecond = 0;
        for (int i = _timeScale.Count - 1; i >= 0; i--) {
            float currentSeconds = ToRealTime(_timeScale[i].T);
            if (seconds > currentSeconds) {
                calculatedSecond += (seconds - currentSeconds) / _timeScale[i].S;
                seconds = currentSeconds;
            }
        }
        return ToBeatTime(seconds + calculatedSecond);
    }

    /// <summary> Converts to JSON File time </summary>
    public float ToJsonTime(float beat)
    {
        for (int i = BPMChanges.Count - 1; i >= 0; i--) {
            if (beat > BPMChanges[i].NB) {
                return ((beat - BPMChanges[i].NB) / BPMChanges[i].M * BPM) + BPMChanges[i].B;
            }
        }
        return ToBeatTime(ToRealTime(beat, false) + _offset);
    }

    /// <summary> Updates the current BPM value stored based on BPM Changes and provided beat value </summary>
    public void SetCurrentBPM(float beat)
    {
        for (int i = 0; i < BPMChanges.Count; i++) {
            if (beat > BPMChanges[i].B) {
                BPM = BPMChanges[i].M;
            }
        }
    }
}