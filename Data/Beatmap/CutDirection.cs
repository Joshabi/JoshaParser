namespace JoshaParser.Data.Beatmap;

/// <summary> Enum Representation of Cut Directions </summary>
public enum CutDirection
{
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3,
    UpLeft = 4,
    UpRight = 5,
    DownLeft = 6,
    DownRight = 7,
    Any = 8
}

/// <summary> Extension methods for CutDirection </summary>
public static class CutDirectionExtensions
{
    /// <summary> Converts a CutDirection to a string </summary>
    public static string AsName(this CutDirection direction) => direction switch
    {
        CutDirection.Up => "Up",
        CutDirection.Down => "Down",
        CutDirection.Left => "Left",
        CutDirection.Right => "Right",
        CutDirection.UpLeft => "UpLeft",
        CutDirection.UpRight => "UpRight",
        CutDirection.DownLeft => "DownLeft",
        CutDirection.DownRight => "DownRight",
        CutDirection.Any => "Any",
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };

    /// <summary> CutDirection's in circular order </summary>
    public static readonly CutDirection[] Ordered =
    [
        CutDirection.Up,
        CutDirection.UpRight,
        CutDirection.Right,
        CutDirection.DownRight,
        CutDirection.Down,
        CutDirection.DownLeft,
        CutDirection.Left,
        CutDirection.UpLeft
    ];

    /// <summary> Rotates a cut direction either way </summary>
    public static CutDirection Rotate(this CutDirection dir, int steps = 1, bool clockwise = true)
    {
        int index = Array.IndexOf(Ordered, dir);
        if (index == -1)
            return dir; // Return the same if direction is Any or invalid

        int newIndex = (index + (clockwise ? steps : -steps) + Ordered.Length) % Ordered.Length;
        return Ordered[newIndex];
    }

    /// <summary> Finds the midpoint from this cut direction to another </summary>
    public static CutDirection MidwayTo(this CutDirection lastDirection, CutDirection targetDirection, bool roundUp = false)
    {
        int lastIndex = Array.IndexOf(Ordered, lastDirection);
        int targetIndex = Array.IndexOf(Ordered, targetDirection);

        // If either direction is CutDirection.Any, return the last direction
        if (lastIndex == -1 || targetIndex == -1) return lastDirection;

        // Calculate the shortest distance and direction (clockwise or counterclockwise)
        int distance = (targetIndex - lastIndex + Ordered.Length) % Ordered.Length;
        bool clockwise = distance <= Ordered.Length / 2;

        // Calculate halfway point in the chosen direction
        int halfwaySteps = (Math.Min(distance, Ordered.Length - distance) + (roundUp ? 1 : 0)) / 2;
        int midwayIndex = (lastIndex + (clockwise ? halfwaySteps : -halfwaySteps) + Ordered.Length) % Ordered.Length;

        return Ordered[midwayIndex];
    }

    /// <summary> Determins if a Cut Direction is within a certain interval distance where 45 degrees is 1 interval </summary>
    public static bool IsWithinIntervals(this CutDirection dir, CutDirection target, int intervals)
    {
        int dirIndex = Array.IndexOf(Ordered, dir);
        int targetIndex = Array.IndexOf(Ordered, target);

        if (dirIndex == -1 || targetIndex == -1 || intervals < 0) return false;

        int circularDistance = Math.Abs(targetIndex - dirIndex);
        circularDistance = Math.Min(circularDistance, Ordered.Length - circularDistance);
        return circularDistance <= intervals;
    }
}
