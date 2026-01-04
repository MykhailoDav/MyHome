using System.Globalization;

namespace MyHomeApp.Converters;

/// <summary>
/// Converts AppTheme enum to bool for Switch control
/// </summary>
public class ThemeToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is AppTheme theme)
        {
            return theme == AppTheme.Dark;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isDark)
        {
            return isDark ? AppTheme.Dark : AppTheme.Light;
        }
        return AppTheme.Light;
    }
}

/// <summary>
/// Converts AppTheme enum to display text
/// </summary>
public class ThemeToTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is AppTheme theme)
        {
            return theme == AppTheme.Dark ? "Dark Mode" : "Light Mode";
        }
        return "Light Mode";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
