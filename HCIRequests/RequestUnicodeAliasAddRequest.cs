using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Unicode Alias Add (HCIv2) - Message ID 0x00F4.
/// This message is used by the host to change the text of a Directory Entry
/// specified in the system configuration map. By adding an alias for a specific
/// Directory Entry, this text will be used in place of the Directory Entry text
/// until the alias is deleted.
/// </summary>
public class RequestUnicodeAliasAddRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Protocol schema version.
    /// </summary>
    private const byte ProtocolSchema = 0x01;

    /// <summary>
    /// The list of alias entries to add.
    /// </summary>
    public List<UnicodeAliasEntry> Entries { get; } = new();

    /// <summary>
    /// Creates a new Request Unicode Alias Add request.
    /// </summary>
    public RequestUnicodeAliasAddRequest()
        : base(HCIMessageID.RequestUnicodeAliasAdd)
    {
    }

    /// <summary>
    /// Adds an alias entry.
    /// </summary>
    /// <param name="dialcode">The dial code (4 bytes).</param>
    /// <param name="text">The alias text (up to 10 unicode characters).</param>
    /// <param name="inhibitRemote">If true, inhibit on remote systems.</param>
    /// <param name="inhibitLocal">If true, inhibit on local systems.</param>
    public void AddAlias(byte[] dialcode, string text, bool inhibitRemote = false, bool inhibitLocal = false)
    {
        Entries.Add(new UnicodeAliasEntry(dialcode, text, inhibitRemote, inhibitLocal));
    }

    /// <summary>
    /// Adds an alias entry for local use only.
    /// </summary>
    /// <param name="dialcode">The dial code (4 bytes).</param>
    /// <param name="text">The alias text (up to 10 unicode characters).</param>
    public void AddLocalAlias(byte[] dialcode, string text)
    {
        AddAlias(dialcode, text, inhibitRemote: true, inhibitLocal: false);
    }

    /// <summary>
    /// Adds an alias entry for remote use only.
    /// </summary>
    /// <param name="dialcode">The dial code (4 bytes).</param>
    /// <param name="text">The alias text (up to 10 unicode characters).</param>
    public void AddRemoteAlias(byte[] dialcode, string text)
    {
        AddAlias(dialcode, text, inhibitRemote: false, inhibitLocal: true);
    }

    /// <summary>
    /// Generates the payload for this request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Calculate total size
        // ProtocolTag(4) + ProtocolSchema(1) + entries * 28 bytes each
        int totalSize = 4 + 1;
        foreach (var entry in Entries)
        {
            totalSize += 28; // Each entry is 28 bytes
        }

        var payload = new byte[totalSize];
        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // Entries
        foreach (var entry in Entries)
        {
            var entryBytes = entry.ToBytes();
            Array.Copy(entryBytes, 0, payload, offset, entryBytes.Length);
            offset += entryBytes.Length;
        }

        return payload;
    }
}
