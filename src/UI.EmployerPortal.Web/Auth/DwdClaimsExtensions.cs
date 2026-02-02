using System.Security.Claims;

namespace UI.EmployerPortal.Web.Auth;

/// <summary>
/// Extension methods for Claims to extract DWD-specific claims
/// </summary>
public static class DwdClaimsExtensions
{
    /// <summary>
    /// Converts a collection of claims to DwdClaims record
    /// </summary>
    public static DwdClaims GetDwdClaims(this IEnumerable<Claim> claims)
    {
        return new DwdClaims
        {
            FirstName = claims.FirstOrDefault(c =>
            {
                return c.Type == "FirstName";
            })?.Value ?? "Guest",
            LastName = claims.FirstOrDefault(c =>
            {
                return c.Type == "LastName";
            })?.Value ?? "User",
            UserID = claims.FirstOrDefault(c =>
            {
                return c.Type == "UserID";
            })?.Value ?? string.Empty,
            OktaUUID = claims.FirstOrDefault(c =>
            {
                return c.Type == "OktaUUID";
            })?.Value ?? string.Empty,
            SecureUserSK = int.TryParse(claims.FirstOrDefault(c =>
            {
                return c.Type == "SecureUserSK";
            })?.Value, out var sk) ? sk : 0
        };
    }
}
