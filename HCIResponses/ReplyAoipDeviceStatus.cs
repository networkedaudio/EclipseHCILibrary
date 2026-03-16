using HCILibrary.Enums;
using System.Text;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Base class for a single AoIP Device Status sub-message entry.
/// Each sub-message has a common header (slot, device type, device ID, state)
/// plus type-specific fields populated by the relevant parser.
/// </summary>
public class AoipDeviceStatusEntry
{
    /// <summary>
    /// The sub-message type that produced this entry.
    /// </summary>
    public AoipStatusMessageType MessageType { get; set; }

    /// <summary>
    /// Slot number of the card.
    /// </summary>
    public byte SlotNumber { get; set; }

    /// <summary>
    /// Device type reported by the card.
    /// </summary>
    public AoipDeviceType DeviceType { get; set; }

    /// <summary>
    /// Device identifier (big-endian 2 bytes).
    /// </summary>
    public ushort DeviceId { get; set; }

    /// <summary>
    /// General state flag. Meaning depends on sub-message type
    /// (e.g. PTP locked, API reachable, etc.).
    /// </summary>
    public bool State { get; set; }

    // ── PTP Status fields ──

    /// <summary>PTP Grand Master clock ID (null-terminated string, max 35 bytes). PtpStatus / PtpStatusSecondary only.</summary>
    public string? MasterId { get; set; }

    /// <summary>PTP local clock ID (null-terminated string, max 35 bytes). PtpStatus / PtpStatusSecondary only.</summary>
    public string? LocalId { get; set; }

    /// <summary>PTP role status (e.g. "Leader", "Follower", "Uncalibrated"). PtpStatus / PtpStatusSecondary only.</summary>
    public string? RoleStatus { get; set; }

    /// <summary>PTP lock status (e.g. "Locked", "Unlocked", "Uncalibrated"). PtpStatus / PtpStatusSecondary only.</summary>
    public string? LockStatus { get; set; }

    /// <summary>Grand Master priority 1 (network byte order). PtpStatus / PtpStatusSecondary only.</summary>
    public uint? MasterPriority1 { get; set; }

    /// <summary>Grand Master priority 2 (network byte order). PtpStatus / PtpStatusSecondary only.</summary>
    public uint? MasterPriority2 { get; set; }

    /// <summary>Local priority 1 (network byte order). PtpStatus / PtpStatusSecondary only.</summary>
    public uint? LocalPriority1 { get; set; }

    /// <summary>Local priority 2 (network byte order). PtpStatus / PtpStatusSecondary only.</summary>
    public uint? LocalPriority2 { get; set; }

    /// <summary>Mean path delay in nanoseconds. PtpStatus / PtpStatusSecondary / PtpStatistics / PtpStatisticsSecondary.</summary>
    public long? MeanPathDelay { get; set; }

    /// <summary>Offset from master (raw, signed). PtpStatus / PtpStatusSecondary / PtpStatistics / PtpStatisticsSecondary.</summary>
    public long? OffsetFromMasterRaw { get; set; }

    // ── PTP Statistics fields ──

    /// <summary>Offset from master RMS. PtpStatistics / PtpStatisticsSecondary only.</summary>
    public long? OffsetFromMasterRms { get; set; }

    /// <summary>Offset from master max. PtpStatistics / PtpStatisticsSecondary only.</summary>
    public long? OffsetFromMasterMax { get; set; }

    /// <summary>Frequency mean. PtpStatistics / PtpStatisticsSecondary only.</summary>
    public long? FreqMean { get; set; }

    /// <summary>Frequency standard deviation. PtpStatistics / PtpStatisticsSecondary only.</summary>
    public long? FreqStdDev { get; set; }

    /// <summary>Mean path delay standard deviation. PtpStatistics / PtpStatisticsSecondary only.</summary>
    public long? MeanPathDelayStdDev { get; set; }

    /// <summary>Variance (schema 2 only). PtpStatistics / PtpStatisticsSecondary only.</summary>
    public long? Variance { get; set; }

    /// <summary>Date/time this statistics message was received. PtpStatistics / PtpStatisticsSecondary only.</summary>
    public DateTime? ReceivedDateTime { get; set; }

    // ── Generic raw data (for unhandled sub-types) ──

    /// <summary>
    /// Raw sub-message bytes for sub-types that do not yet have a dedicated parser
    /// (Aes67RestApiStatus, IvpConnectionStatus, DectSyncStatus, DeviceIpDetails, WifiChannels).
    /// </summary>
    public byte[]? RawSubMessageData { get; set; }
}

/// <summary>
/// Reply AoIP Device Status (Message ID 0x016B / 363).
/// Contains one or more sub-typed status messages about AoIP devices in the matrix.
/// </summary>
public class ReplyAoipDeviceStatus
{
    /// <summary>
    /// Schema version read from the payload header.
    /// </summary>
    public byte Schema { get; set; }

    /// <summary>
    /// Number of sub-messages declared in the header.
    /// </summary>
    public byte NumberOfMessages { get; set; }

    /// <summary>
    /// Parsed sub-message entries.
    /// </summary>
    public List<AoipDeviceStatusEntry> Entries { get; set; } = new();

    /// <summary>
    /// Decodes a Reply AoIP Device Status message payload.
    /// </summary>
    /// <param name="payload">The message payload (after the HCIv2 protocol tag + schema byte).</param>
    /// <param name="schema">The schema version from the HCIv2 header.</param>
    /// <returns>The decoded result, or null if the payload is too short.</returns>
    public static ReplyAoipDeviceStatus? Decode(byte[] payload, byte schema)
    {
        // Minimum: version(1) + numberOfMessages(1) = 2
        if (payload == null || payload.Length < 2)
            return null;

        int offset = 0;

        var result = new ReplyAoipDeviceStatus();

        // Version / schema byte embedded in the sub-message stream
        result.Schema = payload[offset++];

        // Validate that the version byte is one of the known schemas
        if (result.Schema != 1 && result.Schema != 2)
            return null;

        result.NumberOfMessages = payload[offset++];

        for (int i = 0; i < result.NumberOfMessages; i++)
        {
            if (offset >= payload.Length)
                break;

            var entry = ParseSubMessage(payload, ref offset, result.Schema);
            if (entry != null)
                result.Entries.Add(entry);
        }

        return result;
    }

    /// <summary>
    /// Parses a single AoIP status sub-message from the payload.
    /// </summary>
    private static AoipDeviceStatusEntry? ParseSubMessage(byte[] payload, ref int offset, byte schema)
    {
        // Each sub-message starts with: length(1) + messageType(1) + timestamp(4) + fractionalSeconds(2)
        if (offset + 8 > payload.Length)
            return null;

        byte length = payload[offset++];
        byte messageTypeByte = payload[offset++];
        var messageType = Enum.IsDefined(typeof(AoipStatusMessageType), messageTypeByte)
            ? (AoipStatusMessageType)messageTypeByte
            : (AoipStatusMessageType)messageTypeByte;

        // Skip timestamp (4 bytes) + fractional seconds (2 bytes)
        offset += 6;

        // Remaining bytes for this sub-message = length - 1 (messageType) - 6 (timestamps)
        // length includes everything after the length byte itself
        int dataLength = length - 7; // length byte covers: msgType(1) + ts(4) + frac(2) + data
        if (dataLength < 0 || offset + dataLength > payload.Length)
            return null;

        int dataStart = offset;

        AoipDeviceStatusEntry? entry = messageType switch
        {
            AoipStatusMessageType.PtpStatus => ParsePtpStatus(payload, ref offset),
            AoipStatusMessageType.PtpStatusSecondary => ParsePtpStatus(payload, ref offset),
            AoipStatusMessageType.PtpStatistics => ParsePtpStatistics(payload, ref offset, schema),
            AoipStatusMessageType.PtpStatisticsSecondary => ParsePtpStatistics(payload, ref offset, schema),
            _ => ParseGenericSubMessage(payload, ref offset, dataLength),
        };

        if (entry != null)
            entry.MessageType = messageType;

        // Ensure offset advances to the end of this sub-message regardless of how much the parser consumed
        offset = dataStart + dataLength;

        return entry;
    }

    /// <summary>
    /// Parses PTP Status (type 0) and PTP Status Secondary (type 7) sub-messages.
    /// </summary>
    private static AoipDeviceStatusEntry? ParsePtpStatus(byte[] payload, ref int offset)
    {
        // Minimum: slot(1) + deviceType(1) + deviceId(2) + state(1) + masterId(35) + localId(35) + role(15) + lock(15) + priorities(4*4) + delays(4+4) = 125 bytes
        if (offset + 125 > payload.Length)
            return null;

        var entry = new AoipDeviceStatusEntry
        {
            SlotNumber = payload[offset++],
            DeviceType = (AoipDeviceType)payload[offset++],
            DeviceId = ReadUInt16BE(payload, ref offset),
            State = payload[offset++] != 0,
        };

        // Master ID: 35-byte null-terminated ASCII string
        entry.MasterId = ReadNullTerminatedString(payload, ref offset, 35);

        // Local ID: 35-byte null-terminated ASCII string
        entry.LocalId = ReadNullTerminatedString(payload, ref offset, 35);

        // Role Status: 15-byte null-terminated ASCII string
        entry.RoleStatus = ReadNullTerminatedString(payload, ref offset, 15);

        // Normalise legacy terminology
        if (entry.RoleStatus == "Slave")
            entry.RoleStatus = "Follower";
        else if (entry.RoleStatus == "Master")
            entry.RoleStatus = "Leader";

        // Lock Status: 15-byte null-terminated ASCII string
        entry.LockStatus = ReadNullTerminatedString(payload, ref offset, 15);

        // Normalise case: title-case the lock status
        if (!string.IsNullOrWhiteSpace(entry.LockStatus))
        {
            entry.LockStatus = char.ToUpper(entry.LockStatus[0]) + entry.LockStatus[1..].ToLower();
        }
        else
        {
            entry.LockStatus = entry.RoleStatus == "Uncalibrated" ? "Uncalibrated" : "Unknown";
        }

        // Priorities and delays (4 bytes each, network byte order)
        entry.MasterPriority1 = ReadUInt32BE(payload, ref offset);
        entry.MasterPriority2 = ReadUInt32BE(payload, ref offset);
        entry.LocalPriority1 = ReadUInt32BE(payload, ref offset);
        entry.LocalPriority2 = ReadUInt32BE(payload, ref offset);
        entry.MeanPathDelay = ReadInt32BE(payload, ref offset);
        entry.OffsetFromMasterRaw = ReadInt32BE(payload, ref offset);

        return entry;
    }

    /// <summary>
    /// Parses PTP Statistics (type 5) and PTP Statistics Secondary (type 8) sub-messages.
    /// </summary>
    private static AoipDeviceStatusEntry? ParsePtpStatistics(byte[] payload, ref int offset, byte schema)
    {
        // Minimum: slot(1) + deviceType(1) + deviceId(2) + 6*Int64(48) = 52 bytes
        int minSize = 52;
        if (schema == 2)
            minSize += 16; // + ofmRaw(8) + variance(8)

        if (offset + minSize > payload.Length)
            return null;

        var entry = new AoipDeviceStatusEntry
        {
            SlotNumber = payload[offset++],
            DeviceType = (AoipDeviceType)payload[offset++],
            DeviceId = ReadUInt16BE(payload, ref offset),
        };

        long ofmRms = ReadInt64LE(payload, ref offset);
        long ofmMax = ReadInt64LE(payload, ref offset);
        long freqMean = ReadInt64LE(payload, ref offset);
        long freqStddev = ReadInt64LE(payload, ref offset);
        long delayMean = ReadInt64LE(payload, ref offset);
        long delayStddev = ReadInt64LE(payload, ref offset);

        if (schema == 2)
        {
            entry.OffsetFromMasterRaw = ReadInt64LE(payload, ref offset);
            entry.Variance = ReadInt64LE(payload, ref offset);
        }

        entry.ReceivedDateTime = DateTime.UtcNow;
        entry.MeanPathDelay = delayMean;
        entry.MeanPathDelayStdDev = delayStddev;
        entry.OffsetFromMasterRms = ofmRms;
        entry.OffsetFromMasterMax = ofmMax;
        entry.FreqMean = freqMean;
        entry.FreqStdDev = freqStddev;

        return entry;
    }

    /// <summary>
    /// Generic fallback parser that captures the raw bytes for sub-types
    /// without dedicated parsing (AES67, IVP, DECT, IP Details, Wi-Fi).
    /// </summary>
    private static AoipDeviceStatusEntry? ParseGenericSubMessage(byte[] payload, ref int offset, int dataLength)
    {
        if (dataLength < 4 || offset + dataLength > payload.Length)
            return null;

        var entry = new AoipDeviceStatusEntry
        {
            SlotNumber = payload[offset++],
            DeviceType = (AoipDeviceType)payload[offset++],
            DeviceId = ReadUInt16BE(payload, ref offset),
        };

        int remaining = dataLength - 4;
        if (remaining > 0)
        {
            entry.RawSubMessageData = new byte[remaining];
            Array.Copy(payload, offset, entry.RawSubMessageData, 0, remaining);
            offset += remaining;
        }

        return entry;
    }

    // ── Read helpers ──

    private static ushort ReadUInt16BE(byte[] data, ref int offset)
    {
        ushort value = (ushort)((data[offset] << 8) | data[offset + 1]);
        offset += 2;
        return value;
    }

    private static uint ReadUInt32BE(byte[] data, ref int offset)
    {
        uint value = (uint)((data[offset] << 24) | (data[offset + 1] << 16) |
                            (data[offset + 2] << 8) | data[offset + 3]);
        offset += 4;
        return value;
    }

    private static int ReadInt32BE(byte[] data, ref int offset)
    {
        int value = (data[offset] << 24) | (data[offset + 1] << 16) |
                    (data[offset + 2] << 8) | data[offset + 3];
        offset += 4;
        return value;
    }

    private static long ReadInt64LE(byte[] data, ref int offset)
    {
        long value = BitConverter.ToInt64(data, offset);
        offset += 8;
        return value;
    }

    private static string ReadNullTerminatedString(byte[] data, ref int offset, int fieldLength)
    {
        if (offset + fieldLength > data.Length)
        {
            offset += fieldLength;
            return string.Empty;
        }

        string raw = Encoding.ASCII.GetString(data, offset, fieldLength);
        offset += fieldLength;

        int nullPos = raw.IndexOf('\0');
        return nullPos >= 0 ? raw[..nullPos] : raw;
    }
}
