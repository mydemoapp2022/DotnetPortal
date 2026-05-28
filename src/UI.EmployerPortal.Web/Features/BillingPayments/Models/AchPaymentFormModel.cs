//using System.ComponentModel.DataAnnotations;

namespace UI.EmployerPortal.Web.Features.BillingPayments.Models;

/// <summary>
/// AchPaymentFormModel
/// </summary>
public class AchPaymentFormModel
{
    /// <summary>
    /// Amount
    /// </summary>
    //[Required(ErrorMessage = "Payment Amount is required.")]
    public string? Amount { get; set; }

    /// <summary>
    /// SettlementDate
    /// </summary>
    //[Required(ErrorMessage = "Settlement Date is required.")]
    public string? SettlementDate { get; set; }


    /// <summary>Validated decimal amount. Null until the user enters a valid value.</summary>
    public decimal? ValidatedAmount { get; set; }

    /// <summary>Validated settlement date. Null until the user selects a valid date.</summary>
    public DateOnly? ValidatedSettlementDate { get; set; }
}
