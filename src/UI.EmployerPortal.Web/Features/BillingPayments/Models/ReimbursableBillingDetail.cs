using System.ComponentModel.DataAnnotations;

namespace UI.EmployerPortal.Web.Features.BillingPayments.Models;

/// <summary>
/// ReimbursableBillingDetail
/// </summary>
public class ReimbursableBillingDetail
{
    /// <summary>
    /// SessionKey
    /// </summary>
    public const string SessionKey = "ReimbursableBillingDetail";
    /// <summary>
    /// AmountToPay
    /// </summary>
    [PaymentValidation]
    public decimal AmountToPay { get; set; }
    /// <summary>
    /// TotalEFTPayments
    /// </summary>
    public decimal TotalEFTPayments { get; set; }
    /// <summary>
    /// TotalOutstandingBalance
    /// </summary>
    public decimal TotalOutstandingBalance { get; set; }

    /// <summary>
    /// The payment method selected by the user (e.g. "ACH", "Card", "Check").
    /// Persisted in session so it is restored when navigating back.
    /// </summary>
    public string? SelectedPaymentMethod { get; set; }

    /// <summary>
    /// check
    /// </summary>
    public class PaymentValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validates the specified value with respect to the current validation context.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        /// A <see cref="ValidationResult"/> that indicates whether the specified value is valid. Any other validations can be added here as needed, such as ensuring that the sum of employee counts is greater than zero or that wage values are consistent with each other.
        /// </returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is decimal amount && amount <= 0)
            {
                return new ValidationResult("Amount to Pay is required and must be greater than $0.00.");
            }

            return ValidationResult.Success;
        }
    }
}
