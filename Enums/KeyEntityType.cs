namespace HCILibrary.Enums;

/// <summary>
/// Entity type for key assignments.
/// </summary>
public enum KeyEntityType : ushort
{
    /// <summary>
    /// Remove assignment from key.
    /// </summary>
    Null = 0x0000,

    /// <summary>
    /// Add port assignment to key.
    /// </summary>
    Port = 0x0001,

    /// <summary>
    /// Add a Conference to a key.
    /// </summary>
    Conference = 0x0002,

    /// <summary>
    /// Add a Fixed Group to a key.
    /// </summary>
    Group = 0x0003,

    /// <summary>
    /// Add an IFB to a key.
    /// </summary>
    Ifb = 0x0004
}
