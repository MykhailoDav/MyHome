namespace MyHomeApp.ViewModels;


public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty]
    double indoorTemperature;

    [ObservableProperty]
    double indoorHumidity;

    [ObservableProperty]
    double outdoorTemperature;

    [ObservableProperty]
    double outdoorHumidity;

    [ObservableProperty]
    double outdoorPressure;

    [ObservableProperty]
    string indoorTemperatureDisplay = string.Empty;

    [ObservableProperty]
    string indoorHumidityDisplay = string.Empty;

    [ObservableProperty]
    string outdoorTemperatureDisplay = string.Empty;

    [ObservableProperty]
    string outdoorHumidityDisplay = string.Empty;

    [ObservableProperty]
    string outdoorPressureDisplay = string.Empty;

    public DashboardViewModel()
    {
        LoadMockData();
    }

    void LoadMockData()
    {
        IndoorTemperature = 22.5;
        IndoorHumidity = 45.0;

        OutdoorTemperature = 18.3;
        OutdoorHumidity = 62.0;
        OutdoorPressure = 1013.25;

        UpdateDisplayValues();
    }

    partial void OnIndoorTemperatureChanged(double value)
    {
        IndoorTemperatureDisplay = $"{value:F1}°C";
    }

    partial void OnIndoorHumidityChanged(double value)
    {
        IndoorHumidityDisplay = $"{value:F0}%";
    }

    partial void OnOutdoorTemperatureChanged(double value)
    {
        OutdoorTemperatureDisplay = $"{value:F1}°C";
    }

    partial void OnOutdoorHumidityChanged(double value)
    {
        OutdoorHumidityDisplay = $"{value:F0}%";
    }

    partial void OnOutdoorPressureChanged(double value)
    {
        OutdoorPressureDisplay = $"{value:F2} hPa";
    }

    void UpdateDisplayValues()
    {
        IndoorTemperatureDisplay = $"{IndoorTemperature:F1}°C";
        IndoorHumidityDisplay = $"{IndoorHumidity:F0}%";
        OutdoorTemperatureDisplay = $"{OutdoorTemperature:F1}°C";
        OutdoorHumidityDisplay = $"{OutdoorHumidity:F0}%";
        OutdoorPressureDisplay = $"{OutdoorPressure:F2} hPa";
    }
}
