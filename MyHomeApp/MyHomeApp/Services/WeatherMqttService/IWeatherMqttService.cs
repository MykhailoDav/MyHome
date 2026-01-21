namespace MyHomeApp.Services;

public interface IWeatherMqttService
{
    event EventHandler<SensorData>? OutdoorDataReceived;
    event EventHandler<SensorData>? IndoorDataReceived;
    event EventHandler<string>? StatusChanged;
    
    bool IsConnected { get; }
    
    Task ConnectAsync(CancellationToken token = default);
    Task DisconnectAsync(CancellationToken token = default);
    Task PublishCommandAsync(string command, CancellationToken token = default);
}
