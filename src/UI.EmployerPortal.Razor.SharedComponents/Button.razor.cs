using Microsoft.AspNetCore.Components;

namespace UI.EmployerPortal.Razor.SharedComponents;

/// <summary>
/// Reusable button component with configurable variants and sizes
/// </summary>
public partial class Button
{
    /// <summary>
    /// Gets or sets the button text
    /// </summary>
    [Parameter]
    public string Text { get; set; } = "";

    /// <summary>
    /// Gets or sets the button variant (Primary or Secondary).
    /// Primary: filled blue background with white text.
    /// Secondary: white background with blue text and border.
    /// </summary>
    [Parameter]
    public ButtonVariant Variant { get; set; } = ButtonVariant.Primary;

    /// <summary>
    /// Gets or sets the button size (Small, medium or Large)
    /// </summary>
    [Parameter]
    public ButtonSize Size { get; set; } = ButtonSize.Medium;

    /// <summary>
    /// Gets or sets the button type attribute (button, submit, reset).
    /// </summary>
    [Parameter]
    public string Type { get; set; } = "button";

    /// <summary>
    /// Gets or sets whether the button is disabled. 
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the aria-label for accessibility.
    /// If not provided.the button text is used. 
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Controls whether the button border is visible. Default: true
    /// </summary>
    [Parameter]
    public bool ShowBorder { get; set; } = true;

    /// <summary>
    /// Optional child content. When provided, renders inside the button instead of <see cref="Text"/>
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Event callback invoked when the button is clicked.
    /// </summary>
    [Parameter]
    public EventCallback OnClick { get; set; }

    private string VariantClass => Variant switch
    {
        ButtonVariant.Primary => "btn-primary-variant",
        ButtonVariant.Secondary => "btn-secondary-variant",
        _ => "btn-primary-variant"
    };

    private string SizeClass => Size switch
    {
        ButtonSize.Small => "btn-sm",
        ButtonSize.Medium => "btn-md",
        ButtonSize.Large => "btn-lg",
        _ => "btn-md"
    };

    private async Task HandleClick()
    {
        if (!Disabled)
        {
            await OnClick.InvokeAsync();
        }
    }

    /// <summary>
    /// Button size options
    /// </summary>
    public enum ButtonSize
    {
        /// <summary>
        /// Small button with compact padding
        /// </summary>
        Small,

        /// <summary>
        /// Medium button with standard padding (default)
        /// </summary>
        Medium,

        /// <summary>
        /// Large button with expanded padding.
        /// </summary>
        Large
    }

    /// <summary>
    /// Button visual variant
    /// </summary>
    public enum ButtonVariant
    {
        /// <summary>
        /// Filled button with primary color background and white text;
        /// </summary>
        Primary,

        /// <summary>
        /// Outlined button with white background and primary color text/border.
        /// </summary>
        Secondary
    }
}
