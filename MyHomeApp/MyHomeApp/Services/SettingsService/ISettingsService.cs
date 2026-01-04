namespace MyHomeApp.Services;

public interface ISettingsService
{
    AppTheme CurrentTheme { get; set; }
    string CurrentLanguage { get; set; }
    LanguageOption? SelectedLanguage { get; set; }
    List<LanguageOption> AvailableLanguages { get; }
    void ToggleTheme();
    List<LanguageOption> GetAvailableLanguages();
}
