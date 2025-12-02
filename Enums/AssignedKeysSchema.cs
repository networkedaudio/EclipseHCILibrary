namespace HCILibrary.Enums;

/// <summary>
/// Protocol schema version for Request Assigned Keys.
/// </summary>
public enum AssignedKeysSchema : byte
{
    /// <summary>
    /// Schema version 1 - basic information.
    /// </summary>
    Schema1 = 1,

    /// <summary>
    /// Schema version 2 - extended information in reply.
    /// </summary>
    Schema2 = 2
}
