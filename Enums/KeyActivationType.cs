namespace HCILibrary.Enums;

/// <summary>
/// Key activation type.
/// </summary>
public enum KeyActivationType : byte
{
    /// <summary>
    /// Talk only.
    /// </summary>
    TalkOnly = 1,

    /// <summary>
    /// Listen only.
    /// </summary>
    ListenOnly = 2,

    /// <summary>
    /// Talk and listen.
    /// </summary>
    TalkAndListen = 3,

    /// <summary>
    /// Dual talk and listen.
    /// </summary>
    DualTalkAndListen = 4,

    /// <summary>
    /// Talk and forced listen.
    /// </summary>
    TalkAndForcedListen = 5
}
