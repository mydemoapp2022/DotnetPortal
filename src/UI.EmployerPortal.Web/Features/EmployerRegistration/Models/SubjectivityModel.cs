using System.ComponentModel.DataAnnotations;

namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// Model for the UISubjectivity page
/// </summary>
public class SubjectivityModel
{
    /// <summary>
    /// Business Catagory
    /// </summary>
    public BusinessCategory? BusinessCategory { get; set; }

    /// <summary>
    /// HasAppliedFor501c3Status
    /// </summary>
    [Required(ErrorMessage = "Applied for 501(c)(3) status is required.")]
    public bool? HasAppliedFor501c3Status { get; set; }

    /// <summary>
    /// Do you have employees who work in states other than Wisconsin?
    /// </summary>
    [Required(ErrorMessage = "Employees who work in other states is required")]
    public bool? EmployeesInOtherStates { get; set; }

    /// <summary>
    /// Do you have a Federal Unemployment Tax (FUTA) liability based on payrolls in any state other than Wisconsin?
    /// </summary>
    [Required(ErrorMessage = "Federal Unemployment Tax (FUTA) liability is required")]
    public bool? FederalTaxLiability { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [RequiredIfVisible("EmployeesInOtherStates", false, ErrorMessage = "Wages in a calendar quarter is required")]
    public bool? PaidWagesInAQuarterEmployees { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [RequiredIfVisible("FederalTaxLiability", false, ErrorMessage = "Wages in a calendar quarter is required")]
    public bool? PaidWagesInAQuarterTaxes { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [RegularExpression(@"^[1-4]\/\d{4}$", ErrorMessage = "Quarter and Year must be in the format q/yyyy")]
    public string? QuarterYearFirstPaidEmployees { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [RegularExpression(@"^[1-4]\/\d{4}$", ErrorMessage = "Quarter and Year must be in the format q/yyyy")]
    public string? QuarterYearFirstPaidTaxes { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [RequiredIfVisible("FederalTaxLiability", false, ErrorMessage = "Employee question is required")]
    public bool? Employee20Weeks { get; set; }

    /// <summary>
    /// Only Saturday is valid
    /// </summary>
    [Required(ErrorMessage = "A Saturday date must be selected")]
    [SaturdayOnly(ErrorMessage = "The selected date must be a Saturday.")]
    public DateTime? DateWeek20Ended { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? Employee20WeeksFourOrMore { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? HasFederalTaxLiabilityOutsideWisconsin { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public AddressModel FinancialInstitution { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    public bool? ExpectToPayWagesInAQuarter { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string WhenExpectToPayWagesInAQuarter { get; set; } = String.Empty;

    /// <summary>
    /// 
    /// </summary>
    public required List<YearQuartersPaidWages> Wages { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public SubjectivityModel() { }

}
