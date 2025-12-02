namespace HCILibrary.Models;

/// <summary>
/// Represents an alias dialcode for delete operations.
/// </summary>
public class AliasDialcode
{
    /// <summary>
    /// Entity instance (bits 0-15). For example, 1st port in frame would have instance 1.
    /// </summary>
    public ushort EntityInstance { get; set; }

    /// <summary>
    /// Entity type (bits 16-23). See entity definitions.
    /// </summary>
    public byte EntityType { get; set; }

    /// <summary>
    /// Target system number (bits 24-31). For future expansion.
    /// Must currently be set to the matrix ID of the receiving frame or 0.
    /// </summary>
    public byte TargetSystemNumber { get; set; }

    /// <summary>
    /// Creates a new alias dialcode.
    /// </summary>
    public AliasDialcode()
    {
    }

    /// <summary>
    /// Creates a new alias dialcode with the specified values.
    /// </summary>
    /// <param name="entityInstance">Entity instance (0-65535).</param>
    /// <param name="entityType">Entity type.</param>
    /// <param name="targetSystemNumber">Target system number (0 for local).</param>
    public AliasDialcode(ushort entityInstance, byte entityType, byte targetSystemNumber = 0)
    {
        EntityInstance = entityInstance;
        EntityType = entityType;
        TargetSystemNumber = targetSystemNumber;
    }

    /// <summary>
    /// Converts the dialcode to a 4-byte array.
    /// </summary>
    /// <returns>4-byte array representing the dialcode.</returns>
    public byte[] ToBytes()
    {
        // Bits 0-15: Entity instance (little-endian within the 4-byte structure)
        // Bits 16-23: Entity type
        // Bits 24-31: Target system number
        return new byte[]
        {
            (byte)(EntityInstance & 0xFF),
            (byte)((EntityInstance >> 8) & 0xFF),
            EntityType,
            TargetSystemNumber
        };
    }

    /// <summary>
    /// Creates an AliasDialcode from a 4-byte array.
    /// </summary>
    /// <param name="data">The 4-byte array.</param>
    /// <param name="offset">The offset into the array.</param>
    /// <returns>The parsed AliasDialcode.</returns>
    public static AliasDialcode FromBytes(byte[] data, int offset = 0)
    {
        return new AliasDialcode
        {
            EntityInstance = (ushort)(data[offset] | (data[offset + 1] << 8)),
            EntityType = data[offset + 2],
            TargetSystemNumber = data[offset + 3]
        };
    }
}
