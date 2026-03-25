namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// Pairs a validation error message with the HTML element ID of the field that caused it.
/// Used to drive focus-on-click behaviour in NotificationBanner.
/// </summary>
public record ValidationItem(string Message, string FieldId = "");
