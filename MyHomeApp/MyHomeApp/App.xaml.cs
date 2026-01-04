namespace MyHomeApp;

public partial class App : Application
{
	public static new App Current => (App)Application.Current!;

	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState) => new(new AppShell());

}
