using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Remote Key Action Status (0x00ED).
/// Causes the matrix to generate a message containing all of the key assignment actions.
/// Allows all remote assignments to be retrieved for all panels or for a specific panel.
/// </summary>
public class RequestRemoteKeyActionStatusRequest : HCIRequest
{
    /// <summary>
    /// Protocol schema version (1 or 2).
    /// Schema 2 includes endpoint type in the reply.
    /// </summary>
    public byte Schema { get; set; } = 1;

    /// <summary>
    /// Assignment type (currently only ActionType1 is supported).
    /// </summary>
    public RemoteKeyAssignmentType AssignmentType { get; set; } = RemoteKeyAssignmentType.ActionType1;

    /// <summary>
    /// Port number of target panel.
    /// Targets response to the specified panel.
    /// Only supports a request for a single panel at a time.
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestRemoteKeyActionStatusRequest"/> class.
    /// </summary>
    public RequestRemoteKeyActionStatusRequest()
        : base(HCIMessageID.RequestRemoteKeyActionStatus)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestRemoteKeyActionStatusRequest"/> class
    /// with a specified target panel port.
    /// </summary>
    /// <param name="portNumber">Port number of target panel.</param>
    /// <param name="schema">Protocol schema version (1 or 2).</param>
    public RequestRemoteKeyActionStatusRequest(ushort portNumber, byte schema = 1)
        : base(HCIMessageID.RequestRemoteKeyActionStatus)
    {
        PortNumber = portNumber;
        Schema = schema;
    }

    /// <summary>
    /// Generates the payload for the Request Remote Key Action Status message.
    /// </summary>
    /// <returns>The message payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload: Type(1) + PortNumber(2) = 3 bytes
        var payload = new byte[3];
        int offset = 0;

        // Type: 1 byte
        payload[offset++] = (byte)AssignmentType;

        // Port Number: 2 bytes (big-endian)
        payload[offset++] = (byte)((PortNumber >> 8) & 0xFF);
        payload[offset++] = (byte)(PortNumber & 0xFF);

        return payload;
    }
}
