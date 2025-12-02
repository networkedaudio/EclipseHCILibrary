using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Remote Key Actions (0x00EB).
/// Allows the host to remotely assign keys on a panel.
/// </summary>
public class RequestRemoteKeyActionsRequest : HCIRequest
{
    /// <summary>
    /// Assignment type (currently only ActionType1 is supported).
    /// </summary>
    public RemoteKeyAssignmentType AssignmentType { get; set; } = RemoteKeyAssignmentType.ActionType1;

    /// <summary>
    /// Port number of target panel for key assignment.
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// List of remote key action entries.
    /// </summary>
    public List<RemoteKeyAction> Actions { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestRemoteKeyActionsRequest"/> class.
    /// </summary>
    public RequestRemoteKeyActionsRequest()
        : base(HCIMessageID.RequestRemoteKeyActions)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestRemoteKeyActionsRequest"/> class
    /// with a specified target panel port.
    /// </summary>
    /// <param name="portNumber">Port number of target panel.</param>
    public RequestRemoteKeyActionsRequest(ushort portNumber)
        : base(HCIMessageID.RequestRemoteKeyActions)
    {
        PortNumber = portNumber;
    }

    /// <summary>
    /// Generates the payload for the Request Remote Key Actions message.
    /// </summary>
    /// <returns>The message payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload: Type(1) + Count(2) + PortNumber(2) + Actions(12 * count)
        const int actionSize = 12;
        int totalSize = 1 + 2 + 2 + (Actions.Count * actionSize);

        var payload = new byte[totalSize];
        int offset = 0;

        // Type: 1 byte
        payload[offset++] = (byte)AssignmentType;

        // Count: 2 bytes (big-endian)
        payload[offset++] = (byte)((Actions.Count >> 8) & 0xFF);
        payload[offset++] = (byte)(Actions.Count & 0xFF);

        // Port Number: 2 bytes (big-endian)
        payload[offset++] = (byte)((PortNumber >> 8) & 0xFF);
        payload[offset++] = (byte)(PortNumber & 0xFF);

        // Action entries
        foreach (var action in Actions)
        {
            var actionBytes = action.ToBytes();
            Array.Copy(actionBytes, 0, payload, offset, actionBytes.Length);
            offset += actionBytes.Length;
        }

        return payload;
    }

    /// <summary>
    /// Adds a remote key action entry.
    /// </summary>
    /// <param name="action">The remote key action to add.</param>
    public void AddAction(RemoteKeyAction action)
    {
        Actions.Add(action);
    }

    /// <summary>
    /// Adds a port assignment to a key.
    /// </summary>
    /// <param name="region">Region of the key.</param>
    /// <param name="page">Page of the key.</param>
    /// <param name="key">Key position.</param>
    /// <param name="systemNumber">System number of the port.</param>
    /// <param name="portNumber">Port number to assign.</param>
    /// <param name="activation">Key activation type.</param>
    public void AddPortAssignment(byte region, byte page, byte key, byte systemNumber, ushort portNumber,
        KeyActivationType activation = KeyActivationType.TalkAndListen)
    {
        Actions.Add(new RemoteKeyAction
        {
            Region = region,
            Page = page,
            Key = key,
            EntityType = KeyEntityType.Port,
            EntityReference = KeyEntityReference.ForPort(systemNumber, portNumber),
            KeyActivation = activation
        });
    }

    /// <summary>
    /// Adds a conference assignment to a key.
    /// </summary>
    /// <param name="region">Region of the key.</param>
    /// <param name="page">Page of the key.</param>
    /// <param name="key">Key position.</param>
    /// <param name="systemNumber">System number of the conference.</param>
    /// <param name="conferenceNumber">Conference number to assign.</param>
    /// <param name="activation">Key activation type.</param>
    public void AddConferenceAssignment(byte region, byte page, byte key, byte systemNumber, ushort conferenceNumber,
        KeyActivationType activation = KeyActivationType.TalkAndListen)
    {
        Actions.Add(new RemoteKeyAction
        {
            Region = region,
            Page = page,
            Key = key,
            EntityType = KeyEntityType.Conference,
            EntityReference = KeyEntityReference.ForConference(systemNumber, conferenceNumber),
            KeyActivation = activation
        });
    }

    /// <summary>
    /// Adds a fixed group assignment to a key.
    /// </summary>
    /// <param name="region">Region of the key.</param>
    /// <param name="page">Page of the key.</param>
    /// <param name="key">Key position.</param>
    /// <param name="systemNumber">System number of the group.</param>
    /// <param name="groupNumber">Group number to assign.</param>
    /// <param name="activation">Key activation type.</param>
    public void AddGroupAssignment(byte region, byte page, byte key, byte systemNumber, ushort groupNumber,
        KeyActivationType activation = KeyActivationType.TalkAndListen)
    {
        Actions.Add(new RemoteKeyAction
        {
            Region = region,
            Page = page,
            Key = key,
            EntityType = KeyEntityType.Group,
            EntityReference = KeyEntityReference.ForGroup(systemNumber, groupNumber),
            KeyActivation = activation
        });
    }

    /// <summary>
    /// Adds an IFB assignment to a key.
    /// </summary>
    /// <param name="region">Region of the key.</param>
    /// <param name="page">Page of the key.</param>
    /// <param name="key">Key position.</param>
    /// <param name="systemNumber">System number of the IFB.</param>
    /// <param name="ifbNumber">IFB number to assign.</param>
    /// <param name="activation">Key activation type.</param>
    public void AddIfbAssignment(byte region, byte page, byte key, byte systemNumber, byte ifbNumber,
        KeyActivationType activation = KeyActivationType.TalkAndListen)
    {
        Actions.Add(new RemoteKeyAction
        {
            Region = region,
            Page = page,
            Key = key,
            EntityType = KeyEntityType.Ifb,
            EntityReference = KeyEntityReference.ForIfb(systemNumber, ifbNumber),
            KeyActivation = activation
        });
    }

    /// <summary>
    /// Removes an assignment from a key.
    /// </summary>
    /// <param name="region">Region of the key.</param>
    /// <param name="page">Page of the key.</param>
    /// <param name="key">Key position.</param>
    public void RemoveAssignment(byte region, byte page, byte key)
    {
        Actions.Add(new RemoteKeyAction
        {
            Region = region,
            Page = page,
            Key = key,
            EntityType = KeyEntityType.Null,
            EntityReference = new KeyEntityReference()
        });
    }
}
