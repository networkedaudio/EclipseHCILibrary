using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Set Config for Multiple Keys (0x00CD).
/// Sets the config for multiple keys on a panel with advanced key property control
/// e.g. latch mode and activation type.
/// </summary>
public class RequestSetConfigMultipleKeysRequest : HCIRequest
{
    /// <summary>
    /// Protocol schema version (1 or 2).
    /// Schema 2 includes Endpoint Type field.
    /// </summary>
    public byte Schema { get; set; } = 1;

    /// <summary>
    /// Card slot number.
    /// </summary>
    public byte Slot { get; set; }

    /// <summary>
    /// Port offset from first port of the card.
    /// </summary>
    public byte Port { get; set; }

    /// <summary>
    /// Endpoint type (only used with Schema 2).
    /// Used when slot and offset target a role.
    /// Set to 0 if not assigning to a role.
    /// Example: 0x8200 for FS II beltpack key layout.
    /// </summary>
    public ushort EndpointType { get; set; }

    /// <summary>
    /// List of key configuration entries.
    /// </summary>
    public List<MultipleKeyConfigEntry> Keys { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestSetConfigMultipleKeysRequest"/> class.
    /// </summary>
    public RequestSetConfigMultipleKeysRequest()
        : base(HCIMessageID.RequestSetConfigMultipleKeys)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestSetConfigMultipleKeysRequest"/> class
    /// with specified slot and port.
    /// </summary>
    /// <param name="slot">Card slot number.</param>
    /// <param name="port">Port offset from first port of the card.</param>
    /// <param name="schema">Protocol schema version (1 or 2).</param>
    public RequestSetConfigMultipleKeysRequest(byte slot, byte port, byte schema = 1)
        : base(HCIMessageID.RequestSetConfigMultipleKeys)
    {
        Slot = slot;
        Port = port;
        Schema = schema;
    }

    /// <summary>
    /// Generates the payload for the Set Config Multiple Keys message.
    /// </summary>
    /// <returns>The message payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Calculate payload size:
        // Schema 1: Slot(1) + Port(1) + Count(1) + Keys(36 * count)
        // Schema 2: Slot(1) + Port(1) + EndpointType(2) + Count(1) + Keys(36 * count)
        int headerSize = Schema >= 2 ? 5 : 3;
        int keySize = 36; // Per key entry size
        int totalSize = headerSize + (Keys.Count * keySize);

        var payload = new byte[totalSize];
        int offset = 0;

        // Slot: 1 byte
        payload[offset++] = Slot;

        // Port: 1 byte
        payload[offset++] = Port;

        // Endpoint Type: 2 bytes (Schema 2 only, big-endian)
        if (Schema >= 2)
        {
            payload[offset++] = (byte)((EndpointType >> 8) & 0xFF);
            payload[offset++] = (byte)(EndpointType & 0xFF);
        }

        // Count: 1 byte
        payload[offset++] = (byte)Keys.Count;

        // Key entries
        foreach (var key in Keys)
        {
            var keyBytes = key.ToBytes();
            Array.Copy(keyBytes, 0, payload, offset, keyBytes.Length);
            offset += keyBytes.Length;
        }

        return payload;
    }

    /// <summary>
    /// Adds a key configuration entry.
    /// </summary>
    /// <param name="entry">The key configuration entry to add.</param>
    public void AddKey(MultipleKeyConfigEntry entry)
    {
        Keys.Add(entry);
    }

    /// <summary>
    /// Adds a key configuration with the specified parameters.
    /// </summary>
    /// <param name="region">Region this key is on.</param>
    /// <param name="keyId">This key's ID.</param>
    /// <param name="page">Page this key is on.</param>
    /// <param name="entity">Entity type.</param>
    /// <param name="keyOperation">Key operation settings.</param>
    /// <param name="keyConfig">Key configuration settings.</param>
    public void AddKey(byte region, ushort keyId, byte page, ushort entity,
        KeyOperationSettings? keyOperation = null, KeyConfigSettings? keyConfig = null)
    {
        Keys.Add(new MultipleKeyConfigEntry
        {
            Region = region,
            KeyId = keyId,
            Page = page,
            Entity = entity,
            KeyOperation = keyOperation ?? new KeyOperationSettings(),
            KeyConfig = keyConfig ?? new KeyConfigSettings()
        });
    }
}
