namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents an alias entry in a Reply Unicode Alias Status message.
/// </summary>
public class UnicodeAliasStatusEntry
{
    /// <summary>
    /// The dial code (4 bytes).
    /// </summary>
    public byte[] Dialcode { get; set; } = new byte[4];

    /// <summary>
    /// The alias text (up to 10 unicode characters).
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// If true, the alias is inhibited from being used on remote systems.
    /// </summary>
    public bool InhibitRemote { get; set; }

    /// <summary>
    /// If true, the alias is inhibited from being used on local systems.
    /// </summary>
    public bool InhibitLocal { get; set; }

    /// <summary>
    /// If true, the text is unicode (UTF-16 BE).
    /// </summary>
    public bool IsUnicode { get; set; }

    /// <summary>
    /// If true, the alias is only applied to listen keys (used for E-Dante ports).
    /// </summary>
    public bool ListenKeysOnly { get; set; }

    public override string ToString()
    {
        var scope = (InhibitLocal, InhibitRemote) switch
        {
            (false, false) => "Local+Remote",
            (true, false) => "Remote only",
            (false, true) => "Local only",
            (true, true) => "None"
        };
        var listenOnly = ListenKeysOnly ? " [Listen only]" : "";
        return $"\"{Text}\" ({scope}){listenOnly}";
    }
}

/// <summary>
/// Reply Unicode Alias Status (HCIv2) - Message ID 0x00F5.
/// Sent in response to Request Alias Add (U flag set, flags = 0x0C) or
/// Request Alias List (U flag clear, flags = 0x08).
/// </summary>
public class ReplyUnicodeAliasStatus
{
    /// <summary>
    /// Indicates if this is a response to an Alias Add request (true) or Alias List request (false).
    /// Based on the U flag in the message flags.
    /// </summary>
    public bool IsAliasAddResponse { get; set; }

    /// <summary>
    /// The list of alias entries.
    /// </summary>
    public List<UnicodeAliasStatusEntry> Entries { get; } = new();

    /// <summary>
    /// Decodes a Reply Unicode Alias Status from the payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes (after protocol tag and schema).</param>
    /// <param name="uFlagSet">Whether the U flag is set in the message flags.</param>
    /// <returns>The decoded reply, or null if invalid.</returns>
    public static ReplyUnicodeAliasStatus? Decode(byte[] payload, bool uFlagSet = false)
    {
        if (payload == null || payload.Length < 2)
        {
            return null;
        }

        var reply = new ReplyUnicodeAliasStatus
        {
            IsAliasAddResponse = uFlagSet
        };

        int offset = 0;

        // Process entries until we run out of payload
        // Each entry: Count(2) + Dialcode(4) + Text(20) + UnicodeInfo(2) = 28 bytes
        while (offset + 28 <= payload.Length)
        {
            var entry = new UnicodeAliasStatusEntry();

            // Count word: 16 bit (big-endian)
            // bits 10-13: unused
            // bit 14: inhibit remote
            // bit 15: inhibit local
            ushort countWord = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            entry.InhibitRemote = (countWord & 0x4000) != 0; // bit 14
            entry.InhibitLocal = (countWord & 0x8000) != 0;  // bit 15
            offset += 2;

            // Dialcode: 4 bytes
            Array.Copy(payload, offset, entry.Dialcode, 0, 4);
            offset += 4;

            // Text: 20 bytes (10 unicode chars, UTF-16 BE)
            // We'll decode as unicode, trimming null characters
            entry.Text = System.Text.Encoding.BigEndianUnicode
                .GetString(payload, offset, 20)
                .TrimEnd('\0');
            offset += 20;

            // Unicode info: 16 bit word (big-endian)
            // bit 14: listen keys only
            // bit 15: is unicode
            ushort unicodeInfo = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            entry.ListenKeysOnly = (unicodeInfo & 0x4000) != 0; // bit 14
            entry.IsUnicode = (unicodeInfo & 0x8000) != 0;      // bit 15
            offset += 2;

            reply.Entries.Add(entry);
        }

        return reply;
    }

    public override string ToString()
    {
        var responseType = IsAliasAddResponse ? "Add Response" : "List Response";
        return $"Unicode Alias Status ({responseType}): {Entries.Count} alias(es)";
    }
}
