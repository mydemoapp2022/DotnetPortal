using System.ComponentModel.DataAnnotations;

namespace UI.EmployerPortal.Web.Features.BillingPayments.Models;

/// <summary>
/// Form model for adding a bank account.
/// Validates required fields and cross-field rules via <see cref="IValidatableObject"/>.
/// </summary>
public class BankAccountModel : IValidatableObject
{
    /// <summary>
    /// User-defined label for this bank account. Must be unique across the employer's accounts.
    /// </summary>
    [Required(ErrorMessage = "Bank Account Nickname is required")]
    [MaxLength(50, ErrorMessage = "Bank Account Nickname cannot exceed 50 characters")]
    public string? Nickname { get; set; }

    /// <summary>
    /// ABA routing number. Must be exactly 9 digits.
    /// </summary>
    [Required(ErrorMessage = "Bank Routing Number is required")]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "Bank Routing Number must be exactly 9 digits")]
    public string? RoutingNumber { get; set; }

    /// <summary>
    /// Bank account number. Must be digits only, up to 17 characters.
    /// </summary>
    [Required(ErrorMessage = "Bank Account Number is required")]
    [MaxLength(17, ErrorMessage = "Bank Account Number cannot exceed 17 digits")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Bank Account Number must contain digits only")]
    public string? AccountNumber { get; set; }

    /// <summary>
    /// Confirmation entry for <see cref="AccountNumber"/>. Must match exactly.
    /// </summary>
    [Required(ErrorMessage = "Re-enter Account Number is required")]
    public string? ConfirmAccountNumber { get; set; }

    /// <summary>
    /// Bank name populated from the routing number lookup. Read-only on the form.
    /// </summary>
    public string? BankName { get; set; }

    /// <summary>
    /// Account type — Checking or Savings.
    /// </summary>
    [Required(ErrorMessage = "Account Type is required")]
    public string? AccountType { get; set; }

    /// <summary>
    /// Indicates the account is funded by a transfer from a financial institution outside the U.S. (IAT).
    /// When true, the IAT address fields are required.
    /// </summary>
    public bool IsInternational { get; set; }

    /// <summary>
    /// ISO numeric country code for the foreign financial institution.
    /// Required when <see cref="IsInternational"/> is true.
    /// </summary>
    public int IatCountryCode { get; set; }

    /// <summary>
    /// Street address of the foreign financial institution.
    /// Required when <see cref="IsInternational"/> is true.
    /// </summary>
    public string? IatStreetAddress { get; set; }

    /// <summary>
    /// City of the foreign financial institution.
    /// Required when <see cref="IsInternational"/> is true.
    /// </summary>
    public string? IatCity { get; set; }

    /// <summary>
    /// Postal code of the foreign financial institution. Optional.
    /// </summary>
    public string? IatPostalCode { get; set; }

    /// <inheritdoc/>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(AccountNumber) &&
            !string.IsNullOrWhiteSpace(ConfirmAccountNumber) &&
            AccountNumber != ConfirmAccountNumber)
        {
            yield return new ValidationResult(
                "Account numbers do not match",
                [nameof(ConfirmAccountNumber)]);
        }

        if (IsInternational)
        {
            if (IatCountryCode == 0)
            {
                yield return new ValidationResult("Country is required", [nameof(IatCountryCode)]);
            }

            if (string.IsNullOrWhiteSpace(IatStreetAddress))
            {
                yield return new ValidationResult("Street Address is required", [nameof(IatStreetAddress)]);
            }

            if (string.IsNullOrWhiteSpace(IatCity))
            {
                yield return new ValidationResult("City is required", [nameof(IatCity)]);
            }
        }
    }
}
