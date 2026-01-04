using CommunityToolkit.Maui;
using MyHomeApp.Resources.Localization;

namespace MyHomeApp;

/// <summary>
/// Extension methods for organizing application registration
/// </summary>
public static class Registration
{
    /// <summary>
    /// Registers all ViewModels and Views with their routes
    /// </summary>
    /// <param name="builder">The MAUI app builder</param>
    /// <returns>The builder for method chaining</returns>
    public static MauiAppBuilder RegisterViewModelsAndViews(this MauiAppBuilder builder)
    {
        // Register Dashboard (main page)
        builder.Services.AddTransientWithShellRoute<DashboardPage, DashboardViewModel>(Routes.DashboardPage);

        // Add more views here as the app grows
        // Example:
        // builder.Services.AddTransientWithShellRoute<SettingsPage, SettingsViewModel>(
        //     AppConstants.Routes.Settings);

        return builder;
    }

    /// <summary>
    /// Registers all application services
    /// </summary>
    /// <param name="builder">The MAUI app builder</param>
    /// <returns>The builder for method chaining</returns>
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

        // Register Shell
        builder.Services.AddSingleton<AppShell>();

        return builder;
    }

    /// <summary>
    /// Configures fonts for the application
    /// </summary>
    /// <param name="builder">The MAUI app builder</param>
    /// <returns>The builder for method chaining</returns>
    public static MauiAppBuilder ConfigureAppFonts(this MauiAppBuilder builder)
    {
        builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });

        return builder;
    }

    /// <summary>
    /// Configures localization for the application
    /// </summary>
    /// <param name="builder">The MAUI app builder</param>
    /// <returns>The builder for method chaining</returns>
    public static MauiAppBuilder ConfigureLocalization(this MauiAppBuilder builder)
    {
        builder.UseLocalizationResourceManager(settings =>
        {
            settings.AddResource(AppResources.ResourceManager);
            settings.RestoreLatestCulture(true);
        });

        return builder;
    }

    /// <summary>
    /// Configures MAUI handlers customization
    /// </summary>
    /// <param name="builder">The MAUI app builder</param>
    /// <returns>The builder for method chaining</returns>
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
