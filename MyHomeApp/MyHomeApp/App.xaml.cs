namespace MyHomeApp;

public partial class App : Application
{
    readonly ISettingsService settingsService;
    public static new App Current => (App)Application.Current!;

    public App(ISettingsService settingsService)
    {
        this.settingsService = settingsService;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState) => new(new AppShell(settingsService));

    protected override void OnStart()
    {

    }

    protected override void OnResume()
    {

    }
}
