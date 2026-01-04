namespace MyHomeApp.Services;

/// <summary>
/// Implementation of theme management service
/// </summary>
public sealed class ThemeService : IThemeService
{
    public void SetTheme(AppTheme appTheme)
    {
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = appTheme;
        }
    }

    public AppTheme UserAppTheme => Application.Current?.UserAppTheme ?? AppTheme.Unspecified;

    public AppTheme RequestedTheme => Application.Current?.RequestedTheme ?? AppTheme.Unspecified;

    public void ToggleTheme()
    {
        var currentTheme = UserAppTheme;
        var newTheme = currentTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;
        SetTheme(newTheme);
    }
}
