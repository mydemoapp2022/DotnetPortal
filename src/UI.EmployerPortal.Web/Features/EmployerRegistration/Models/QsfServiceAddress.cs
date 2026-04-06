namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// Wisconsin address where services were performed for a Qualified Settlement Fund.
/// </summary>
public class QsfServiceAddress
{
    /// <summary>Address Line 1.</summary>
    public string? AddressLine1 { get; set; }

    /// <summary>Address Line 2 (optional).</summary>
    public string? AddressLine2 { get; set; }

    /// <summary>City.</summary>
    public string? City { get; set; }

    /// <summary>State (defaults to Wisconsin).</summary>
    public string State { get; set; } = "Wisconsin";

    /// <summary>Zip code (5 digits).</summary>
    public string? ZipCode { get; set; }

    /// <summary>Zip extension (optional, 4 digits).</summary>
    public string? ZipExtension { get; set; }
}
