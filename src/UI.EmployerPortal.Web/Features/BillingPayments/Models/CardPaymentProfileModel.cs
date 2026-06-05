using System.ComponentModel.DataAnnotations;

namespace UI.EmployerPortal.Web.Features.BillingPayments.Models;

/// <summary>
/// Holds the billing/contact profile data collected on the
/// Credit/Debit Card Registration page (Step 1).
/// </summary>
public class CardPaymentProfileModel
{
    // ── Contact ──────────────────────────────────────────────────────────

    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a 10-digit phone number.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    // ── Billing Address ──────────────────────────────────────────────────

    /// <summary>"United States" or another country name.</summary>
    [Required(ErrorMessage = "Country is required.")]
    public string Country { get; set; } = "United States";

    [Required(ErrorMessage = "Address line 1 is required.")]
    [MaxLength(100, ErrorMessage = "Address cannot exceed 100 characters.")]
    public string AddressLine1 { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? AddressLine2 { get; set; }

    [Required(ErrorMessage = "City is required.")]
    [MaxLength(60)]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// State abbreviation (e.g. "WI"). Required only when Country is "United States".
    /// </summary>
    public string? State { get; set; }

    [Required(ErrorMessage = "Zip code is required.")]
    [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Enter a valid zip code (e.g. 53201 or 53201-4302).")]
    public string ZipCode { get; set; } = string.Empty;

    /// <summary>Optional zip extension (the 4-digit part after the dash).</summary>
    public string? ZipExt { get; set; }

    // ── Session ──────────────────────────────────────────────────────────

    /// <summary>Session key used to persist this model across the card payment flow.</summary>
    public const string SessionKey = "CardPaymentProfileModel";
}
