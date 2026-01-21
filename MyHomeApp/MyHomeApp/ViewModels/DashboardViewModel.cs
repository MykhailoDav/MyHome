namespace MyHomeApp.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    readonly IWeatherMqttService weatherMqttService;
    readonly ILocalizationResourceManager localizationService;

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

    [ObservableProperty]
    string connectionStatus = string.Empty;

    [ObservableProperty]
    bool isConnected;

    [ObservableProperty]
    string indoorLastUpdate = string.Empty;

    [ObservableProperty]
    string outdoorLastUpdate = string.Empty;

    [ObservableProperty]
    bool indoorHasError;

    [ObservableProperty]
    bool outdoorHasError;

    [ObservableProperty]
    string? indoorErrorMessage;

    [ObservableProperty]
    string? outdoorErrorMessage;

    public DashboardViewModel(IWeatherMqttService weatherMqttService, ILocalizationResourceManager localizationService)
    {
        this.weatherMqttService = weatherMqttService;
        this.localizationService = localizationService;

        weatherMqttService.OutdoorDataReceived += OnOutdoorDataReceived;
        weatherMqttService.IndoorDataReceived += OnIndoorDataReceived;
        weatherMqttService.StatusChanged += OnStatusChanged;

        ConnectionStatus = localizationService.GetValue("Disconnected");
        IndoorLastUpdate = localizationService.GetValue("Never");
        OutdoorLastUpdate = localizationService.GetValue("Never");

        _ = InitializeAsync();
    }

    async Task InitializeAsync(CancellationToken token = default)
    {
        try
        {
            ConnectionStatus = localizationService.GetValue("Connecting");
            await weatherMqttService.ConnectAsync(token);
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Initialize MQTT Error: {ex.Message}");
            ConnectionStatus = $"{localizationService.GetValue("Error")}: {ex.Message}";
        }
    }

    void OnOutdoorDataReceived(object? sender, SensorData data)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (data.IsError)
            {
                OutdoorHasError = true;
                OutdoorErrorMessage = data.Message ?? localizationService.GetValue("SensorNotAvailable");
                OutdoorTemperatureDisplay = localizationService.GetValue("Error");
                OutdoorHumidityDisplay = localizationService.GetValue("Error");
                OutdoorPressureDisplay = localizationService.GetValue("Error");
            }
            else
            {
                OutdoorHasError = false;
                OutdoorErrorMessage = null;
                
                if (data.Temperature.HasValue)
                {
                    OutdoorTemperature = data.Temperature.Value;
                }
                
                if (data.Humidity.HasValue)
                {
                    OutdoorHumidity = data.Humidity.Value;
                }
                
                if (data.Pressure.HasValue)
                {
                    OutdoorPressure = data.Pressure.Value;
                }
            }
            
            OutdoorLastUpdate = $"{localizationService.GetValue("Updated")}: {DateTime.Now:HH:mm:ss}";
        });
    }

    void OnIndoorDataReceived(object? sender, SensorData data)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (data.IsError)
            {
                IndoorHasError = true;
                IndoorErrorMessage = data.Message ?? localizationService.GetValue("SensorNotAvailable");
                IndoorTemperatureDisplay = localizationService.GetValue("Error");
                IndoorHumidityDisplay = localizationService.GetValue("Error");
            }
            else
            {
                IndoorHasError = false;
                IndoorErrorMessage = null;
                
                if (data.Temperature.HasValue)
                {
                    IndoorTemperature = data.Temperature.Value;
                }
                
                if (data.Humidity.HasValue)
                {
                    IndoorHumidity = data.Humidity.Value;
                }
            }
            
            IndoorLastUpdate = $"{localizationService.GetValue("Updated")}: {DateTime.Now:HH:mm:ss}";
        });
    }

    void OnStatusChanged(object? sender, string status)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ConnectionStatus = weatherMqttService.IsConnected 
                ? localizationService.GetValue("Connected") 
                : localizationService.GetValue("Disconnected");
            IsConnected = weatherMqttService.IsConnected;
        });
    }

    partial void OnIndoorTemperatureChanged(double value)
    {
        if (!IndoorHasError)
        {
            IndoorTemperatureDisplay = $"{value:F1}°C";
        }
    }

    partial void OnIndoorHumidityChanged(double value)
    {
        if (!IndoorHasError)
        {
            IndoorHumidityDisplay = $"{value:F0}%";
        }
    }

    partial void OnOutdoorTemperatureChanged(double value)
    {
        if (!OutdoorHasError)
        {
            OutdoorTemperatureDisplay = $"{value:F1}°C";
        }
    }

    partial void OnOutdoorHumidityChanged(double value)
    {
        if (!OutdoorHasError)
        {
            OutdoorHumidityDisplay = $"{value:F0}%";
        }
    }

    partial void OnOutdoorPressureChanged(double value)
    {
        if (!OutdoorHasError)
        {
            OutdoorPressureDisplay = $"{value:F2} hPa";
        }
    }

    [RelayCommand]
    async Task ReconnectAsync(CancellationToken token = default)
    {
        try
        {
            ConnectionStatus = localizationService.GetValue("Reconnecting");
            await weatherMqttService.DisconnectAsync(token);
            await Task.Delay(1000, token);
            await weatherMqttService.ConnectAsync(token);
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Reconnect MQTT Error: {ex.Message}");
            ConnectionStatus = $"{localizationService.GetValue("Error")}: {ex.Message}";
        }
    }

    [RelayCommand]
    Task RefreshAsync(CancellationToken token = default)
    {
        ConnectionStatus = weatherMqttService.IsConnected 
            ? localizationService.GetValue("Connected") 
            : localizationService.GetValue("Disconnected");
        return Task.CompletedTask;
    }
}
