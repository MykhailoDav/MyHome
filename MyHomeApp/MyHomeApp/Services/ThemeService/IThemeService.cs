namespace MyHomeApp.Services;

public interface IThemeService
{
    void SetTheme(AppTheme appTheme);
    AppTheme UserAppTheme { get; }
}
