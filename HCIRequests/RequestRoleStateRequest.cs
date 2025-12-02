using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Role State (HCIv2) - Message ID 0x0184 (388).
/// This message is used to request the current status of a matrix role.
/// </summary>
public class RequestRoleStateRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Value to request all roles.
    /// </summary>
    public const ushort AllRoles = 0xFFFF;

    /// <summary>
    /// Role number to request, or 0xFFFF for all roles.
    /// </summary>
    public ushort Role { get; set; } = AllRoles;

    /// <summary>
    /// Creates a new Request Role State for all roles.
    /// </summary>
    public RequestRoleStateRequest()
        : base(HCIMessageID.RequestRoleState)
    {
    }

    /// <summary>
    /// Creates a new Request Role State for a specific role.
    /// </summary>
    /// <param name="role">Role number, or 0xFFFF for all roles.</param>
    public RequestRoleStateRequest(ushort role)
        : base(HCIMessageID.RequestRoleState)
    {
        Role = role;
    }

    /// <summary>
    /// Generates the payload for the request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // Protocol Tag: 4 bytes (0xABBACEDE)
        // Protocol Schema: 1 byte (set to 1)
        // Role: 2 bytes (big-endian), 0xFFFF for all roles

        using var ms = new MemoryStream();

        // Protocol Tag: 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol Schema: 1 byte
        ms.WriteByte(0x01);

        // Role: 2 bytes (big-endian)
        ms.WriteByte((byte)(Role >> 8));
        ms.WriteByte((byte)(Role & 0xFF));

        return ms.ToArray();
    }
}
