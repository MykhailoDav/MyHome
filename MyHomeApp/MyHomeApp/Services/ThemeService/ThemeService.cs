namespace MyHomeApp.Services;

public sealed class ThemeService : IThemeService
{
    public void SetTheme(AppTheme appTheme)
    {
        Application.Current?.UserAppTheme = appTheme;
    }

    public AppTheme UserAppTheme => Application.Current?.UserAppTheme ?? AppTheme.Unspecified;
}
