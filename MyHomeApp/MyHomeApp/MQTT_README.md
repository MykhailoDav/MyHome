# MQTT Weather Station Integration

## Overview

This .NET MAUI application connects to HiveMQ Cloud MQTT broker to receive real-time sensor data from ESP32 weather stations.

## Features

- **Real-time data updates** via MQTT
- **Indoor sensor** (HTU21): Temperature & Humidity
- **Outdoor sensor** (BME280): Temperature, Humidity & Pressure
- **Auto-reconnect** functionality
- **Connection status** monitoring
- **Manual refresh** and reconnect buttons

## Configuration

MQTT connection settings are located in `MyHomeApp/Constants/MqttConfig.cs`:

```csharp
public static class MqttConfig
{
    public const string BrokerUrl = "0a2cbb2a55b94edca77b664f403a756a.s1.eu.hivemq.cloud";
    public const int BrokerPort = 8883;
    public const string Username = "MyHomeApp";
    public const string Password = "MyHome123";
    public const string ClientId = "MyHomeApp_MAUI";
    
    public static class Topics
    {
        public const string Outdoor = "weather/outdoor";
        public const string Indoor = "weather/indoor";
        public const string Status = "weather/status";
        public const string Command = "weather/command";
    }
}
```

## MQTT Topics

### Subscribed Topics

1. **weather/outdoor** - BME280 sensor data
   ```json
   {
     "temperature": 18.3,
     "pressure": 101325.0,
     "humidity": 62.0,
     "timestamp": 1234567890
   }
   ```

2. **weather/indoor** - HTU21 sensor data
   ```json
   {
     "temperature": 22.5,
     "humidity": 45.0,
     "timestamp": 1234567890
   }
   ```

3. **weather/status** - Device status messages
   ```json
   {
     "status": "online"
   }
   ```

### Publish Topics

- **weather/command** - Send commands to ESP32 device

## Architecture

### Services

- **IWeatherMqttService** - Interface for MQTT service
- **WeatherMqttService** - Implementation using MQTTnet library
  - Handles connection/disconnection
  - Subscribes to topics
  - Parses JSON sensor data
  - Raises events for UI updates

### ViewModels

- **DashboardViewModel** - Main dashboard logic
  - Listens to MQTT events
  - Updates UI on main thread
  - Provides commands for reconnect/refresh

### Models

- **SensorData** - Data model for sensor readings
  ```csharp
  public class SensorData
  {
      public float Temperature { get; set; }
      public float? Pressure { get; set; }  // Nullable for indoor sensor
      public float Humidity { get; set; }
      public long Timestamp { get; set; }
      public DateTime DateTime => DateTimeOffset.FromUnixTimeSeconds(Timestamp).LocalDateTime;
  }
  ```

## Dependencies

- **MQTTnet** (v4.3.7.1207) - MQTT client library
- **CommunityToolkit.Mvvm** (v8.4.0) - MVVM helpers
- **CommunityToolkit.Maui** (v13.0.0) - UI components

## Usage

### Starting the App

1. The app automatically connects to MQTT broker on startup
2. Connection status is shown at the top of the dashboard
3. Sensor data updates in real-time when published by ESP32

### Manual Controls

- **Reconnect** - Manually reconnect to MQTT broker
- **Refresh** - Update connection status display

## ESP32 Integration

### Required Hardware

- ESP32 board (e.g., ESP32-S3)
- BME280 sensor (outdoor)
- HTU21 sensor (indoor)

### ESP32 Configuration

See `../Weather/ThermometersESP32/src/config.h`:

```c
#define WIFI_SSID "YourWiFiSSID"
#define WIFI_PASSWORD "YourPassword"

#define MQTT_BROKER_URL "0a2cbb2a55b94edca77b664f403a756a.s1.eu.hivemq.cloud"
#define MQTT_PORT 8883
#define MQTT_USERNAME "MyHomeApp"
#define MQTT_PASSWORD "MyHome123"
#define MQTT_CLIENT_ID "ESP32_WeatherStation"

#define MQTT_TOPIC_OUTDOOR "weather/outdoor"
#define MQTT_TOPIC_INDOOR "weather/indoor"
#define MQTT_TOPIC_STATUS "weather/status"

#define PUBLISH_INTERVAL_MS 10000  // 10 seconds
```

## Testing

### Using MQTT Explorer

1. Download [MQTT Explorer](http://mqtt-explorer.com/)
2. Connect to HiveMQ Cloud:
   - Protocol: mqtts://
   - Host: 0a2cbb2a55b94edca77b664f403a756a.s1.eu.hivemq.cloud
   - Port: 8883
   - Username: MyHomeApp
   - Password: MyHome123

3. Publish test message to `weather/outdoor`:
   ```json
   {
     "temperature": 25.5,
     "pressure": 101325,
     "humidity": 50.0,
     "timestamp": 1234567890
   }
   ```

## Troubleshooting

### Connection Issues

- Check internet connectivity
- Verify MQTT credentials in `MqttConfig.cs`
- Check HiveMQ Cloud console for connection logs
- Review debug output using `Trace.WriteLine`

### No Data Received

- Verify ESP32 is publishing to correct topics
- Check JSON format matches `SensorData` model
- Use MQTT Explorer to monitor topic messages

### App Crashes

- Check MQTTnet package version compatibility
- Verify .NET MAUI workload is installed
- Review build output for errors

## Extending Functionality

### Add Historical Data

See `README.md` examples for SQLite integration

### Add Charts

See `README.md` examples for LiveCharts2 integration

### Add Notifications

Implement temperature alerts using local notifications

### Export Data

Add CSV export functionality for sensor data

## Security Notes

?? **Important**: The current configuration stores MQTT credentials in code. For production:

1. Move credentials to secure storage (e.g., Azure Key Vault)
2. Use `SecureStorage` API for sensitive data
3. Implement certificate pinning for TLS
4. Use device-specific client IDs

## License

This project is part of MyHome application suite.
