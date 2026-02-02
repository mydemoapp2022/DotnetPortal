using System.Security.Claims;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Models;

namespace UI.EmployerPortal.Web.Features.Shared.Accounts.Services;

/// <summary>
/// Mock implementation of IUserAccountService for development/demo purposes
/// </summary>
internal class MockUserAccountService : IUserAccountService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MockUserAccountService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated()
    {
        return true;
    }

    public ClaimsPrincipal? GetCurrentUser()
    {
        return _httpContextAccessor.HttpContext?.User;
    }

    public IEnumerable<Claim> GetClaims()
    {
        // Return mock claims for development
        return
        [
            new Claim("FirstName", "John"),
            new Claim("LastName", "Doe"),
            new Claim("UserID", "demo-user"),
            new Claim("OktaUUID", "mock-okta-uuid-12345"),
            new Claim("SecureUserSK", "12345")
        ];
    }

    public string? GetClaimValue(string type)
    {
        return GetClaims().FirstOrDefault(c =>
        {
            return c.Type == type;
        })?.Value;
    }

    public string? GetCurrentUserId()
    {
        return "demo-user";
    }

    public Task<SecureUser> ObtainSecureUser(string userID, string email, string firstName, string lastName, string oktaUUID)
    {
        return Task.FromResult(new SecureUser
        {
            SecureUserSK = 12345,
            UserID = userID,
            EmailAddress = email,
            FirstName = firstName,
            LastName = lastName,
            OktaUUID = oktaUUID
        });
    }
}
