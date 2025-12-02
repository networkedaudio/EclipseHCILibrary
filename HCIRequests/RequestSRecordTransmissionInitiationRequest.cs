using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request SRecord Transmission Initiation (0x0045).
/// Initiates an SRecord transfer to the matrix for configuration or firmware updates.
/// HCIv2 only.
/// </summary>
public class RequestSRecordTransmissionInitiationRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Sub Message ID for SRecord transmission initiation.
    /// </summary>
    private const byte SubMessageId = 5;

    /// <summary>
    /// Gets or sets the protocol schema version. Currently set to 1.
    /// </summary>
    public byte ProtocolSchema { get; set; } = 1;

    /// <summary>
    /// Gets or sets the apply type specifying how the configuration should be applied.
    /// </summary>
    public SRecordApplyType ApplyType { get; set; } = SRecordApplyType.ApplyChanges;

    /// <summary>
    /// Gets or sets the partial NV clear options.
    /// Specifies which local changes should be cleared during a reset.
    /// Only applicable when ApplyType is ApplyWithReset or ApplyWithResetAndNvramClear.
    /// </summary>
    public PartialNvClearOptions PartialNvClearOptions { get; set; } = PartialNvClearOptions.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestSRecordTransmissionInitiationRequest"/> class
    /// with the default apply type (Apply Changes - no reset).
    /// </summary>
    public RequestSRecordTransmissionInitiationRequest()
        : base(HCIMessageID.RequestSRecordTransmissionInitiation)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestSRecordTransmissionInitiationRequest"/> class
    /// with the specified apply type.
    /// </summary>
    /// <param name="applyType">The apply type specifying how the configuration should be applied.</param>
    public RequestSRecordTransmissionInitiationRequest(SRecordApplyType applyType)
        : base(HCIMessageID.RequestSRecordTransmissionInitiation)
    {
        ApplyType = applyType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestSRecordTransmissionInitiationRequest"/> class
    /// with the specified apply type and partial NV clear options.
    /// </summary>
    /// <param name="applyType">The apply type specifying how the configuration should be applied.</param>
    /// <param name="partialNvClearOptions">The partial NV clear options.</param>
    public RequestSRecordTransmissionInitiationRequest(SRecordApplyType applyType, PartialNvClearOptions partialNvClearOptions)
        : base(HCIMessageID.RequestSRecordTransmissionInitiation)
    {
        ApplyType = applyType;
        PartialNvClearOptions = partialNvClearOptions;
    }

    /// <summary>
    /// Creates a request for Apply with Reset (Red reset).
    /// </summary>
    /// <param name="partialNvClearOptions">Optional partial NV clear options.</param>
    /// <returns>A new request configured for Apply with Reset.</returns>
    public static RequestSRecordTransmissionInitiationRequest CreateApplyWithReset(
        PartialNvClearOptions partialNvClearOptions = PartialNvClearOptions.None)
    {
        return new RequestSRecordTransmissionInitiationRequest(SRecordApplyType.ApplyWithReset, partialNvClearOptions);
    }

    /// <summary>
    /// Creates a request for Apply Changes (no reset).
    /// </summary>
    /// <returns>A new request configured for Apply Changes.</returns>
    public static RequestSRecordTransmissionInitiationRequest CreateApplyChanges()
    {
        return new RequestSRecordTransmissionInitiationRequest(SRecordApplyType.ApplyChanges);
    }

    /// <summary>
    /// Creates a request for Apply with Reset and NVRAM clear (Black reset).
    /// </summary>
    /// <param name="partialNvClearOptions">Optional partial NV clear options.</param>
    /// <returns>A new request configured for Apply with Reset and NVRAM clear.</returns>
    public static RequestSRecordTransmissionInitiationRequest CreateApplyWithResetAndNvramClear(
        PartialNvClearOptions partialNvClearOptions = PartialNvClearOptions.None)
    {
        return new RequestSRecordTransmissionInitiationRequest(SRecordApplyType.ApplyWithResetAndNvramClear, partialNvClearOptions);
    }

    /// <summary>
    /// Generates the payload for the Request SRecord Transmission Initiation message.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // [0-3]:   HCIv2 Protocol Tag (0xAB 0xBA 0xCE 0xDE)
        // [4]:     Protocol Schema (1 byte)
        // [5-6]:   Length (2 bytes, big-endian) - payload length from start of length field
        // [7]:     Sub Message ID (1 byte) - always 5
        // [8]:     Apply Type (1 byte)
        // [9-17]:  Reserved (9 bytes)
        // [18-19]: Partial NV Clear Options (2 bytes, big-endian)

        // Length field covers: Length(2) + SubMessageID(1) + ApplyType(1) + Reserved(9) + PartialNvClear(2) = 15 bytes
        const ushort payloadLength = 15;
        const int totalSize = 4 + 1 + 2 + 1 + 1 + 9 + 2; // Tag + Schema + Length + SubMsgID + ApplyType + Reserved + PartialNvClear = 20

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

        // Sub Message ID
        payload[offset++] = SubMessageId;

        // Apply Type
        payload[offset++] = (byte)ApplyType;

        // Reserved (9 bytes) - already zeroed
        offset += 9;

        // Partial NV Clear Options (2 bytes, big-endian)
        ushort nvClearOptions = (ushort)PartialNvClearOptions;
        payload[offset++] = (byte)((nvClearOptions >> 8) & 0xFF);
        payload[offset++] = (byte)(nvClearOptions & 0xFF);

        return payload;
    }
}
