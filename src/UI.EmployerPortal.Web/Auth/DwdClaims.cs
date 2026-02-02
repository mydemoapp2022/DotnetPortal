namespace UI.EmployerPortal.Web.Auth;

/// <summary>
/// DWD Claims record for user information
/// </summary>
public record DwdClaims
{
    /// <summary>
    /// Gets the user's first name.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the user's last name.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the user's unique identifier.
    /// </summary>
    public string UserID { get; init; } = string.Empty;

    /// <summary>
    /// Gets the user's Okta UUID.
    /// </summary>
    public string OktaUUID { get; init; } = string.Empty;

    /// <summary>
    /// Gets the user's secure user surrogate key.
    /// </summary>
    public int SecureUserSK { get; init; }
}
