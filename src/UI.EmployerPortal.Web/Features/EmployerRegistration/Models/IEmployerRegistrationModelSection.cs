namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// Interface to make sure all the registration child models implement a similar method for exporting survey resonses
/// </summary>
public interface IEmployerRegistrationModelSection
{
    /// <summary>
    /// This should return a list of the survey responses from their 
    /// </summary>
    /// <returns></returns>
    List<SurveyResponse> GetSurveyResponses();

    /// <summary>
    /// Convert a boolean response to the string value expected by the save response API call
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    static string ConvertBooleanResponseToString(bool response)
    {
        return response ? "Yes" : "No";
    }

    /// <summary>
    /// Returns a string that is the legal name concatenated in forward (firstName lastName) order
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="middleInitial"></param>
    /// <param name="lastName"></param>
    /// <returns></returns>
    static string ConcatenateLegalName(string firstName, string middleInitial, string lastName)
    {
        return $"{firstName} {middleInitial}. {lastName}";
    }
}
