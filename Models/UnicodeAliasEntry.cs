namespace HCILibrary.Models;

/// <summary>
/// Represents an alias entry for Request Unicode Alias Add message.
/// </summary>
public class UnicodeAliasEntry
{
    /// <summary>
    /// Maximum length for alias text (10 unicode characters).
    /// </summary>
    public const int MaxTextLength = 10;

    /// <summary>
    /// The dial code (4 bytes).
    /// </summary>
    public byte[] Dialcode { get; set; } = new byte[4];

    /// <summary>
    /// The alias text (up to 10 unicode characters).
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// If true, inhibit the alias from being used on remote systems.
    /// </summary>
    public bool InhibitRemote { get; set; }

    /// <summary>
    /// If true, inhibit the alias from being used on local systems.
    /// </summary>
    public bool InhibitLocal { get; set; }

    /// <summary>
    /// Creates a new Unicode alias entry.
    /// </summary>
    public UnicodeAliasEntry()
    {
    }

    /// <summary>
    /// Creates a new Unicode alias entry with the specified values.
    /// </summary>
    /// <param name="dialcode">The dial code (4 bytes).</param>
    /// <param name="text">The alias text (up to 10 unicode characters).</param>
    /// <param name="inhibitRemote">If true, inhibit on remote systems.</param>
    /// <param name="inhibitLocal">If true, inhibit on local systems.</param>
    public UnicodeAliasEntry(byte[] dialcode, string text, bool inhibitRemote = false, bool inhibitLocal = false)
    {
        if (dialcode != null && dialcode.Length >= 4)
        {
            Array.Copy(dialcode, Dialcode, 4);
        }
        Text = text?.Length > MaxTextLength ? text.Substring(0, MaxTextLength) : text ?? string.Empty;
        InhibitRemote = inhibitRemote;
        InhibitLocal = inhibitLocal;
    }

    /// <summary>
    /// Generates the bytes for the count word with inhibit flags.
    /// </summary>
    /// <returns>The count word as a 16-bit value.</returns>
    public ushort GetCountWord()
    {
        ushort count = 1; // One alias in this entry
        if (InhibitRemote) count |= 0x4000; // bit 14
        if (InhibitLocal) count |= 0x8000;  // bit 15
        return count;
    }

    /// <summary>
    /// Generates the bytes for this alias entry.
    /// </summary>
    /// <returns>The entry bytes (Count + Dialcode + Text + UnicodeInfo).</returns>
    public byte[] ToBytes()
    {
        // Count(2) + Dialcode(4) + Text(20) + UnicodeInfo(2) = 28 bytes
        var bytes = new byte[28];
        int offset = 0;

        // Count: 16 bit word with inhibit flags (big-endian)
        ushort countWord = GetCountWord();
        bytes[offset++] = (byte)(countWord >> 8);
        bytes[offset++] = (byte)(countWord & 0xFF);

        // Dialcode: 4 bytes
        Array.Copy(Dialcode, 0, bytes, offset, 4);
        offset += 4;

        // Text: 10 unicode chars (UTF-16 BE, 20 bytes)
        byte[] textBytes = System.Text.Encoding.BigEndianUnicode.GetBytes(Text.PadRight(MaxTextLength, '\0'));
        int textBytesToCopy = Math.Min(textBytes.Length, 20);
        Array.Copy(textBytes, 0, bytes, offset, textBytesToCopy);
        offset += 20;

        // Unicode info: 16 bit word, bit 15 set to 1 for unicode
        bytes[offset++] = 0x80; // bit 15 set (big-endian high byte)
        bytes[offset++] = 0x00;

        return bytes;
    }

    public override string ToString()
    {
        var scope = (InhibitLocal, InhibitRemote) switch
        {
            (false, false) => "Local+Remote",
            (true, false) => "Remote only",
            (false, true) => "Local only",
            (true, true) => "None"
        };
        return $"\"{Text}\" ({scope})";
    }
}
