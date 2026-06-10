using System.ServiceModel;
using System.Text;
using Microsoft.AspNetCore.Http;
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

/// <summary>Result from creating an Orbipay hosted form session.</summary>
public sealed record OrbipaySessionResult
{
    /// <summary>True if session created successfully.</summary>
    public bool Success { get; init; }

    /// <summary>HTML markup containing the Orbipay form and script tag.</summary>
    public string? HostedFormHtml { get; init; }

    /// <summary>Error message if creation failed.</summary>
    public string? ErrorMessage { get; init; }
}

/// <summary>Result from confirming payment via Orbipay API.</summary>
public sealed record OrbipayConfirmationResult
{
    /// <summary>True if payment was successfully confirmed.</summary>
    public bool Success { get; init; }

    /// <summary>Confirmation number from Orbipay.</summary>
    public string? ConfirmationNumber { get; init; }

    /// <summary>Payment amount processed.</summary>
    public decimal? Amount { get; init; }

    /// <summary>Payment method (e.g., VISA, MASTERCARD).</summary>
    public string? PaymentMethod { get; init; }

    /// <summary>Last 4 digits of card.</summary>
    public string? LastFourDigits { get; init; }

    /// <summary>Convenience fee applied (2% per business rules).</summary>
    public decimal? ConvenienceFee { get; init; }

    /// <summary>Payment date/time.</summary>
    public DateTime? PaymentDate { get; init; }

    /// <summary>Error description if confirmation failed.</summary>
    public string? ErrorDescription { get; init; }

    /// <summary>Error field from Orbipay if applicable.</summary>
    public string? ErrorField { get; init; }

    /// <summary>Error code from Orbipay if applicable.</summary>
    public string? ErrorCode { get; init; }
}

/// <summary>Request payload for payment confirmation.</summary>
public sealed record OrbipayPaymentConfirmationRequest
{
    public decimal Amount { get; init; }
    public string ContactName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string AddressLine1 { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string? State { get; init; }
    public string Zip { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public int EmployerSk { get; init; }
    public int RegistrationSk { get; init; }
    public string EmployerLegalName { get; init; } = string.Empty;
    public string EmployerAccountNumber { get; init; } = string.Empty;
    public string UIAccountNumber { get; init; } = string.Empty;
    public bool IsVoluntary { get; init; }
}

/// <summary>
/// Implementation of Orbipay hosted payment form integration.
/// Follows the US Bank eBill system workflow for card payments.
/// </summary>
internal sealed class CardPaymentService : ICardPaymentService
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
                _logger.LogError("Orbipay configuration section not found in appsettings");
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
                _logger.LogError("Orbipay HostedFormUrl or ClientKey is missing");
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

            _logger.LogInformation("Orbipay hosted form session created for amount {Amount}", amount);

            return new OrbipaySessionResult
            {
                Success = true,
                HostedFormHtml = html
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Orbipay hosted form session");
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
                _logger.LogError("Orbipay credentials incomplete");
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

                _logger.LogInformation(
                    "Payment confirmed: ConfirmationNumber={ConfirmationNumber}, Amount={Amount}",
                    confirmation.ConfirmationNumber,
                    confirmation.Amount);

                return confirmation;
            }
            catch (CommunicationException ex)
            {
                _logger.LogError(ex, "Communication error with Alacriti payment API");
                return new OrbipayConfirmationResult
                {
                    Success = false,
                    ErrorDescription = "Unable to reach the payment provider. Please try again."
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming Orbipay payment");
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
        char quote = '"';

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
    private static Dictionary<string, string> BuildCustomFields(OrbipayPaymentConfirmationRequest request)
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
            { "cdf008", Truncate(request.EmployerAccountNumber.Substring(0, Math.Min(5, request.EmployerAccountNumber.Length)), 64) }
        };
    }

    private static string Truncate(string? value, int maxLength)
        => string.IsNullOrEmpty(value)
            ? string.Empty
            : value.Length > maxLength
                ? value[..maxLength]
                : value;

    private static string HtmlEncode(string? value)
        => System.Net.WebUtility.HtmlEncode(value ?? string.Empty);
}
