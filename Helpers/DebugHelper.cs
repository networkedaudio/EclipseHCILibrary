using System.Diagnostics;

namespace HCILibrary.Helpers;

/// <summary>
/// Helper class for debug output operations.
/// </summary>
public static class DebugHelper
{
    /// <summary>
    /// Writes a byte array to the debug output in 0xXX format.
    /// </summary>
    /// <param name="label">A label to prefix the output.</param>
    /// <param name="bytes">The byte array to write.</param>
    [Conditional("DEBUG")]
    public static void WriteBytes(string label, byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
        {
            Debug.WriteLine($"{label}: (empty)");
            return;
        }

        var hexString = string.Join(" ", bytes.Select(b => $"0x{b:X2}"));
        Debug.WriteLine($"{label} ({bytes.Length} bytes): {hexString}");
    }

    /// <summary>
    /// Writes a byte array to the debug output in 0xXX format with no label.
    /// </summary>
    /// <param name="bytes">The byte array to write.</param>
    [Conditional("DEBUG")]
    public static void WriteBytes(byte[] bytes)
    {
        WriteBytes("Bytes", bytes);
    }

    /// <summary>
    /// Writes a byte span to the debug output in 0xXX format.
    /// </summary>
    /// <param name="label">A label to prefix the output.</param>
    /// <param name="bytes">The byte span to write.</param>
    [Conditional("DEBUG")]
    public static void WriteBytes(string label, ReadOnlySpan<byte> bytes)
    {
        WriteBytes(label, bytes.ToArray());
    }

    /// <summary>
    /// Writes a byte array to the debug output in 0xXX format, split into rows.
    /// </summary>
    /// <param name="label">A label to prefix the output.</param>
    /// <param name="bytes">The byte array to write.</param>
    /// <param name="bytesPerRow">Number of bytes per row (default: 16).</param>
    [Conditional("DEBUG")]
    public static void WriteBytesFormatted(string label, byte[] bytes, int bytesPerRow = 16)
    {
        if (bytes == null || bytes.Length == 0)
        {
            Debug.WriteLine($"{label}: (empty)");
            return;
        }

        Debug.WriteLine($"{label} ({bytes.Length} bytes):");

        for (int i = 0; i < bytes.Length; i += bytesPerRow)
        {
            int count = Math.Min(bytesPerRow, bytes.Length - i);
            var hexString = string.Join(" ", bytes.Skip(i).Take(count).Select(b => $"0x{b:X2}"));
            Debug.WriteLine($"  {i:D4}: {hexString}");
        }
    }
}
