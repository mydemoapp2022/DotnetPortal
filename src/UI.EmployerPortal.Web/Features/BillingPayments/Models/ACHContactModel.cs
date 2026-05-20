using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


namespace UI.EmployerPortal.Web.Features.BillingPayments.Models;
/// <summary>
/// ACH Contact
/// </summary>
public class ACHContactModel
{
    /// <summary>
    /// The first name of the person uploading the file
    /// </summary>
    ///
    [Required(ErrorMessage = "Contact Name is required.")]
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the phone number for upload contact
    /// </summary>
    ///
    [Required(ErrorMessage = "Phone Number is required.")]
    public string PhoneNumber { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the optional phone extension for the upload contact
    /// </summary>
    public string PhoneExt { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the phone number text for upload contact
    /// </summary>
    ///
    [Required(ErrorMessage = "Phone Number Format is required.")]
    public string PhoneNumberFormat { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the email address for the upload contact
    /// </summary>
    ///
    [Required(ErrorMessage = "Email address is required.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the confirmation email address for the upload contact
    /// </summary>
    ///
    [ConfirmEmailvalidation]
    public string ConfirmEmail { get; set; } = string.Empty;
    /// <summary>
    /// Returnvalue
    /// </summary>
    public int WebContactInformationsk { get; set; }
    /// <summary>
    /// InternationalFlag
    /// </summary>
    public bool InternationalFlag { get; set; }

    /// <summary>
    /// check
    /// </summary>
    public class ConfirmEmailvalidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var model = (ACHContactModel) validationContext.ObjectInstance;
            var confirmEmail = value as string;
            if (string.IsNullOrWhiteSpace(confirmEmail))
            {
                return new ValidationResult("Confirm email is required");
            }
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return !regex.IsMatch(confirmEmail)
                ? new ValidationResult("Invalid email format")
                : confirmEmail != model.Email ? new ValidationResult("Email adresses do not match") : ValidationResult.Success;
        }
    }
}
