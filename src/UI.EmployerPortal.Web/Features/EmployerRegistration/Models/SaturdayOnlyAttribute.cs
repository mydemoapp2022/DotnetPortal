using System.ComponentModel.DataAnnotations;

namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// Determines if the date is a Saturday
/// </summary>
public class SaturdayOnlyAttribute : ValidationAttribute
{

    /// <summary>
    /// Only Valid if the Date is a Saturday
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return value is DateTime dateTime
            ? dateTime.DayOfWeek == DayOfWeek.Saturday
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage ?? "The selected date must be a Saturday.", new[] { validationContext.MemberName! })
            : ValidationResult.Success;
    }
}
