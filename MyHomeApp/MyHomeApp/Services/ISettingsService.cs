using System.Globalization;

namespace MyHomeApp.Services;

/// <summary>
/// Service interface for managing application settings
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Gets or sets the current application theme
    /// </summary>
    AppTheme CurrentTheme { get; set; }

    /// <summary>
    /// Gets or sets the current language code
    /// </summary>
    string CurrentLanguage { get; set; }

    /// <summary>
    /// Gets or sets the selected language option
    /// </summary>
    LanguageOption? SelectedLanguage { get; set; }

    /// <summary>
    /// Gets the list of available languages
    /// </summary>
    List<LanguageOption> AvailableLanguages { get; }

    /// <summary>
    /// Toggles between Light and Dark themes
    /// </summary>
    void ToggleTheme();

    /// <summary>
    /// Gets the list of available language options
    /// </summary>
    List<LanguageOption> GetAvailableLanguages();
}
