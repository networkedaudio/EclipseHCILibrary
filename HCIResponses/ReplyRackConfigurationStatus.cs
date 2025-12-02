using HCILibrary.Models;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Rack Configuration Status (HCIv2) - Message ID 0x002C (44), Sub ID 13.
/// Contains the configuration status from the matrix.
/// </summary>
public class ReplyRackConfigurationStatus
{
    /// <summary>
    /// Protocol schema version.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// Sub Message ID (should be 13).
    /// </summary>
    public byte SubId { get; set; }

    /// <summary>
    /// Length of entry payload.
    /// </summary>
    public ushort EntryLength { get; set; }

    /// <summary>
    /// Matrix/system number.
    /// </summary>
    public byte MatrixId { get; set; }

    /// <summary>
    /// Download timestamp - Year.
    /// </summary>
    public byte DownloadYear { get; set; }

    /// <summary>
    /// Download timestamp - Month.
    /// </summary>
    public byte DownloadMonth { get; set; }

    /// <summary>
    /// Download timestamp - Day.
    /// </summary>
    public byte DownloadDay { get; set; }

    /// <summary>
    /// Download timestamp - Hour.
    /// </summary>
    public byte DownloadHour { get; set; }

    /// <summary>
    /// Download timestamp - Minute.
    /// </summary>
    public byte DownloadMin { get; set; }

    /// <summary>
    /// Download timestamp - Second.
    /// </summary>
    public byte DownloadSec { get; set; }

    /// <summary>
    /// Download timestamp - Milliseconds.
    /// </summary>
    public byte DownloadMsec { get; set; }

    /// <summary>
    /// System identity (18 bytes).
    /// </summary>
    public byte[] SystemIdentity { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// System identity as a string (trimmed).
    /// </summary>
    public string SystemIdentityString => System.Text.Encoding.ASCII.GetString(SystemIdentity).TrimEnd('\0');

    /// <summary>
    /// Download flags.
    /// </summary>
    public byte DownloadFlags { get; set; }

    /// <summary>
    /// Map format (5 bytes).
    /// </summary>
    public byte[] MapFormat { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Checksum (2 x 16-bit words = 4 bytes).
    /// </summary>
    public uint Checksum { get; set; }

    /// <summary>
    /// Matrix description (20 bytes).
    /// </summary>
    public byte[] MatrixDescription { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Matrix description as a string (trimmed).
    /// </summary>
    public string MatrixDescriptionString => System.Text.Encoding.ASCII.GetString(MatrixDescription).TrimEnd('\0');

    /// <summary>
    /// HCI license.
    /// </summary>
    public byte HciLicense { get; set; }

    /// <summary>
    /// HCI extended license.
    /// </summary>
    public byte HciExtendedLicense { get; set; }

    /// <summary>
    /// Virtual client users.
    /// </summary>
    public byte VirtualClientUsers { get; set; }

    /// <summary>
    /// Dynam-EC licenses.
    /// </summary>
    public byte DynamEcLicenses { get; set; }

    /// <summary>
    /// Delta Lite extension license.
    /// </summary>
    public byte DeltaLiteExtensionLicense { get; set; }

    /// <summary>
    /// Gets the download timestamp as a DateTime.
    /// </summary>
    public DateTime? DownloadTimestamp
    {
        get
        {
            try
            {
                // Year is stored as offset from 2000 or as full year depending on implementation
                int year = DownloadYear < 100 ? 2000 + DownloadYear : DownloadYear;
                return new DateTime(year, DownloadMonth, DownloadDay, DownloadHour, DownloadMin, DownloadSec, DownloadMsec);
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Decodes the payload into a ReplyRackConfigurationStatus.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyRackConfigurationStatus Decode(byte[] payload)
    {
        var reply = new ReplyRackConfigurationStatus();

        if (payload.Length < 7)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip
        offset += 4;

        // Protocol Schema: 1 byte
        reply.ProtocolSchema = payload[offset++];

        // Sub ID: 1 byte
        reply.SubId = payload[offset++];

        // Entry Length: 2 bytes (big-endian)
        if (offset + 2 > payload.Length)
            return reply;
        reply.EntryLength = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Matrix ID: 1 byte
        if (offset >= payload.Length)
            return reply;
        reply.MatrixId = payload[offset++];

        // Download Year: 1 byte
        if (offset >= payload.Length)
            return reply;
        reply.DownloadYear = payload[offset++];

        // Download Month: 1 byte
        if (offset >= payload.Length)
            return reply;
        reply.DownloadMonth = payload[offset++];

        // Download Day: 1 byte
        if (offset >= payload.Length)
            return reply;
        reply.DownloadDay = payload[offset++];

        // Download Hour: 1 byte
        if (offset >= payload.Length)
            return reply;
        reply.DownloadHour = payload[offset++];

        // Download Min: 1 byte
        if (offset >= payload.Length)
            return reply;
        reply.DownloadMin = payload[offset++];

        // Download Sec: 1 byte
        if (offset >= payload.Length)
            return reply;
        reply.DownloadSec = payload[offset++];

        // Download Msec: 1 byte
        if (offset >= payload.Length)
            return reply;
        reply.DownloadMsec = payload[offset++];

        // System Identity: 18 bytes
        if (offset + 18 > payload.Length)
            return reply;
        reply.SystemIdentity = new byte[18];
        Array.Copy(payload, offset, reply.SystemIdentity, 0, 18);
        offset += 18;

        // Download Flags: 1 byte
        if (offset >= payload.Length)
            return reply;
        reply.DownloadFlags = payload[offset++];

        // Map Format: 5 bytes
        if (offset + 5 > payload.Length)
            return reply;
        reply.MapFormat = new byte[5];
        Array.Copy(payload, offset, reply.MapFormat, 0, 5);
        offset += 5;

        // Checksum: 2 x 16-bit words (4 bytes total, big-endian)
        if (offset + 4 > payload.Length)
            return reply;
        reply.Checksum = (uint)((payload[offset] << 24) | (payload[offset + 1] << 16) |
                                (payload[offset + 2] << 8) | payload[offset + 3]);
        offset += 4;

        // Matrix Description: 20 bytes
        if (offset + 20 > payload.Length)
            return reply;
        reply.MatrixDescription = new byte[20];
        Array.Copy(payload, offset, reply.MatrixDescription, 0, 20);
        offset += 20;

        // HCI License: 1 byte
        if (offset >= payload.Length)
            return reply;
        reply.HciLicense = payload[offset++];

        // HCI Extended License: 1 byte
        if (offset >= payload.Length)
            return reply;
        reply.HciExtendedLicense = payload[offset++];

        // Virtual Client Users: 1 byte
        if (offset >= payload.Length)
            return reply;
        reply.VirtualClientUsers = payload[offset++];

        // Dynam-EC Licenses: 1 byte
        if (offset >= payload.Length)
            return reply;
        reply.DynamEcLicenses = payload[offset++];

        // Delta Lite Extension License: 1 byte
        if (offset >= payload.Length)
            return reply;
        reply.DeltaLiteExtensionLicense = payload[offset++];

        return reply;
    }
}
