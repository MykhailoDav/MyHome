# ESP32 Weather Station with HiveMQ Cloud

Проект метеостанції на ESP32, який відправляє дані з датчиків BME280 (температура, тиск, вологість) та HTU21 (температура, вологість) на HiveMQ Cloud через MQTT.

## Налаштування

### 1. Конфігурація WiFi та MQTT

Відредагуйте файл `include/config.h` та вкажіть ваші дані:

```c
// WiFi credentials
#define WIFI_SSID "YOUR_WIFI_SSID"           // Ваша WiFi мережа
#define WIFI_PASSWORD "YOUR_WIFI_PASSWORD"   // Пароль WiFi

// HiveMQ Cloud credentials
#define MQTT_BROKER_URL "YOUR_CLUSTER_URL"   // URL вашого HiveMQ Cloud кластера
#define MQTT_PORT 8883
#define MQTT_USERNAME "YOUR_USERNAME"         // Ваш username з HiveMQ Cloud
#define MQTT_PASSWORD "YOUR_PASSWORD"         // Ваш password з HiveMQ Cloud
```

### 2. MQTT Topics

За замовчуванням використовуються наступні топіки:
- `weather/outdoor` - дані з BME280 (outdoor sensor)
- `weather/indoor` - дані з HTU21 (indoor sensor)
- `weather/status` - статус пристрою (online/offline)

### 3. Формат JSON повідомлень

**Outdoor (BME280):**
```json
{
  "temperature": 22.50,
  "pressure": 101325.0,
  "humidity": 45.20,
  "timestamp": 1234567890
}
```

**Indoor (HTU21):**
```json
{
  "temperature": 23.10,
  "humidity": 48.50,
  "timestamp": 1234567890
}
```

## Збірка та завантаження

### PlatformIO

```bash
# Збірка проекту
pio run

# Завантаження на ESP32
pio run --target upload

# Монітор Serial
pio device monitor
```

## Підключення до .NET MAUI

У вашому .NET MAUI додатку ви можете підписатися на MQTT топіки для отримання даних:

```csharp
// Приклад підключення до HiveMQ Cloud
var mqttFactory = new MqttFactory();
var mqttClient = mqttFactory.CreateMqttClient();

var mqttClientOptions = new MqttClientOptionsBuilder()
    .WithTcpServer("YOUR_CLUSTER_URL", 8883)
    .WithCredentials("YOUR_USERNAME", "YOUR_PASSWORD")
    .WithTls()
    .Build();

await mqttClient.ConnectAsync(mqttClientOptions);

// Підписка на топіки
await mqttClient.SubscribeAsync("weather/outdoor");
await mqttClient.SubscribeAsync("weather/indoor");

// Обробка повідомлень
mqttClient.ApplicationMessageReceivedAsync += e =>
{
    var topic = e.ApplicationMessage.Topic;
    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
    
    // Парсинг JSON
    var data = JsonSerializer.Deserialize<SensorData>(payload);
    
    return Task.CompletedTask;
};
```

## Апаратна конфігурація

### I2C Підключення

- **SDA Pin**: GPIO 41
- **SCL Pin**: GPIO 42
- **Frequency**: 100 kHz

### Адреси датчиків

- **BME280 (Outdoor)**: 0x76
- **HTU21 (Indoor)**: 0x40

## Безпека

Проект використовує:
- TLS/SSL шифрування для MQTT (порт 8883)
- ISRG Root X1 сертифікат (Let's Encrypt) для верифікації HiveMQ Cloud
- Username/Password аутентифікація

## Troubleshooting

### WiFi не підключається
- Перевірте SSID та пароль в `config.h`
- Переконайтеся що ESP32 в зоні дії WiFi

### MQTT не підключається
- Перевірте URL кластера HiveMQ Cloud
- Перевірте username та password
- Переконайтеся що порт 8883 не заблокований

### Датчики не знайдені
- Перевірте I2C з'єднання
- Запустіть I2C scan (виводиться при старті)
- Перевірте адреси датчиків

## Ліцензія

MIT License
