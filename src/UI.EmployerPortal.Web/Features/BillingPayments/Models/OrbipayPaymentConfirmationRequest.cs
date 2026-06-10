namespace UI.EmployerPortal.Web.Features.BillingPayments.Models;

/// <summary>Request payload for payment confirmation.</summary>
public sealed record OrbipayPaymentConfirmationRequest
{
    /// <summary>Payment amount to be confirmed.</summary>
    public decimal Amount { get; init; }
    /// <summary>Contact name associated with the payment.</summary>
    public string ContactName { get; init; } = string.Empty;
    /// <summary>Email address of the contact.</summary>
    public string Email { get; init; } = string.Empty;
    /// <summary>First line of the billing address.</summary>
    public string AddressLine1 { get; init; } = string.Empty;
    /// <summary>City of the billing address.</summary>
    public string City { get; init; } = string.Empty;
    /// <summary>State of the billing address.</summary>
    public string? State { get; init; }
    /// <summary>ZIP code of the billing address.</summary>
    public string Zip { get; init; } = string.Empty;
    /// <summary>Country of the billing address.</summary>
    public string Country { get; init; } = string.Empty;
    /// <summary>Employer's unique identifier.</summary>
    public int EmployerSk { get; init; }
    /// <summary>Registration's unique identifier.</summary>
    public int RegistrationSk { get; init; }
    /// <summary>Legal name of the employer.</summary>
    public string EmployerLegalName { get; init; } = string.Empty;
    /// <summary>Employer's account number.</summary>
    public string EmployerAccountNumber { get; init; } = string.Empty;
    /// <summary>User interface account number.</summary>
    public string UIAccountNumber { get; init; } = string.Empty;
    /// <summary>Indicates if the payment is voluntary.</summary>
    public bool IsVoluntary { get; init; }
}
