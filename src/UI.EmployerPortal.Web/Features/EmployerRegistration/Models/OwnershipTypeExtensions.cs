using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// Extension methods for OwnershipType enum
/// </summary>
public static class OwnershipTypeExtensions
{
    /// <summary>
    /// Gets the display name from the Display attribute
    /// </summary>
    public static string GetDisplayName(this OwnershipType ownershipType)
    {
        var displayAttribute = ownershipType
            .GetType()
            .GetMember(ownershipType.ToString())
            .FirstOrDefault()
            ?.GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? ownershipType.ToString();
    }

    /// <summary>
    /// Determines whether the ownership type is a governmental classification.
    /// Governmental types: City, County, Federal, Indian Tribe, Other Government Local,
    /// Other Government State, School District, State, Township, Village.
    /// </summary>
    public static bool IsGovernmental(this OwnershipType ownershipType)
    {
        return ownershipType is
            OwnershipType.CityGovernmentAgency or
            OwnershipType.CountyGovernmentAgency or
            OwnershipType.FederalGovernmentAgency or
            OwnershipType.IndianTribe or
            OwnershipType.LocalGovernmentUnitNotListed or
            OwnershipType.StateGovernmentUnitNotListed or
            OwnershipType.SchoolDistrict or
            OwnershipType.StateGovernmentAgency or
            OwnershipType.Township or
            OwnershipType.Village;
    }
}
