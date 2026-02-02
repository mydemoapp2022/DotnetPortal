using System.ServiceModel;

namespace UI.EmployerPortal.Web.Startup.WcfServiceClients;

internal sealed record WcfServiceClientConfiguration
{
    public static readonly WcfServiceClientConfiguration DefaultConfiguration = new()
    {
        ClientCredentialTypeValue = HttpClientCredentialType.Windows.ToString(),
        MaxReceivedMessageSize = 2_000_000,
        SecurityModeValue = BasicHttpSecurityMode.Transport.ToString(),
    };

    public string? Url { get; init; }

    public string? SecurityModeValue
    {
        private get;
        init
        {
            field = value;
            SecurityMode = Enum.TryParse<BasicHttpSecurityMode>(value, out var securityMode) ? securityMode : BasicHttpSecurityMode.None;
        }
    }

    public string? ClientCredentialTypeValue
    {
        private get;
        init
        {
            field = value;
            ClientCredentialType = Enum.TryParse<HttpClientCredentialType>(value, out var clientCredentialType) ? clientCredentialType : HttpClientCredentialType.Windows;
        }
    }

    public int MaxReceivedMessageSize { get; init; }

    public BasicHttpSecurityMode SecurityMode { get; private init; }
    public HttpClientCredentialType ClientCredentialType { get; private init; }
}
