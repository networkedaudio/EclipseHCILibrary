namespace HCILibrary.Enums;

/// <summary>
/// Beltpack configuration mode values.
/// Determines how a beltpack is assigned to roles/teams.
/// </summary>
public enum BeltpackConfigMode : byte
{
    /// <summary>
    /// Pool mode - beltpack can be assigned to any available role.
    /// </summary>
    Pool = 0,

    /// <summary>
    /// Preferred team mode.
    /// </summary>
    PreferredTeam = 1,

    /// <summary>
    /// Preferred role mode.
    /// </summary>
    PreferredRole = 2,

    /// <summary>
    /// Preferred role and team mode.
    /// </summary>
    PreferredRoleAndTeam = 3,

    /// <summary>
    /// Fixed role mode - beltpack is assigned to a specific role.
    /// </summary>
    FixedRole = 4,

    /// <summary>
    /// Mode not set.
    /// </summary>
    NotSet = 225
}
