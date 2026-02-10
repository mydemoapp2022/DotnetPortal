using UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Services;

/// <summary>
/// ISSNValidationService
/// </summary>
public interface ISSNValidationService
{
    /// <summary>
    /// Validates an SSN and returns validation result
    /// </summary>
    SSNValidationResult ValidateSSN(string? ssn, string fieldLabel = "SSN");

    /// <summary>
    /// Formats SSN with dashes
    /// </summary>
    string FormatSSN(string value);

    /// <summary>
    /// Masks SSN for display (shows only last 4 digits)
    /// </summary>
    string MaskSSN(string? ssn);

    /// <summary>
    /// Checks for duplicate SSNs in a list
    /// </summary>
    List<string> FindDuplicateSSNs(List<string?> ssnList, Func<int, string> getLabelForIndex);
}

/// <summary>
/// Service for SSN validation and formatting
/// </summary>
public class SSNValidationService : ISSNValidationService
{
    /// <summary>
    /// Validates an SSN and returns validation result
    /// </summary>
    public SSNValidationResult ValidateSSN(string? ssn, string fieldLabel = "SSN")
    {
        var result = new SSNValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(ssn))
        {
            result.IsValid = false;
            result.ErrorMessage = $"{fieldLabel} is required";
            return result;
        }

        // Strip out dashes
        var digitsOnly = new string(ssn.Where(char.IsDigit).ToArray());

        // Check length
        if (digitsOnly.Length != 9)
        {
            result.IsValid = false;
            result.ErrorMessage = $"{fieldLabel} format must be 999-99-9999";
            return result;
        }

        // SSN cannot contain all the same digits
        if (digitsOnly.Distinct().Count() == 1)
        {
            result.IsValid = false;
            result.ErrorMessage = $"{fieldLabel} cannot be all the same character";
            return result;
        }

        // SSN cannot be 123456789
        if (digitsOnly == "123456789")
        {
            result.IsValid = false;
            result.ErrorMessage = $"{fieldLabel} cannot be \"123456789\"";
            return result;
        }

        return result;
    }

    /// <summary>
    /// Formats SSN with dashes
    /// </summary>
    public string FormatSSN(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var digitsOnly = new string(value.Where(char.IsDigit).ToArray());

        if (digitsOnly.Length > 9)
        {
            digitsOnly = digitsOnly[..9];
        }

        return digitsOnly.Length > 5
            ? $"{digitsOnly[..3]}-{digitsOnly.Substring(3, 2)}-{digitsOnly[5..]}"
            : digitsOnly.Length > 3
                ? $"{digitsOnly[..3]}-{digitsOnly[3..]}"
                : digitsOnly;
    }

    /// <summary>
    /// Masks SSN for display (shows only last 4 digits)
    /// </summary>
    public string MaskSSN(string? ssn)
    {
        if (string.IsNullOrWhiteSpace(ssn))
        {
            return string.Empty;
        }

        var digitsOnly = new string(ssn.Where(char.IsDigit).ToArray());

        if (digitsOnly.Length >= 4)
        {
            var lastFour = digitsOnly[^4..];
            return $"***-**-{lastFour}";
        }

        return ssn;
    }

    /// <summary>
    /// Checks for duplicate SSNs in a list
    /// </summary>
    public List<string> FindDuplicateSSNs(List<string?> ssnList, Func<int, string> getLabelForIndex)
    {
        var errors = new List<string>();
        var ssnDict = new Dictionary<string, List<int>>();

        for (var i = 0; i < ssnList.Count; i++)
        {
            var ssn = ssnList[i];
            if (string.IsNullOrWhiteSpace(ssn))
            {
                continue;
            }

            // Strip dashes for comparison
            var digitsOnly = new string(ssn.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length == 9)
            {
                if (!ssnDict.ContainsKey(digitsOnly))
                {
                    ssnDict[digitsOnly] = [];
                }
                ssnDict[digitsOnly].Add(i);
            }
        }

        // Find duplicates
        foreach (var kvp in ssnDict.Where(x =>
        {
            return x.Value.Count > 1;
        }))
        {
            var indices = string.Join(", ", kvp.Value.Select(i =>
            {
                return getLabelForIndex(i);
            }));
            errors.Add($"Duplicate SSN found for: {indices}");
        }
        return errors;
    }
}
