namespace MyHomeApp;

public partial class AppShell : Shell
{
    private readonly ISettingsService settingsService;

    public AppShell(ISettingsService settingsService)
    {
        InitializeComponent();
        this.settingsService = settingsService;

        SettingsPanel.BindingContext = this.settingsService;

        InitializeThemeRadioButtons();
    }

    private void InitializeThemeRadioButtons()
    {
        if (settingsService.CurrentTheme == AppTheme.Dark)
        {
            DarkThemeRadio.IsChecked = true;
        }
        else
        {
            LightThemeRadio.IsChecked = true;
        }
    }

    private void OnThemeChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value) return;

        if (sender is not RadioButton radioButton) return;

        AppTheme newTheme = radioButton.Value?.ToString() == "Dark" ? AppTheme.Dark : AppTheme.Light;

        if (settingsService.CurrentTheme != newTheme)
        {
            settingsService.CurrentTheme = newTheme;
        }
    }
}