namespace MyHomeApp.Services;

/// <summary>
/// Service interface for managing application theme
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Sets the application theme
    /// </summary>
    /// <param name="appTheme">The theme to apply</param>
    void SetTheme(AppTheme appTheme);

    /// <summary>
    /// Gets the user-selected theme
    /// </summary>
    AppTheme UserAppTheme { get; }

    /// <summary>
    /// Gets the currently requested theme (may differ from UserAppTheme if set to Unspecified)
    /// </summary>
    AppTheme RequestedTheme { get; }

    /// <summary>
    /// Toggles between Light and Dark themes
    /// </summary>
    void ToggleTheme();
}
