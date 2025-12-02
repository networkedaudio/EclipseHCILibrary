using HCILibrary.Enums;
using HCILibrary.Models;
using System.Text;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Beltpack Add (HCIv2) - Message ID 0x0193 (403).
/// This message is used to request the addition of a wireless beltpack to the matrix.
/// This effectively adds a beltpack registration to the matrix with several properties
/// e.g. the beltpack role association mode and details.
/// </summary>
public class RequestBeltpackAddRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Maximum length for beltpack name.
    /// </summary>
    private const int MaxNameLength = 15;

    /// <summary>
    /// Null port number value (65535).
    /// </summary>
    public const ushort NullRole = 0xFFFF;

    /// <summary>
    /// Null role team ID value (255).
    /// </summary>
    public const byte NullRoleTeam = 0xFF;

    /// <summary>
    /// Protocol schema version (set to 1).
    /// </summary>
    public byte ProtocolSchema { get; set; } = 1;

    /// <summary>
    /// Serial number of the beltpack.
    /// </summary>
    public uint SerialNumber { get; set; }

    /// <summary>
    /// PMID (Physical Module ID) of the beltpack.
    /// </summary>
    public uint Pmid { get; set; }

    /// <summary>
    /// 15-character name of the beltpack.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Associated role (if applicable to mode).
    /// Set to 65535 (0xFFFF) if not used.
    /// </summary>
    public ushort Role { get; set; } = NullRole;

    /// <summary>
    /// Beltpack configuration mode.
    /// </summary>
    public BeltpackConfigMode Mode { get; set; } = BeltpackConfigMode.NotSet;

    /// <summary>
    /// Associated role team (if applicable to mode).
    /// Set to 255 (0xFF) if not used.
    /// </summary>
    public byte RoleTeam { get; set; } = NullRoleTeam;

    /// <summary>
    /// Creates a new Request Beltpack Add request.
    /// </summary>
    public RequestBeltpackAddRequest() 
        : base(HCIMessageID.RequestBeltpackAdd)
    {
        ExpectedReplyMessageID = HCIMessageID.ReplyBeltpackAdd;
    }

    /// <summary>
    /// Creates a new Request Beltpack Add request with specified parameters.
    /// </summary>
    /// <param name="serialNumber">The serial number of the beltpack.</param>
    /// <param name="pmid">The PMID of the beltpack.</param>
    /// <param name="name">The name of the beltpack (max 15 characters).</param>
    /// <param name="mode">The beltpack configuration mode.</param>
    /// <param name="role">The associated role (0xFFFF if not used).</param>
    /// <param name="roleTeam">The associated role team (0xFF if not used).</param>
    public RequestBeltpackAddRequest(uint serialNumber, uint pmid, string name, 
        BeltpackConfigMode mode, ushort role = NullRole, byte roleTeam = NullRoleTeam) 
        : base(HCIMessageID.RequestBeltpackAdd)
    {
        SerialNumber = serialNumber;
        Pmid = pmid;
        Name = name;
        Mode = mode;
        Role = role;
        RoleTeam = roleTeam;
        ExpectedReplyMessageID = HCIMessageID.ReplyBeltpackAdd;
    }

    /// <summary>
    /// Creates a request to add a beltpack in pool mode.
    /// </summary>
    /// <param name="serialNumber">The serial number of the beltpack.</param>
    /// <param name="pmid">The PMID of the beltpack.</param>
    /// <param name="name">The name of the beltpack.</param>
    /// <returns>A configured request to add a pool mode beltpack.</returns>
    public static RequestBeltpackAddRequest PoolMode(uint serialNumber, uint pmid, string name)
    {
        return new RequestBeltpackAddRequest(serialNumber, pmid, name, BeltpackConfigMode.Pool);
    }

    /// <summary>
    /// Creates a request to add a beltpack in fixed role mode.
    /// </summary>
    /// <param name="serialNumber">The serial number of the beltpack.</param>
    /// <param name="pmid">The PMID of the beltpack.</param>
    /// <param name="name">The name of the beltpack.</param>
    /// <param name="role">The fixed role to assign.</param>
    /// <returns>A configured request to add a fixed role beltpack.</returns>
    public static RequestBeltpackAddRequest FixedRoleMode(uint serialNumber, uint pmid, string name, ushort role)
    {
        return new RequestBeltpackAddRequest(serialNumber, pmid, name, BeltpackConfigMode.FixedRole, role);
    }

    /// <summary>
    /// Creates a request to add a beltpack in preferred role mode.
    /// </summary>
    /// <param name="serialNumber">The serial number of the beltpack.</param>
    /// <param name="pmid">The PMID of the beltpack.</param>
    /// <param name="name">The name of the beltpack.</param>
    /// <param name="role">The preferred role.</param>
    /// <returns>A configured request to add a preferred role beltpack.</returns>
    public static RequestBeltpackAddRequest PreferredRoleMode(uint serialNumber, uint pmid, string name, ushort role)
    {
        return new RequestBeltpackAddRequest(serialNumber, pmid, name, BeltpackConfigMode.PreferredRole, role);
    }

    /// <summary>
    /// Creates a request to add a beltpack in preferred team mode.
    /// </summary>
    /// <param name="serialNumber">The serial number of the beltpack.</param>
    /// <param name="pmid">The PMID of the beltpack.</param>
    /// <param name="name">The name of the beltpack.</param>
    /// <param name="roleTeam">The preferred role team.</param>
    /// <returns>A configured request to add a preferred team beltpack.</returns>
    public static RequestBeltpackAddRequest PreferredTeamMode(uint serialNumber, uint pmid, string name, byte roleTeam)
    {
        return new RequestBeltpackAddRequest(serialNumber, pmid, name, BeltpackConfigMode.PreferredTeam, NullRole, roleTeam);
    }

    /// <summary>
    /// Creates a request to add a beltpack in preferred role and team mode.
    /// </summary>
    /// <param name="serialNumber">The serial number of the beltpack.</param>
    /// <param name="pmid">The PMID of the beltpack.</param>
    /// <param name="name">The name of the beltpack.</param>
    /// <param name="role">The preferred role.</param>
    /// <param name="roleTeam">The preferred role team.</param>
    /// <returns>A configured request to add a preferred role and team beltpack.</returns>
    public static RequestBeltpackAddRequest PreferredRoleAndTeamMode(uint serialNumber, uint pmid, string name, ushort role, byte roleTeam)
    {
        return new RequestBeltpackAddRequest(serialNumber, pmid, name, BeltpackConfigMode.PreferredRoleAndTeam, role, roleTeam);
    }

    /// <inheritdoc/>
    protected override byte[] GeneratePayload()
    {
        // Payload: Protocol Tag (4) + Protocol Schema (1) + Serial Number (4) + PMID (4) + 
        //          Name (15) + Role (2) + Mode (1) + Role Team (1) = 32 bytes
        var payload = new byte[32];
        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // Serial Number: 4 bytes (big-endian)
        payload[offset++] = (byte)(SerialNumber >> 24);
        payload[offset++] = (byte)(SerialNumber >> 16);
        payload[offset++] = (byte)(SerialNumber >> 8);
        payload[offset++] = (byte)(SerialNumber & 0xFF);

        // PMID: 4 bytes (big-endian)
        payload[offset++] = (byte)(Pmid >> 24);
        payload[offset++] = (byte)(Pmid >> 16);
        payload[offset++] = (byte)(Pmid >> 8);
        payload[offset++] = (byte)(Pmid & 0xFF);

        // Name: 15 bytes (padded with nulls if shorter)
        byte[] nameBytes = Encoding.UTF8.GetBytes(Name ?? string.Empty);
        int nameBytesToCopy = Math.Min(nameBytes.Length, MaxNameLength);
        Array.Copy(nameBytes, 0, payload, offset, nameBytesToCopy);
        offset += MaxNameLength;

        // Role: 2 bytes (big-endian)
        payload[offset++] = (byte)(Role >> 8);
        payload[offset++] = (byte)(Role & 0xFF);

        // Mode: 1 byte
        payload[offset++] = (byte)Mode;

        // Role Team: 1 byte
        payload[offset++] = RoleTeam;

        return payload;
    }
}
