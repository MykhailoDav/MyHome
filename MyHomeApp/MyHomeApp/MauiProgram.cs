using epj.RouteGenerator;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace MyHomeApp;

[AutoRoutes("Page")]
public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.RegisterServices()
			.RegisterViewModelsAndViews()
			.ConfigureAppFonts()
			.ConfigureLocalization()
			.ConfigureHandlers();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();
		return app;
	}
}
