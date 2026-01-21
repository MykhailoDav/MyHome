using CommunityToolkit.Maui;
using MyHomeApp.Resources.Localization;

namespace MyHomeApp;

public static class Registration
{
    public static MauiAppBuilder RegisterViewModelsAndViews(this MauiAppBuilder builder)
    {
        builder.Services.AddTransientWithShellRoute<DashboardPage, DashboardViewModel>(Routes.DashboardPage);
        return builder;
    }

    public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        // Register MAUI Essentials services as singletons
        builder.Services.AddSingleton(Preferences.Default);
        builder.Services.AddSingleton(Connectivity.Current);
        builder.Services.AddSingleton(Browser.Default);
        builder.Services.AddSingleton(AppInfo.Current);
        builder.Services.AddSingleton(Email.Default);
        builder.Services.AddSingleton(SecureStorage.Default);

        // Register application services
        builder.Services.AddSingleton<IThemeService, ThemeService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IDiagnosticService, DiagnosticService>();
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<IWeatherMqttService, WeatherMqttService>();

        return builder;
    }

    public static MauiAppBuilder ConfigureAppFonts(this MauiAppBuilder builder)
    {
        builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });

        return builder;
    }

    public static MauiAppBuilder ConfigureLocalization(this MauiAppBuilder builder)
    {
        builder.UseLocalizationResourceManager(settings =>
        {
            settings.AddResource(AppResources.ResourceManager);
            settings.RestoreLatestCulture(true);
        });

        return builder;
    }

    public static MauiAppBuilder ConfigureHandlers(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
#if ANDROID
            // Remove default underline from Entry controls on Android
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {
                handler.PlatformView.BackgroundTintList = 
                    Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            });
#endif
        });

        return builder;
    }
}
