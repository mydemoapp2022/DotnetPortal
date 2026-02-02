using System.ServiceModel;
using OpenTelemetry.Instrumentation.Wcf;
using UI.EmployerPortal.Generated.ServiceClients.AccountMaintenanceService;
using UI.EmployerPortal.Generated.ServiceClients.AccountSummaryService;
using UI.EmployerPortal.Generated.ServiceClients.AddressValidationService;
using UI.EmployerPortal.Generated.ServiceClients.DocumentService;
using UI.EmployerPortal.Generated.ServiceClients.EmployerRegistrationService;
using UI.EmployerPortal.Generated.ServiceClients.LoginService;
using UI.EmployerPortal.Generated.ServiceClients.PortalCorrespondenceService;
using UI.EmployerPortal.Generated.ServiceClients.PortalUtilityService;
using UI.EmployerPortal.Generated.ServiceClients.SIDESService;
using UI.EmployerPortal.Generated.ServiceClients.TaxWageAdjustmentService;
using UI.EmployerPortal.Generated.ServiceClients.TaxWageReportingService;

namespace UI.EmployerPortal.Web.Startup.WcfServiceClients;

internal static class DependencyInjection
{
    public static IServiceCollection AddWcfServiceClients(this IServiceCollection services, IConfiguration builderConfiguration, bool isOpenTelemetryEnabled)
    {
        var configurations = WcfServiceClientConfigurations.LoadFromConfiguration(builderConfiguration);
        services.AddTransient<IAccountMaintenanceService>(x =>
        {
            var configuration = configurations.AccountMaintenanceServiceConfiguration ?? WcfServiceClientConfiguration.DefaultConfiguration;
            var binding = CreateBinding(configuration);
            var endpointAddress = string.IsNullOrEmpty(configuration?.Url)
                ? new AccountMaintenanceServiceClient().Endpoint.Address
                : new EndpointAddress(configuration.Url);

            var client = new AccountMaintenanceServiceClient(binding, endpointAddress);
            if (isOpenTelemetryEnabled)
            {
                client.Endpoint.EndpointBehaviors.Add(new TelemetryEndpointBehavior());
            }

            return client;
        });

        services.AddTransient<IAccountSummaryService>(x =>
        {
            var configuration = configurations.AccountSummaryServiceConfiguration ?? WcfServiceClientConfiguration.DefaultConfiguration;
            var binding = CreateBinding(configuration);
            var endpointAddress = string.IsNullOrEmpty(configuration?.Url)
                ? new AccountSummaryServiceClient().Endpoint.Address
                : new EndpointAddress(configuration.Url);

            var client = new AccountSummaryServiceClient(binding, endpointAddress);
            if (isOpenTelemetryEnabled)
            {
                client.Endpoint.EndpointBehaviors.Add(new TelemetryEndpointBehavior());
            }

            return client;
        });

        services.AddTransient<IAddressValidationService>(x =>
        {
            var configuration = configurations.AddressValidationServiceConfiguration ?? WcfServiceClientConfiguration.DefaultConfiguration;
            var binding = CreateBinding(configuration);
            var endpointAddress = string.IsNullOrEmpty(configuration?.Url)
                ? new AddressValidationServiceClient().Endpoint.Address
                : new EndpointAddress(configuration.Url);

            var client = new AddressValidationServiceClient(binding, endpointAddress);
            if (isOpenTelemetryEnabled)
            {
                client.Endpoint.EndpointBehaviors.Add(new TelemetryEndpointBehavior());
            }

            return client;
        });

        services.AddTransient<DocumentRequestService>(x =>
        {
            var configuration = configurations.DocumentRequestServiceConfiguration ?? WcfServiceClientConfiguration.DefaultConfiguration;
            var binding = CreateBinding(configuration);
            var endpointAddress = string.IsNullOrEmpty(configuration?.Url)
                ? new DocumentRequestServiceClient().Endpoint.Address
                : new EndpointAddress(configuration.Url);

            var client = new DocumentRequestServiceClient(binding, endpointAddress);
            if (isOpenTelemetryEnabled)
            {
                client.Endpoint.EndpointBehaviors.Add(new TelemetryEndpointBehavior());
            }

            return client;
        });

        services.AddTransient<IEmployerRegistrationService>(x =>
        {
            var configuration = configurations.EmployerRegistrationServiceConfiguration ?? WcfServiceClientConfiguration.DefaultConfiguration;
            var binding = CreateBinding(configuration);
            var endpointAddress = string.IsNullOrEmpty(configuration?.Url)
                ? new EmployerRegistrationServiceClient().Endpoint.Address
                : new EndpointAddress(configuration.Url);

            var client = new EmployerRegistrationServiceClient(binding, endpointAddress);
            if (isOpenTelemetryEnabled)
            {
                client.Endpoint.EndpointBehaviors.Add(new TelemetryEndpointBehavior());
            }

            return client;
        });

        services.AddTransient<ILoginService>(x =>
        {
            var configuration = configurations.LoginServiceConfiguration ?? WcfServiceClientConfiguration.DefaultConfiguration;
            var binding = CreateBinding(configuration);
            var endpointAddress = string.IsNullOrEmpty(configuration?.Url)
                ? new LoginServiceClient().Endpoint.Address
                : new EndpointAddress(configuration.Url);

            var client = new LoginServiceClient(binding, endpointAddress);
            if (isOpenTelemetryEnabled)
            {
                client.Endpoint.EndpointBehaviors.Add(new TelemetryEndpointBehavior());
            }

            return client;
        });

        services.AddTransient<IPortalCorrespondenceService>(x =>
        {
            var configuration = configurations.PortalCorrespondenceServiceConfiguration ?? WcfServiceClientConfiguration.DefaultConfiguration;
            var binding = CreateBinding(configuration);
            var endpointAddress = string.IsNullOrEmpty(configuration?.Url)
                ? new PortalCorrespondenceServiceClient().Endpoint.Address
                : new EndpointAddress(configuration.Url);

            var client = new PortalCorrespondenceServiceClient(binding, endpointAddress);
            if (isOpenTelemetryEnabled)
            {
                client.Endpoint.EndpointBehaviors.Add(new TelemetryEndpointBehavior());
            }

            return client;
        });

        services.AddTransient<IPortalUtilityService>(x =>
        {
            var configuration = configurations.PortalUtilityServiceConfiguration ?? WcfServiceClientConfiguration.DefaultConfiguration;
            var binding = CreateBinding(configuration);
            var endpointAddress = string.IsNullOrEmpty(configuration?.Url)
                ? new PortalUtilityServiceClient().Endpoint.Address
                : new EndpointAddress(configuration.Url);

            var client = new PortalUtilityServiceClient(binding, endpointAddress);
            if (isOpenTelemetryEnabled)
            {
                client.Endpoint.EndpointBehaviors.Add(new TelemetryEndpointBehavior());
            }

            return client;
        });

        services.AddTransient<ISIDESService>(x =>
        {
            var configuration = configurations.SIDESServiceConfiguration ?? WcfServiceClientConfiguration.DefaultConfiguration;
            var binding = CreateBinding(configuration);
            var endpointAddress = string.IsNullOrEmpty(configuration?.Url)
                ? new SIDESServiceClient().Endpoint.Address
                : new EndpointAddress(configuration.Url);

            var client = new SIDESServiceClient(binding, endpointAddress);
            if (isOpenTelemetryEnabled)
            {
                client.Endpoint.EndpointBehaviors.Add(new TelemetryEndpointBehavior());
            }

            return client;
        });

        services.AddTransient<ITaxWageAdjustmentService>(x =>
        {
            var configuration = configurations.TaxWageAdjustmentServiceConfiguration ?? WcfServiceClientConfiguration.DefaultConfiguration;
            var binding = CreateBinding(configuration);
            var endpointAddress = string.IsNullOrEmpty(configuration?.Url)
                ? new TaxWageAdjustmentServiceClient().Endpoint.Address
                : new EndpointAddress(configuration.Url);

            var client = new TaxWageAdjustmentServiceClient(binding, endpointAddress);
            if (isOpenTelemetryEnabled)
            {
                client.Endpoint.EndpointBehaviors.Add(new TelemetryEndpointBehavior());
            }

            return client;
        });

        services.AddTransient<ITaxWageReportingService>(x =>
        {
            var configuration = configurations.TaxWageReportingServiceConfiguration ?? WcfServiceClientConfiguration.DefaultConfiguration;
            var binding = CreateBinding(configuration);
            var endpointAddress = string.IsNullOrEmpty(configuration?.Url)
                ? new TaxWageReportingServiceClient().Endpoint.Address
                : new EndpointAddress(configuration.Url);

            var client = new TaxWageReportingServiceClient(binding, endpointAddress);
            if (isOpenTelemetryEnabled)
            {
                client.Endpoint.EndpointBehaviors.Add(new TelemetryEndpointBehavior());
            }

            return client;
        });

        return services;
    }

    private static BasicHttpBinding CreateBinding(WcfServiceClientConfiguration configuration)
    {
        return new BasicHttpBinding
        {
            MaxReceivedMessageSize = configuration!.MaxReceivedMessageSize,
            Security =
            {
                Mode = configuration.SecurityMode,
                Transport =
                {
                    ClientCredentialType = configuration.ClientCredentialType
                }
            }
        };
    }
}
