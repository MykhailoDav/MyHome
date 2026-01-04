namespace MyHomeApp;

/// <summary>
/// Application-wide constants for preferences keys, routes, and configuration
/// </summary>
public static class AppConstants
{
    /// <summary>
    /// Preferences keys for persistent storage
    /// </summary>
    public static class PreferencesKeys
    {
        public const string Theme = "app_theme";
        public const string Language = "app_language";
    }
    
    /// <summary>
    /// Supported culture codes
    /// </summary>
    public static class Cultures
    {
        public const string English = "en";
        public const string Ukrainian = "uk";
        
        public const string EnglishFull = "en-US";
        public const string UkrainianFull = "uk-UA";
    }

    /// <summary>
    /// Default values
    /// </summary>
    public static class Defaults
    {
        public const string Language = Cultures.English;
        public static readonly AppTheme Theme = AppTheme.Light;
    }
}
