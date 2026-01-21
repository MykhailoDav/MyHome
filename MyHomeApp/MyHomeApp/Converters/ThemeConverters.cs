using System.Globalization;
using MyHomeApp.Resources.Localization;

namespace MyHomeApp.Converters;

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
            return theme == AppTheme.Dark ? AppResources.DarkMode : AppResources.LightMode;
        }
        return AppResources.LightMode;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts error state to color - returns red for error, normal color otherwise
/// </summary>
public class ErrorToColorConverter : IMultiValueConverter
{
    public object? Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Length < 2)
            return Colors.Gray;

        bool hasError = values[0] is bool error && error;
        Color normalColor = values[1] as Color ?? Colors.Gray;

        return hasError ? Color.FromArgb("#C62828") : normalColor;
    }

    public object?[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
