namespace HCILibrary.Enums;

/// <summary>
/// Indicates the HCI protocol version of a message.
/// </summary>
public enum HCIVersion
{
    /// <summary>
    /// HCI version 1 protocol.
    /// </summary>
    HCIv1 = 1,

    /// <summary>
    /// HCI version 2 protocol (indicated by AB BA CE DE marker).
    /// </summary>
    HCIv2 = 2
}
