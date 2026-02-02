using System.Security.Claims;
//using UI.EmployerPortal.Generated.ServiceClients.LoginService;
using UI.EmployerPortal.Web.Startup.ResiliencyProtocols;
using SecureUser = UI.EmployerPortal.Web.Features.Shared.Accounts.Models.SecureUser;

namespace UI.EmployerPortal.Web.Features.Shared.Accounts.Services;

internal interface IUserAccountService
{
    bool IsAuthenticated();
    ClaimsPrincipal? GetCurrentUser();
    IEnumerable<Claim> GetClaims();
    string? GetClaimValue(string type);
    string? GetCurrentUserId();
    Task<SecureUser> ObtainSecureUser(string userID, string email, string firstName, string lastName, string oktaUUID);
}

internal class UserAccountService : IUserAccountService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAsyncRetryPolicy<UserAccountService> _retryPolicy;
    //private readonly ILoginService _loginService;

    public UserAccountService(IHttpContextAccessor httpContextAccessor,
                            IAsyncRetryPolicy<UserAccountService> retryPolicy
                            )
    {
        _httpContextAccessor = httpContextAccessor;
        _retryPolicy = retryPolicy;
        //_loginService = loginService;
    }

    public ClaimsPrincipal? GetCurrentUser()
    {
        return _httpContextAccessor.HttpContext?.User;
    }

    public IEnumerable<Claim> GetClaims()
    {
        return GetCurrentUser()?.Claims ?? Enumerable.Empty<Claim>();
    }

    public string? GetClaimValue(string type)
    {
        return GetCurrentUser()?.FindFirst(type)?.Value;
    }

    public string? GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public async Task<SecureUser> ObtainSecureUser(string userID, string email, string firstName, string lastName, string oktaUUID)
    {
        //var request = new SecureUserRequest()
        //{
        //    UserID = userID,
        //    EmailAddress = email,
        //    FirstName = firstName,
        //    LastName = lastName,
        //    OktaUUID = oktaUUID,
        //};

        //var response = await _retryPolicy.ExecuteAsync(() =>
        //{
        //    return _loginService.ObtainSecureUserAsync(request);
        //});

        //// Safely map to domain SecureUser (assuming result proxy carries user info)
        //var proxy = response.User;

        //return new SecureUser
        //{
        //    SecureUserSK = proxy.SecureUserSK,
        //    UserID = proxy.UserID,
        //    EmailAddress = proxy.EmailAddress,
        //    FirstName = proxy.FirstName,
        //    LastName = proxy.LastName,
        //    OktaUUID = proxy.OktaUUID,
        //    WIUID = proxy.WIUID,
        //    Domain = proxy.Domain,
        //    WebSessionUUID = proxy.WebSessionUUID
        //};
        return new SecureUser
        {
            SecureUserSK = 1,
            UserID = "1",
            EmailAddress = "abc@mail.com",
            FirstName = "FirstName",
            LastName = "LastName",
            OktaUUID = "OktaUUID",
            WIUID = "WIUID",
            Domain = "Domain",
            WebSessionUUID = "WebSessionUUID"
        };
    }

    public bool IsAuthenticated()
    {
        return GetCurrentUser()?.Identity?.IsAuthenticated ?? false;
    }
}
