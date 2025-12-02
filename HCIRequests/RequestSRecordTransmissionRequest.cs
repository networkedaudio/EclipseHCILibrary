using HCILibrary.Enums;
using HCILibrary.Models;
using System.Text;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request SRecord Transmission (0x0045).
/// Sends a single SRecord line to the matrix during an SRecord transfer session.
/// Each line from the SRecord file should be sent as a separate message.
/// HCIv2 only.
/// </summary>
/// <remarks>
/// This message uses the same Message ID (0x0045) as <see cref="RequestSRecordTransmissionInitiationRequest"/>.
/// The matrix distinguishes between them by the first byte after the length field:
/// - Initiation: Sub Message ID = 5
/// - Transmission: First byte of SRecord = 'S' (0x53 = 83)
/// </remarks>
public class RequestSRecordTransmissionRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Gets or sets the protocol schema version. Currently set to 1.
    /// </summary>
    public byte ProtocolSchema { get; set; } = 1;

    /// <summary>
    /// Gets or sets the SRecord line data (ASCII string starting with 'S').
    /// </summary>
    public string SRecord { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestSRecordTransmissionRequest"/> class.
    /// </summary>
    public RequestSRecordTransmissionRequest()
        : base(HCIMessageID.RequestSRecordTransmissionInitiation) // Same message ID
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestSRecordTransmissionRequest"/> class
    /// with the specified SRecord line.
    /// </summary>
    /// <param name="sRecord">The SRecord line (ASCII string starting with 'S').</param>
    public RequestSRecordTransmissionRequest(string sRecord)
        : base(HCIMessageID.RequestSRecordTransmissionInitiation) // Same message ID
    {
        SRecord = sRecord ?? string.Empty;
    }

    /// <summary>
    /// Creates a transmission request from raw SRecord bytes.
    /// </summary>
    /// <param name="sRecordBytes">The SRecord line as ASCII bytes.</param>
    /// <returns>A new RequestSRecordTransmissionRequest.</returns>
    public static RequestSRecordTransmissionRequest FromBytes(byte[] sRecordBytes)
    {
        return new RequestSRecordTransmissionRequest(Encoding.ASCII.GetString(sRecordBytes));
    }

    /// <summary>
    /// Validates that the SRecord starts with 'S' as required by the Motorola SRecord format.
    /// </summary>
    /// <returns>True if the SRecord is valid, false otherwise.</returns>
    public bool IsValidSRecord()
    {
        return !string.IsNullOrEmpty(SRecord) && SRecord.StartsWith('S');
    }

    /// <summary>
    /// Gets the SRecord type (the character after 'S', e.g., '0', '1', '2', '3', '7', '8', '9').
    /// </summary>
    /// <returns>The SRecord type character, or null if invalid.</returns>
    public char? GetSRecordType()
    {
        if (SRecord?.Length >= 2 && SRecord[0] == 'S')
        {
            return SRecord[1];
        }
        return null;
    }

    /// <summary>
    /// Generates the payload for the Request SRecord Transmission message.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // [0-3]:   HCIv2 Protocol Tag (0xAB 0xBA 0xCE 0xDE)
        // [4]:     Protocol Schema (1 byte)
        // [5-6]:   Length (2 bytes, big-endian) - payload length from start of length field
        // [7+]:    SRecord data (variable length ASCII)

        byte[] sRecordBytes = Encoding.ASCII.GetBytes(SRecord);
        
        // Length field covers: Length(2) + SRecord data
        ushort payloadLength = (ushort)(2 + sRecordBytes.Length);
        int totalSize = 4 + 1 + 2 + sRecordBytes.Length; // Tag + Schema + Length + SRecord

        var payload = new byte[totalSize];
        int offset = 0;

        // HCIv2 protocol tag
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol schema
        payload[offset++] = ProtocolSchema;

        // Length (2 bytes, big-endian)
        payload[offset++] = (byte)((payloadLength >> 8) & 0xFF);
        payload[offset++] = (byte)(payloadLength & 0xFF);

        // SRecord data
        Array.Copy(sRecordBytes, 0, payload, offset, sRecordBytes.Length);

        return payload;
    }
}
