//using System.ComponentModel.DataAnnotations;
namespace UI.EmployerPortal.Razor.SharedComponents.Model;

/// <summary>
/// Represents a postal address, including details such as street, city, state, and postal code.
/// </summary>
public class AddressModel
{
    /// <summary>
    /// Name
    /// </summary>
    //[RequiredIfNameVisible("IsNameVisible", ErrorMessage = "Name Required")]
    public string? Name { get; set; }


}
