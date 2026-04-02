namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Components;

/// <summary>
/// Represets an option for a radio button input, with a value of type T and a display label.
/// </summary>
/// <typeparam name="T"></typeparam>
public class RadioOption<T>
{
    /// <summary>
    /// Gets or sets the value stored in the container.
    /// </summary>
    public T Value { get; set; } = default!;

    /// <summary>
    /// Gets or sets the display label associated with this instance.
    /// </summary>
    public string Label { get; set; } = string.Empty;
}
