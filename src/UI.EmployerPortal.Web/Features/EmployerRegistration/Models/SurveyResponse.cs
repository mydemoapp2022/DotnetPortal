namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// Used to map a response string to a specific response sk 
/// </summary>
public record SurveyResponse
{
    /// <summary>
    /// Response Item ID for the response
    /// </summary>
    public int _surveyResponseItemSk;

    /// <summary>
    /// String value for the response
    /// </summary>
    public string _response = string.Empty;
}
