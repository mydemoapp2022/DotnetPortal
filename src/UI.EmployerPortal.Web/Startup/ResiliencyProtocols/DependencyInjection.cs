namespace UI.EmployerPortal.Web.Startup.ResiliencyProtocols;

internal static class DependencyInjection
{
    public static IServiceCollection AddRetryPolicies(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IAsyncRetryPolicy<>), typeof(AsyncRetryPolicyWrapper<>));

        return services;
    }
}
