using System.ServiceModel;
using System.Text;
using UI.EmployerPortal.Web.Features.BillingPayments.Models;

namespace UI.EmployerPortal.Web.Features.BillingPayments.Services;

/// <summary>
/// Integrates with US Bank eBill Orbipay (Alacriti) for hosted card payment forms.
/// This service handles session creation and payment confirmation following Orbipay's
/// hosted payment workflow, mirroring the reference VB.NET CardPaymentPost.aspx.vb implementation.
/// </summary>
public interface ICardPaymentService
{
    /// <summary>
    /// Creates an Orbipay hosted form session for card payment entry.
    /// Returns HTML markup to embed the Orbipay form in the page.
    /// </summary>
    Task<OrbipaySessionResult> CreateHostedFormSessionAsync(
        decimal amount,
        string contactName,
        string email,
        string addressLine1,
        string? addressLine2,
        string city,
        string? state,
        string zip,
        string country);

    /// <summary>
    /// Confirms payment after Orbipay hosted form submission.
    /// Called when user completes the form and posts back to the application.
    /// Mirrors the reference CardPaymentPost workflow: extract token, call Alacriti API,
    /// save result to database.
    /// </summary>
    Task<OrbipayConfirmationResult> ConfirmPaymentAsync(
        string token,
        string digiSign,
        string customerAccountReference,
        OrbipayPaymentConfirmationRequest request);
}

/// <summary>
/// Implementation of Orbipay hosted payment form integration.
/// Follows the US Bank eBill system workflow for card payments.
/// </summary>
internal sealed partial class CardPaymentService : ICardPaymentService
{
    private readonly IConfiguration _config;
    private readonly ILogger<CardPaymentService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CardPaymentService(
        IConfiguration config,
        ILogger<CardPaymentService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<OrbipaySessionResult> CreateHostedFormSessionAsync(
        decimal amount,
        string contactName,
        string email,
        string addressLine1,
        string? addressLine2,
        string city,
        string? state,
        string zip,
        string country)
    {
        try
        {
            var orbipaySection = _config.GetSection("Orbipay");
            if (!orbipaySection.Exists())
            {
                LogOrbipayConfigMissing(_logger);
                return new OrbipaySessionResult
                {
                    Success = false,
                    ErrorMessage = "Payment provider configuration is unavailable. Please contact support."
                };
            }

            // Extract configuration
            var hostedFormUrl = orbipaySection["HostedFormUrl"];
            var clientKey = orbipaySection["ClientKey"];
            var locale = orbipaySection["Locale"] ?? "en";

            if (string.IsNullOrEmpty(hostedFormUrl) || string.IsNullOrEmpty(clientKey))
            {
                LogOrbipayConfigIncomplete(_logger);
                return new OrbipaySessionResult
                {
                    Success = false,
                    ErrorMessage = "Payment provider is not properly configured."
                };
            }

            // Parse contact name
            var nameParts = (contactName ?? string.Empty).Split(' ', 2);
            var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
            var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            // Generate unique form/script IDs
            var formId = "orbipay-checkout-form";
            var scriptId = "orbipay-checkout-script";

            // Build the hosted form HTML (mirrors reference VB.NET GetHostedForm)
            var html = BuildOrbipayFormMarkup(
                hostedFormUrl,
                formId,
                scriptId,
                clientKey,
                firstName,
                lastName,
                email,
                addressLine1,
                addressLine2,
                city,
                state,
                zip,
                country,
                amount,
                locale);

            LogHostedFormSessionCreated(_logger, amount);

            return new OrbipaySessionResult
            {
                Success = true,
                HostedFormHtml = html
            };
        }
        catch (Exception ex)
        {
            LogErrorCreatingHostedFormSession(_logger, ex);
            return new OrbipaySessionResult
            {
                Success = false,
                ErrorMessage = "An error occurred while preparing the payment form. Please try again."
            };
        }
    }

    public async Task<OrbipayConfirmationResult> ConfirmPaymentAsync(
        string token,
        string digiSign,
        string customerAccountReference,
        OrbipayPaymentConfirmationRequest request)
    {
        try
        {
            var orbipaySection = _config.GetSection("Orbipay");
            var clientKey = orbipaySection["ClientKey"];
            var signatureKey = orbipaySection["SignatureKey"];
            var apiKey = orbipaySection["ApiKey"];
            var privateKey = orbipaySection["PrivateKey"];
            var publicKey = orbipaySection["PublicKey"];
            var liveMode = orbipaySection["LiveMode"]?.ToLowerInvariant() ?? "false";

            if (string.IsNullOrEmpty(clientKey) || string.IsNullOrEmpty(signatureKey))
            {
                LogOrbipayCredentialsIncomplete(_logger);
                return new OrbipayConfirmationResult
                {
                    Success = false,
                    ErrorDescription = "Payment provider credentials are incomplete."
                };
            }

            // Build custom fields (mirrors reference implementation)
            var customFields = BuildCustomFields(request);

            try
            {
                // TODO: Integrate with actual Alacriti Com.Alacriti.Checkout.Api.Payment class
                // Pseudocode below shows the intended flow from reference VB.NET:
                //
                // var invocationContext = new Com.Alacriti.Checkout.Api.InvocationContext(
                //     apiKey, privateKey, publicKey);
                //
                // var payment = new Com.Alacriti.Checkout.Api.Payment(customerAccountReference, request.Amount.ToString())
                //     .withToken(token, digiSign)
                //     .forClient(clientKey, signatureKey, apiKey)
                //     .withCustomFields(customFields)
                //     .confirm(invocationContext, liveMode);
                //
                // if (payment != null && payment.Error == null)
                // {
                //     // Success: Save payment details
                //     return new OrbipayConfirmationResult
                //     {
                //         Success = true,
                //         ConfirmationNumber = payment.ConfirmationNumber,
                //         Amount = decimal.Parse(payment.Amount),
                //         PaymentMethod = payment.PaymentMethod,
                //         LastFourDigits = payment.FundingAccount?.AccountNumber?.Last(4).ToString(),
                //         ConvenienceFee = string.IsNullOrEmpty(payment.Fee?.Feeamount)
                //             ? null
                //             : decimal.Parse(payment.Fee.Feeamount),
                //         PaymentDate = DateTime.Parse(payment.PaymentDate)
                //     };
                // }
                // else
                // {
                //     // Error: Extract error details
                //     var errorDesc = string.Join("; ", payment.Error.Select(e => e.Message));
                //     return new OrbipayConfirmationResult
                //     {
                //         Success = false,
                //         ErrorDescription = errorDesc,
                //         ErrorField = payment.Error.FirstOrDefault()?.Field,
                //         ErrorCode = payment.Error.FirstOrDefault()?.Code
                //     };
                // }

                // Temporary stub implementation for testing flow
                var confirmation = new OrbipayConfirmationResult
                {
                    Success = true,
                    ConfirmationNumber = $"ORB{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Amount = request.Amount,
                    PaymentMethod = "CREDIT CARD",
                    LastFourDigits = "4242",
                    ConvenienceFee = Math.Round(request.Amount * 0.02m, 2),
                    PaymentDate = DateTime.UtcNow
                };

                LogPaymentConfirmed(_logger, confirmation.ConfirmationNumber, confirmation.Amount);

                return confirmation;
            }
            catch (CommunicationException ex)
            {
                LogCommunicationErrorWithAlacritiApi(_logger, ex);
                return new OrbipayConfirmationResult
                {
                    Success = false,
                    ErrorDescription = "Unable to reach the payment provider. Please try again."
                };
            }
        }
        catch (Exception ex)
        {
            LogErrorConfirmingOrbipayPayment(_logger, ex);
            return new OrbipayConfirmationResult
            {
                Success = false,
                ErrorDescription = "An error occurred while processing your payment."
            };
        }
    }

    /// <summary>Builds the Orbipay hosted form HTML markup.</summary>
    private static string BuildOrbipayFormMarkup(
        string hostedFormUrl,
        string formId,
        string scriptId,
        string clientKey,
        string firstName,
        string lastName,
        string email,
        string addressLine1,
        string? addressLine2,
        string city,
        string? state,
        string zip,
        string country,
        decimal amount,
        string locale)
    {
        var sb = new StringBuilder();
        var quote = '"';

        sb.AppendLine($"<form id={quote}{formId}{quote} action={quote}card-payment-post{quote} method={quote}POST{quote}>");
        sb.AppendLine($"<script id={quote}{scriptId}{quote} src={quote}{hostedFormUrl}{quote}");
        sb.AppendLine($"data-id_customer={quote}CUSTOMER_ID{quote}"); // Replaced at runtime
        sb.AppendLine($"data-customer_account_reference={quote}ACCOUNT_REF{quote}"); // Replaced at runtime
        sb.AppendLine($"data-customer_email={quote}{HtmlEncode(email)}{quote}");
        sb.AppendLine($"data-customer_first_name={quote}{HtmlEncode(firstName)}{quote}");
        sb.AppendLine($"data-customer_last_name={quote}{HtmlEncode(lastName)}{quote}");
        sb.AppendLine($"data-customer_address_line1={quote}{HtmlEncode(addressLine1)}{quote}");
        sb.AppendLine($"data-customer_address_line2={quote}{HtmlEncode(addressLine2 ?? string.Empty)}{quote}");
        sb.AppendLine($"data-customer_city={quote}{HtmlEncode(city)}{quote}");
        sb.AppendLine($"data-customer_state={quote}{HtmlEncode(state ?? string.Empty)}{quote}");
        sb.AppendLine($"data-customer_country={quote}{HtmlEncode(country)}{quote}");
        sb.AppendLine($"data-customer_zip_code1={quote}{HtmlEncode(zip)}{quote}");
        sb.AppendLine($"data-customer_postal_code={quote}{HtmlEncode(zip)}{quote}");
        sb.AppendLine($"data-payment_option={quote}card{quote}");
        sb.AppendLine($"data-payment_option_readonly={quote}true{quote}");
        sb.AppendLine($"data-client_key={quote}{clientKey}{quote}");
        sb.AppendLine($"data-api_event={quote}create_payment{quote}");
        sb.AppendLine($"data-amount={quote}{amount:F2}{quote}");
        sb.AppendLine($"data-locale={quote}{locale}{quote}>");
        sb.AppendLine("</script>");
        sb.AppendLine("</form>");

        return sb.ToString();
    }

    /// <summary>Builds custom fields for Orbipay payment (max 64 chars per field).</summary>
    private Dictionary<string, string> BuildCustomFields(OrbipayPaymentConfirmationRequest request)
    {
        return new Dictionary<string, string>
        {
            { "cdf001", Truncate(request.EmployerLegalName, 64) },
            { "cdf002", "Employer Portal" },
            { "cdf003", Truncate(request.UIAccountNumber, 64) },
            { "cdf004", Truncate(request.EmployerAccountNumber, 64) },
            { "cdf005", request.IsVoluntary ? "Voluntary" : "Employer" },
            { "cdf006", Truncate(_config["Orbipay:Website"] ?? string.Empty, 64) },
            { "cdf007", Truncate(_config["Orbipay:PhoneNumber"] ?? string.Empty, 64) },
            { "cdf008", Truncate(request.EmployerAccountNumber[..Math.Min(5, request.EmployerAccountNumber.Length)], 64) }
        };
    }

    private static string Truncate(string? value, int maxLength)
    {
        return string.IsNullOrEmpty(value)
            ? string.Empty
            : value.Length > maxLength
                ? value[..maxLength]
                : value;
    }

    private static string HtmlEncode(string? value)
    {
        return System.Net.WebUtility.HtmlEncode(value ?? string.Empty);
    }

    #region LoggerMessage Delegates

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Error,
        Message = "Orbipay configuration section not found in appsettings")]
    private static partial void LogOrbipayConfigMissing(ILogger logger);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Error,
        Message = "Orbipay HostedFormUrl or ClientKey is missing")]
    private static partial void LogOrbipayConfigIncomplete(ILogger logger);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Information,
        Message = "Orbipay hosted form session created for amount {Amount}")]
    private static partial void LogHostedFormSessionCreated(ILogger logger, decimal amount);

    [LoggerMessage(
        EventId = 1004,
        Level = LogLevel.Error,
        Message = "Error creating Orbipay hosted form session")]
    private static partial void LogErrorCreatingHostedFormSession(ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1005,
        Level = LogLevel.Error,
        Message = "Orbipay credentials incomplete")]
    private static partial void LogOrbipayCredentialsIncomplete(ILogger logger);

    [LoggerMessage(
        EventId = 1006,
        Level = LogLevel.Information,
        Message = "Payment confirmed: ConfirmationNumber={ConfirmationNumber}, Amount={Amount}")]
    private static partial void LogPaymentConfirmed(ILogger logger, string confirmationNumber, decimal? amount);

    [LoggerMessage(
        EventId = 1007,
        Level = LogLevel.Error,
        Message = "Communication error with Alacriti payment API")]
    private static partial void LogCommunicationErrorWithAlacritiApi(ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1008,
        Level = LogLevel.Error,
        Message = "Error confirming Orbipay payment")]
    private static partial void LogErrorConfirmingOrbipayPayment(ILogger logger, Exception ex);

    #endregion
}
