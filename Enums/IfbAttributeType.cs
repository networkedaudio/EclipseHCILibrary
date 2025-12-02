namespace HCILibrary.Enums;

/// <summary>
/// IFB attribute type for Request IFB Status.
/// </summary>
public enum IfbAttributeType : byte
{
    /// <summary>
    /// Request internal level.
    /// </summary>
    IntLevel = 0,

    /// <summary>
    /// Request dim level.
    /// </summary>
    DimLevel = 1,

    /// <summary>
    /// Request priority.
    /// </summary>
    Priority = 2,

    /// <summary>
    /// Request active callers.
    /// </summary>
    ActiveCallers = 3,

    /// <summary>
    /// Request sources.
    /// </summary>
    Sources = 4,

    /// <summary>
    /// Request destination.
    /// </summary>
    Destination = 5,

    /// <summary>
    /// Request returns.
    /// </summary>
    Returns = 6,

    /// <summary>
    /// Request potential callers.
    /// Use this when requesting IFB info from a matrix that doesn't own the IFB.
    /// </summary>
    PotentialCallers = 7,

    /// <summary>
    /// Return listens (Reply only).
    /// </summary>
    ReturnListens = 8,

    /// <summary>
    /// Destination listens (Reply only).
    /// </summary>
    DestinationListens = 9,

    /// <summary>
    /// Request all attribute types for the specified IFB.
    /// </summary>
    All = 255
}
