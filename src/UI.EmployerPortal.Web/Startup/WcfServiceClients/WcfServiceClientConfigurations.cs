namespace UI.EmployerPortal.Web.Startup.WcfServiceClients;

internal sealed record WcfServiceClientConfigurations
{
    public WcfServiceClientConfiguration? AccountMaintenanceServiceConfiguration { get; init; }
    public WcfServiceClientConfiguration? AccountSummaryServiceConfiguration { get; init; }
    public WcfServiceClientConfiguration? AddressValidationServiceConfiguration { get; init; }
    public WcfServiceClientConfiguration? DocumentRequestServiceConfiguration { get; init; }
    public WcfServiceClientConfiguration? EmployerRegistrationServiceConfiguration { get; init; }
    public WcfServiceClientConfiguration? LoginServiceConfiguration { get; init; }
    public WcfServiceClientConfiguration? PortalCorrespondenceServiceConfiguration { get; init; }
    public WcfServiceClientConfiguration? PortalUtilityServiceConfiguration { get; init; }
    public WcfServiceClientConfiguration? SIDESServiceConfiguration { get; init; }
    public WcfServiceClientConfiguration? TaxWageAdjustmentServiceConfiguration { get; init; }
    public WcfServiceClientConfiguration? TaxWageReportingServiceConfiguration { get; init; }

    public static WcfServiceClientConfigurations LoadFromConfiguration(IConfiguration configuration)
    {
        var sectionName = "WcfServiceClient";
        return new WcfServiceClientConfigurations()
        {
            AccountMaintenanceServiceConfiguration = LoadConfiguration(sectionName, "AccountMaintenanceService", configuration),
            AccountSummaryServiceConfiguration = LoadConfiguration(sectionName, "AccountSummaryService", configuration),
            AddressValidationServiceConfiguration = LoadConfiguration(sectionName, "AddressValidationService", configuration),
            DocumentRequestServiceConfiguration = LoadConfiguration(sectionName, "DocumentRequestService", configuration),
            EmployerRegistrationServiceConfiguration = LoadConfiguration(sectionName, "EmployerRegistrationService", configuration),
            LoginServiceConfiguration = LoadConfiguration(sectionName, "LoginService", configuration),
            PortalCorrespondenceServiceConfiguration = LoadConfiguration(sectionName, "PortalCorrespondenceService", configuration),
            PortalUtilityServiceConfiguration = LoadConfiguration(sectionName, "PortalUtilityService", configuration),
            SIDESServiceConfiguration = LoadConfiguration(sectionName, "SIDESService", configuration),
            TaxWageAdjustmentServiceConfiguration = LoadConfiguration(sectionName, "TaxWageAdjustmentService", configuration),
            TaxWageReportingServiceConfiguration = LoadConfiguration(sectionName, "TaxWageReportingService", configuration),
        };
    }

    private static WcfServiceClientConfiguration LoadConfiguration(string sectionName, string serviceName, IConfiguration configuration)
    {
        return new WcfServiceClientConfiguration
        {
            Url = configuration.GetValue<string?>($"{sectionName}:{serviceName}:Url"),
            MaxReceivedMessageSize = configuration.GetValue<int?>($"{sectionName}:{serviceName}:MaxReceivedMessageSize") ?? WcfServiceClientConfiguration.DefaultConfiguration.MaxReceivedMessageSize,
            ClientCredentialTypeValue = configuration.GetValue<string?>($"{sectionName}:{serviceName}:ClientCredentialType") ?? WcfServiceClientConfiguration.DefaultConfiguration.ClientCredentialType.ToString(),
            SecurityModeValue = configuration.GetValue<string?>($"{sectionName}:{serviceName}:SecurityMode") ?? WcfServiceClientConfiguration.DefaultConfiguration.SecurityMode.ToString(),
        };
    }
}
