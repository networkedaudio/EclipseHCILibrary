namespace HCILibrary.Helpers;

/// <summary>
/// Helper class for VOX threshold gain level calculations.
/// VOX gain is calculated as: gain (dB) = level - 40
/// Range: 0 (-40 dB) to 40 (0 dB)
/// </summary>
public static class VoxGain
{
    /// <summary>
    /// The minimum VOX level value (corresponds to -40 dB).
    /// </summary>
    public const ushort MinLevel = 0;

    /// <summary>
    /// The maximum VOX level value (corresponds to 0 dB).
    /// </summary>
    public const ushort MaxLevel = 40;

    /// <summary>
    /// The dB offset used to convert level to dB.
    /// Formula: dB = level - 40
    /// </summary>
    public const int DbOffset = 40;

    #region Common Level Constants

    /// <summary>-40 dB - Level 0 (most sensitive)</summary>
    public const ushort Minus40dB = 0;

    /// <summary>-35 dB - Level 5</summary>
    public const ushort Minus35dB = 5;

    /// <summary>-30 dB - Level 10</summary>
    public const ushort Minus30dB = 10;

    /// <summary>-25 dB - Level 15</summary>
    public const ushort Minus25dB = 15;

    /// <summary>-20 dB - Level 20 (default VOX threshold)</summary>
    public const ushort Minus20dB = 20;

    /// <summary>-15 dB - Level 25</summary>
    public const ushort Minus15dB = 25;

    /// <summary>-10 dB - Level 30</summary>
    public const ushort Minus10dB = 30;

    /// <summary>-5 dB - Level 35</summary>
    public const ushort Minus5dB = 35;

    /// <summary>0 dB - Level 40 (least sensitive)</summary>
    public const ushort Zero0dB = 40;

    /// <summary>Default VOX threshold level (20, which is -20 dB).</summary>
    public const ushort DefaultLevel = 20;

    #endregion

    /// <summary>
    /// Converts a VOX level value (0-40) to decibels (-40 to 0).
    /// </summary>
    /// <param name="level">The VOX level value (0-40).</param>
    /// <returns>The gain in decibels (-40 to 0).</returns>
    public static int ToDecibels(ushort level)
    {
        return level - DbOffset;
    }

    /// <summary>
    /// Converts a decibel value (-40 to 0) to a VOX level value (0-40).
    /// </summary>
    /// <param name="decibels">The gain in decibels (-40 to 0).</param>
    /// <returns>The VOX level value (0-40).</returns>
    public static ushort FromDecibels(int decibels)
    {
        int level = decibels + DbOffset;
        return (ushort)Math.Clamp(level, MinLevel, MaxLevel);
    }

    /// <summary>
    /// Validates whether a VOX level value is within the valid range (0-40).
    /// </summary>
    /// <param name="level">The VOX level value to validate.</param>
    /// <returns>True if the level is valid (0-40), false otherwise.</returns>
    public static bool IsValidLevel(ushort level)
    {
        return level <= MaxLevel;
    }

    /// <summary>
    /// Clamps a VOX level value to the valid range (0-40).
    /// </summary>
    /// <param name="level">The VOX level value to clamp.</param>
    /// <returns>The clamped level value.</returns>
    public static ushort ClampLevel(ushort level)
    {
        return Math.Min(level, MaxLevel);
    }

    /// <summary>
    /// Gets a descriptive string for a VOX level value.
    /// </summary>
    /// <param name="level">The VOX level value (0-40).</param>
    /// <returns>A string describing the level (e.g., "-20 dB").</returns>
    public static string GetDescription(ushort level)
    {
        int db = ToDecibels(level);
        return $"{db} dB";
    }

    /// <summary>
    /// Determines if the given level is the default VOX threshold.
    /// </summary>
    /// <param name="level">The VOX level value.</param>
    /// <returns>True if this is the default level (20), false otherwise.</returns>
    public static bool IsDefaultLevel(ushort level)
    {
        return level == DefaultLevel;
    }
}
