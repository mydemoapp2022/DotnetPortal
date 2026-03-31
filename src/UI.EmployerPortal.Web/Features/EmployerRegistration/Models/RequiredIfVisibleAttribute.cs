using System.ComponentModel.DataAnnotations;

namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// The 'Name' field may not be visible on the form.  In that event,
/// the value is not required.
/// </summary>
public class RequiredIfVisibleAttribute : ValidationAttribute
{
    private readonly string _isVisible;
    private readonly bool _expectedValue;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isVisible"></param>
    /// <param name="expectedValue"></param>
    public RequiredIfVisibleAttribute(string isVisible, bool expectedValue)
    {
        _isVisible = isVisible;
        _expectedValue = expectedValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var instance = context.ObjectInstance;
        var isValidationRequired = instance.GetType().GetProperty(_isVisible)?.GetValue(instance, null);

        var isRequired = isValidationRequired is not bool || (bool) isValidationRequired;

        if (isRequired == _expectedValue)
        {
            var name = value == null ? String.Empty : value.ToString();
            return !String.IsNullOrWhiteSpace(name)
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage ?? "Name is required ", new[] { context.MemberName! });
        }

        return ValidationResult.Success;
    }
}
