using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using UI.EmployerPortal.Web.Auth;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Services;

namespace UI.EmployerPortal.Web.Startup;

internal static class MyWisconsinID
{
    private const string AuthCookieName = "UIEmployerPortalAuth";

    public static IServiceCollection AddMyWisconsinId(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IUserAccountService, UserAccountService>();
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = AuthCookieName;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.HttpOnly = true;
            })
            .AddOpenIdConnect(options =>
            {
                options.Authority = configuration.GetValue<string>("OIDC:Authority").TrimEnd("/").ToString();
                options.ClientId = configuration.GetValue<string>("OIDC:ClientId");
                options.ClientSecret = configuration.GetValue<string>("OIDC:ClientSecret");
                options.MetadataAddress = $"{options.Authority}/.well-known/oauth-authorization-server";
                options.RequireHttpsMetadata = true;
                options.ResponseType = "code";

                options.SaveTokens = true;

                options.CallbackPath = "/authorization-code/callback";
                options.SignedOutCallbackPath = "/signed-out";

                options.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var serviceContext = context.HttpContext.RequestServices;

                        var accountService = serviceContext.GetRequiredService<IUserAccountService>();
                        var logger = serviceContext.GetRequiredService<ILogger<Program>>();

                        var principal = context.Principal;

                        if (principal is not null &&
                            !principal.HasClaim(c =>
                            {
                                return c.Type == ClaimTypes.NameIdentifier;
                            }))
                        {
                            var subClaim = principal.FindFirst("sub")?.Value;
                            if (subClaim is not null && principal.Identity is ClaimsIdentity id)
                            {
                                id.AddClaim(new Claim(ClaimTypes.NameIdentifier, subClaim));
                            }
                        }

                        if (principal?.Identity?.IsAuthenticated ?? false)
                        {
                            var dwdClaims = principal.Claims.GetDwdClaims();
                            var response = await accountService.ObtainSecureUser(dwdClaims.UserID!,
                                dwdClaims.UserID!,
                                dwdClaims.FirstName!,
                                dwdClaims.LastName!,
                                dwdClaims.OktaUUID!);

                            var claimsIdentiy = (ClaimsIdentity) principal.Identity;
                            var secureUserSKClaim = response.SecureUserSK.ToString().AsDwdSecureUserSKClaim();
                            claimsIdentiy.AddClaim(secureUserSKClaim);
                        }
                        else
                        {
                            logger.LogWarning("User is NOT authenticated.");
                        }
                    },
                    OnSignedOutCallbackRedirect = async context =>
                    {
                        foreach (var cookie in context.Request.Cookies.Keys)
                        {
                            if (cookie.StartsWith(AuthCookieName))
                            {
                                context.Response.Cookies.Delete(cookie, new CookieOptions
                                {
                                    Path = "/",
                                    Domain = null
                                });
                            }
                        }
                    },
                    OnRedirectToIdentityProviderForSignOut = async context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                            .CreateLogger("OpenIdConnect");

                        var idToken = await context.HttpContext.GetTokenAsync("id_token");
                        if (!string.IsNullOrWhiteSpace(idToken))
                        {
                            context.ProtocolMessage.IdTokenHint = idToken;
                            logger.LogInformation("Sign-out: id_token_hint provided, Okta session will end");
                        }
                        else
                        {
                            logger.LogWarning("Sign-out: id_token_hint is NULL - Okta SSO session will NOT end!");
                        }
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<Program>>();

                        logger.LogError(context.Exception, "Okta authentication failed");
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddCascadingAuthenticationState();
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }

    public static WebApplication AddMyWisconsinIDEndpoints(this WebApplication app)
    {
        app.MapGet("/authorization-code/callback", async context =>
        {
            context.Response.Redirect("/");
        });

        app.MapGet("/signed-out", async context =>
        {
            context.Response.Redirect("/");
        });

        app.MapGet("/auth/login", async context =>
        {
            await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
        });
        app.MapGet("/auth/logout", async context =>
        {
            await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
        });

        return app;
    }
}
