namespace HCILibrary.Helpers;

/// <summary>
/// Helper class for crosspoint gain level calculations and common level values.
/// Gain is calculated as: gain (dB) = (level value - 204) * 0.355
/// </summary>
public static class GainLevel
{
    /// <summary>
    /// The reference level value for 0 dB gain.
    /// </summary>
    public const ushort UnityGainLevel = 204;

    /// <summary>
    /// The multiplier used to convert level to dB.
    /// </summary>
    public const double DbPerLevel = 0.355;

    #region Common Level Constants

    /// <summary>Cut (mute) - Level 0</summary>
    public const ushort Cut = 0;

    /// <summary>-72 dB - Level 1</summary>
    public const ushort Minus72dB = 1;

    /// <summary>-66 dB - Level 18</summary>
    public const ushort Minus66dB = 18;

    /// <summary>-60 dB - Level 35</summary>
    public const ushort Minus60dB = 35;

    /// <summary>-54 dB - Level 52</summary>
    public const ushort Minus54dB = 52;

    /// <summary>-48 dB - Level 69</summary>
    public const ushort Minus48dB = 69;

    /// <summary>-42 dB - Level 86</summary>
    public const ushort Minus42dB = 86;

    /// <summary>-36 dB - Level 103</summary>
    public const ushort Minus36dB = 103;

    /// <summary>-30 dB - Level 119</summary>
    public const ushort Minus30dB = 119;

    /// <summary>-24 dB - Level 136</summary>
    public const ushort Minus24dB = 136;

    /// <summary>-18 dB - Level 153</summary>
    public const ushort Minus18dB = 153;

    /// <summary>-12 dB - Level 170</summary>
    public const ushort Minus12dB = 170;

    /// <summary>-9 dB - Level 179</summary>
    public const ushort Minus9dB = 179;

    /// <summary>-6 dB - Level 187</summary>
    public const ushort Minus6dB = 187;

    /// <summary>-3 dB - Level 196</summary>
    public const ushort Minus3dB = 196;

    /// <summary>0 dB (unity gain) - Level 204</summary>
    public const ushort Unity0dB = 204;

    /// <summary>+3 dB - Level 212</summary>
    public const ushort Plus3dB = 212;

    /// <summary>+6 dB - Level 221</summary>
    public const ushort Plus6dB = 221;

    /// <summary>+9 dB - Level 229</summary>
    public const ushort Plus9dB = 229;

    /// <summary>+12 dB - Level 238</summary>
    public const ushort Plus12dB = 238;

    /// <summary>+15 dB - Level 246</summary>
    public const ushort Plus15dB = 246;

    /// <summary>+18 dB - Level 255</summary>
    public const ushort Plus18dB = 255;

    /// <summary>+21 dB - Level 263</summary>
    public const ushort Plus21dB = 263;

    /// <summary>+24 dB - Level 272</summary>
    public const ushort Plus24dB = 272;

    /// <summary>+27 dB - Level 280</summary>
    public const ushort Plus27dB = 280;

    /// <summary>+29 dB - Level 287</summary>
    public const ushort Plus29dB = 287;

    #endregion

    /// <summary>
    /// Converts a level value to gain in dB.
    /// </summary>
    /// <param name="level">The level value (0-287).</param>
    /// <returns>The gain in dB.</returns>
    public static double LevelToDb(ushort level)
    {
        if (level == 0) return double.NegativeInfinity; // Cut
        return (level - UnityGainLevel) * DbPerLevel;
    }

    /// <summary>
    /// Converts a gain in dB to a level value.
    /// </summary>
    /// <param name="db">The gain in dB (-72 to +29).</param>
    /// <returns>The level value.</returns>
    public static ushort DbToLevel(double db)
    {
        if (double.IsNegativeInfinity(db)) return Cut;
        
        int level = (int)Math.Round((db / DbPerLevel) + UnityGainLevel);
        return (ushort)Math.Clamp(level, 0, 287);
    }

    /// <summary>
    /// Gets a formatted string representation of a level value.
    /// </summary>
    /// <param name="level">The level value.</param>
    /// <returns>A string like "+6 dB" or "Cut".</returns>
    public static string FormatLevel(ushort level)
    {
        if (level == 0) return "Cut";
        
        double db = LevelToDb(level);
        return db >= 0 ? $"+{db:F1} dB" : $"{db:F1} dB";
    }
}
