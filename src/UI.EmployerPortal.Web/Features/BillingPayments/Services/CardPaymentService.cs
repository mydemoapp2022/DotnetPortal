using System.Globalization;
using System.ServiceModel;
using System.Text;
using Com.Alacriti.Checkout;
using Com.Alacriti.Checkout.Api;
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
    private readonly ICardPaymentSystem _cardPaymentSystem;
    private static readonly object CheckoutInitLock = new();
    private static bool _checkoutInitialized;

    public CardPaymentService(
        IConfiguration config,
        ILogger<CardPaymentService> logger,
        IHttpContextAccessor httpContextAccessor,
        ICardPaymentSystem cardPaymentSystem)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _cardPaymentSystem = cardPaymentSystem ?? throw new ArgumentNullException(nameof(cardPaymentSystem));
        EnsureCheckoutInitialized();
    }

    /// <summary>
    /// Initializes Orbipay checkout SDK configuration one time per app domain.
    /// </summary>
    private void EnsureCheckoutInitialized()
    {
        if (_checkoutInitialized)
        {
            return;
        }

        lock (CheckoutInitLock)
        {
            if (_checkoutInitialized)
            {
                return;
            }

            var path = _config["Orbipay:CheckoutConfigPath"];
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(AppContext.BaseDirectory, path);
            }

            if (!File.Exists(path))
            {
                return;
            }

            Checkout.initProperties(path);
            _checkoutInitialized = true;
        }
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
            var orbipaySection = await _cardPaymentSystem.GetEbillConfigurationAsync();
            var clientKey = orbipaySection.TaxClientKey;
            var signatureKey = orbipaySection.TaxSecretKey;
            var clientApiKey = orbipaySection.TaxApiKey;
            var clientPrivateKey = orbipaySection.TaxPrivateKey;
            var hwfPublicKey = orbipaySection.PublicKey;
            var liveMode = orbipaySection.LiveMode == true ? "true" : "false";

            if (string.IsNullOrWhiteSpace(clientKey) ||
                string.IsNullOrWhiteSpace(signatureKey) ||
                string.IsNullOrWhiteSpace(clientApiKey) ||
                string.IsNullOrWhiteSpace(clientPrivateKey) ||
                string.IsNullOrWhiteSpace(hwfPublicKey))
            {
                LogOrbipayCredentialsIncomplete(_logger);
                return new OrbipayConfirmationResult
                {
                    Success = false,
                    ErrorDescription = "Payment provider credentials are incomplete."
                };
            }

            var customFields = BuildCustomFields(request);
            var invocationContext = new InvocationContext(clientApiKey, clientPrivateKey, hwfPublicKey);

            var payment = new Com.Alacriti.Checkout.Api.Payment(customerAccountReference, request.Amount.ToString("F2", CultureInfo.InvariantCulture))
                .withToken(token, digiSign)
                .forClient(clientKey, signatureKey, clientApiKey)
                .withCustomFields(customFields)
                .confirm(invocationContext, liveMode);

            if (payment is not null && payment.Error is null)
            {
                var convenienceFee = 0m;
                if (!string.IsNullOrWhiteSpace(payment.Fee?.Feeamount))
                {
                    _ = decimal.TryParse(payment.Fee.Feeamount, NumberStyles.Number, CultureInfo.InvariantCulture, out convenienceFee);
                }

                var amount = request.Amount;
                if (!string.IsNullOrWhiteSpace(payment.Amount))
                {
                    _ = decimal.TryParse(payment.Amount, NumberStyles.Number, CultureInfo.InvariantCulture, out amount);
                }

                DateTime? paymentDate = null;
                if (!string.IsNullOrWhiteSpace(payment.PaymentDate) &&
                    DateTime.TryParse(payment.PaymentDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsedDate))
                {
                    paymentDate = parsedDate;
                }

                var lastFour = payment.FundingAccount?.AccountNumber;
                if (!string.IsNullOrWhiteSpace(lastFour) && lastFour.Length > 4)
                {
                    lastFour = lastFour[^4..];
                }

                LogPaymentConfirmed(_logger, payment.ConfirmationNumber, amount);

                return new OrbipayConfirmationResult
                {
                    Success = true,
                    ConfirmationNumber = payment.ConfirmationNumber,
                    Amount = amount,
                    PaymentMethod = payment.PaymentMethod,
                    LastFourDigits = lastFour,
                    ConvenienceFee = convenienceFee == 0m ? null : convenienceFee,
                    PaymentDate = paymentDate
                };
            }

            var errors = payment?.Error?.ToList() ?? [];
            var errorMessage = string.Join(" ", errors.Select(e => e.Message).Where(m => !string.IsNullOrWhiteSpace(m)));
            var errorField = string.Join(" ", errors.Select(e => e.Field).Where(f => !string.IsNullOrWhiteSpace(f)));
            var errorCode = string.Join(" ", errors.Select(e => e.Code).Where(c => !string.IsNullOrWhiteSpace(c)));

            return new OrbipayConfirmationResult
            {
                Success = false,
                ErrorDescription = string.IsNullOrWhiteSpace(errorMessage) ? "Card payment failed." : errorMessage,
                ErrorField = errorField,
                ErrorCode = errorCode
            };
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

        sb.AppendLine($"<form id={quote}{formId}{quote} action={quote}/billing-payments/card-payment{quote} method={quote}POST{quote}>");
        sb.AppendLine($"<script id={quote}{scriptId}{quote} src={quote}{hostedFormUrl}{quote}");
        sb.AppendLine($"data-prevent_posting={quote}true{quote}");
        sb.AppendLine($"data-client_key={quote}{clientKey}{quote}");
        sb.AppendLine($"data-api_event={quote}create_payment{quote}");
        sb.AppendLine($"data-payment_option={quote}card{quote}");
        sb.AppendLine($"data-payment_option_readonly={quote}true{quote}");
        sb.AppendLine($"data-amount={quote}{amount.ToString("F2", CultureInfo.InvariantCulture)}{quote}");
        sb.AppendLine($"data-customer_first_name={quote}{HtmlEncode(firstName)}{quote}");
        sb.AppendLine($"data-customer_last_name={quote}{HtmlEncode(lastName)}{quote}");
        sb.AppendLine($"data-customer_email={quote}{HtmlEncode(email)}{quote}");
        sb.AppendLine($"data-customer_address_line1={quote}{HtmlEncode(addressLine1)}{quote}");
        sb.AppendLine($"data-customer_address_line2={quote}{HtmlEncode(addressLine2 ?? string.Empty)}{quote}");
        sb.AppendLine($"data-customer_city={quote}{HtmlEncode(city)}{quote}");
        sb.AppendLine($"data-customer_state={quote}{HtmlEncode(state ?? string.Empty)}{quote}");
        sb.AppendLine($"data-customer_country={quote}{HtmlEncode(country)}{quote}");
        sb.AppendLine($"data-customer_zip_code1={quote}{HtmlEncode(zip)}{quote}");
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
