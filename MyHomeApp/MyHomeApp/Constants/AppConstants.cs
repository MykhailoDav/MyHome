namespace MyHomeApp;

public static class AppConstants
{
    public static class PreferencesKeys
    {
        public const string Theme = "app_theme";
        public const string Language = "app_language";
    }
    
    public static class Cultures
    {
        public const string English = "en";
        public const string Ukrainian = "uk";
        
        public const string EnglishFull = "en-US";
        public const string UkrainianFull = "uk-UA";
    }

    public static class Defaults
    {
        public const string Language = Cultures.English;
        public static readonly AppTheme Theme = AppTheme.Dark;
    }
}
