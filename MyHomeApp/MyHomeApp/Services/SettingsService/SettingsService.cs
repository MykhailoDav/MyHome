using System.Globalization;

namespace MyHomeApp.Services;

public partial class SettingsService : ObservableObject, ISettingsService
{
    readonly IThemeService themeService;
    readonly ILocalizationResourceManager localizationService;

    [ObservableProperty]
    AppTheme currentTheme;

    [ObservableProperty]
    LanguageOption? currentLanguage;

    [ObservableProperty]
    List<LanguageOption> availableLanguages;

    public SettingsService(IThemeService themeService, ILocalizationResourceManager localizationService)
    {
        this.themeService = themeService;
        this.localizationService = localizationService;

        AvailableLanguages = GetAvailableLanguages();
       CurrentTheme = LoadTheme();

        string? codeToUse = LoadLanguage();

        if (string.IsNullOrEmpty(codeToUse))
        {
            string systemCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            if (AvailableLanguages.Any(l => l.Code == systemCode))
            {
                codeToUse = systemCode;
            }
            else
            {
                codeToUse = AppConstants.Defaults.Language;
            }
        }

        CurrentLanguage = AvailableLanguages.FirstOrDefault(l => l.Code == codeToUse)
                          ?? AvailableLanguages.First();

        ApplyTheme(CurrentTheme);
        ApplyLanguage(CurrentLanguage);
        SaveLanguage(CurrentLanguage);
    }

    partial void OnCurrentThemeChanged(AppTheme value)
    {
        ApplyTheme(value);
        SaveTheme(value);
    }

    partial void OnCurrentLanguageChanged(LanguageOption? oldValue, LanguageOption? newValue)
    {
        if (newValue is not null && newValue.Code != oldValue?.Code)
        {
            ApplyLanguage(newValue);
            SaveLanguage(newValue);
        }
    }

    void ApplyTheme(AppTheme theme) => themeService.SetTheme(theme);

    void ApplyLanguage(LanguageOption language)
    {
        if (language is null) return;

        var culture = language.Code switch
        {
            AppConstants.Cultures.Ukrainian => new CultureInfo(AppConstants.Cultures.UkrainianFull),
            AppConstants.Cultures.English => new CultureInfo(AppConstants.Cultures.EnglishFull),
            _ => new CultureInfo(AppConstants.Cultures.EnglishFull)
        };

        localizationService.CurrentCulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    static AppTheme LoadTheme() => Enum.Parse<AppTheme>(Preferences.Get(AppConstants.PreferencesKeys.Theme, AppConstants.Defaults.Theme.ToString()));

    static void SaveTheme(AppTheme theme) => Preferences.Set(AppConstants.PreferencesKeys.Theme, theme.ToString());

    static string? LoadLanguage() => Preferences.Get(AppConstants.PreferencesKeys.Language, null);

    static void SaveLanguage(LanguageOption language) => Preferences.Set(AppConstants.PreferencesKeys.Language, language.Code);

    public void ToggleTheme() => CurrentTheme = CurrentTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;

    public List<LanguageOption> GetAvailableLanguages() =>
        [
            new() { Code = AppConstants.Cultures.English, Name = "English" },
            new() { Code = AppConstants.Cultures.Ukrainian, Name = "Українська" }
        ];
}