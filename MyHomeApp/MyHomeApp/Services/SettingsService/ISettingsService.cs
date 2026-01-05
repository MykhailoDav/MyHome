namespace MyHomeApp.Services;

public interface ISettingsService
{
    AppTheme CurrentTheme { get; set; }
    LanguageOption? CurrentLanguage { get; set; }
    List<LanguageOption> AvailableLanguages { get; }
    void ToggleTheme();
    List<LanguageOption> GetAvailableLanguages();
}
