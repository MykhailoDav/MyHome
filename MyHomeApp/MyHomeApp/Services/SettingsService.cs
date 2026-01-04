using System.Globalization;

namespace MyHomeApp.Services;

/// <summary>
/// Service for managing application settings like theme and language
/// </summary>
public partial class SettingsService : ObservableObject, ISettingsService
{
    readonly IThemeService themeService;
    readonly ILocalizationResourceManager localizationService;

    [ObservableProperty]
    AppTheme currentTheme;

    [ObservableProperty]
    string currentLanguage;

    [ObservableProperty]
    LanguageOption? selectedLanguage;

    [ObservableProperty]
    List<LanguageOption> availableLanguages;

    public SettingsService(IThemeService themeService, ILocalizationResourceManager localizationService)
    {
        this.themeService = themeService;
        this.localizationService = localizationService;

        // Initialize available languages
        AvailableLanguages = GetAvailableLanguages();

        // Load saved settings or use defaults
        CurrentTheme = LoadTheme();
        CurrentLanguage = LoadLanguage();

        // Set selected language
        SelectedLanguage = AvailableLanguages.FirstOrDefault(l => l.Code == CurrentLanguage);

        ApplyTheme(CurrentTheme);
        ApplyLanguage(CurrentLanguage);
    }

    partial void OnCurrentThemeChanged(AppTheme value)
    {
        ApplyTheme(value);
        SaveTheme(value);
    }

    partial void OnCurrentLanguageChanged(string value)
    {
        ApplyLanguage(value);
        SaveLanguage(value);
    }

    partial void OnSelectedLanguageChanged(LanguageOption? value)
    {
        if (value != null && value.Code != CurrentLanguage)
        {
            CurrentLanguage = value.Code;
        }
    }

    void ApplyTheme(AppTheme theme)
    {
        themeService.SetTheme(theme);
    }

    void ApplyLanguage(string language)
    {
        var culture = language switch
        {
            AppConstants.Cultures.Ukrainian => new CultureInfo(AppConstants.Cultures.UkrainianFull),
            AppConstants.Cultures.English => new CultureInfo(AppConstants.Cultures.EnglishFull),
            _ => new CultureInfo(AppConstants.Cultures.EnglishFull)
        };

        localizationService.CurrentCulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    AppTheme LoadTheme()
    {
        if (Preferences.ContainsKey(AppConstants.PreferencesKeys.Theme))
        {
            var themeString = Preferences.Get(AppConstants.PreferencesKeys.Theme, AppConstants.Defaults.Theme.ToString());
            return Enum.Parse<AppTheme>(themeString);
        }
        return AppConstants.Defaults.Theme;
    }

    void SaveTheme(AppTheme theme)
    {
        Preferences.Set(AppConstants.PreferencesKeys.Theme, theme.ToString());
    }

    string LoadLanguage()
    {
        return Preferences.Get(AppConstants.PreferencesKeys.Language, AppConstants.Defaults.Language);
    }

    void SaveLanguage(string language)
    {
        Preferences.Set(AppConstants.PreferencesKeys.Language, language);
    }

    public void ToggleTheme()
    {
        CurrentTheme = CurrentTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;
    }

    public List<LanguageOption> GetAvailableLanguages()
    {
        return
        [
            new() { Code = AppConstants.Cultures.English, Name = "English" },
            new() { Code = AppConstants.Cultures.Ukrainian, Name = "Українська" }
        ];
    }
}

public class LanguageOption
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
