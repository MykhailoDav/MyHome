# Швидкий старт 🚀

## Крок 1: Налаштування HiveMQ Cloud ☁️

1. Зареєструйтесь на https://www.hivemq.com/mqtt-cloud-broker/
2. Створіть безкоштовний кластер
3. Створіть credentials (username/password) в розділі "Access Management"
4. Збережіть:
   - Cluster URL (наприклад: `abc123.s1.eu.hivemq.cloud`)
   - Username
   - Password

📖 Детальні інструкції в файлі `HIVEMQ_SETUP.md`

## Крок 2: Налаштування ESP32 📡

### 2.1 Відредагуйте `include/config.h`

```c
#define WIFI_SSID "YourWiFiName"          // ✏️ Змініть на вашу WiFi мережу
#define WIFI_PASSWORD "YourWiFiPassword"  // ✏️ Змініть на ваш WiFi пароль

#define MQTT_BROKER_URL "abc123.s1.eu.hivemq.cloud"  // ✏️ Ваш HiveMQ Cluster URL
#define MQTT_USERNAME "weather_esp32"     // ✏️ Ваш HiveMQ username
#define MQTT_PASSWORD "YourPassword"      // ✏️ Ваш HiveMQ password
```

### 2.2 Збірка та завантаження

```bash
# Збірка
pio run

# Завантаження на ESP32
pio run --target upload

# Моніторинг
pio device monitor
```

### 2.3 Очікуваний вивід

```
WeatherStation starting...
Scanning I2C bus...
  Found device at 0x40
  Found device at 0x76
Scan complete. Found 2 device(s)
Initializing WiFi...
Connected to AP SSID:YourWiFiName
Got IP:192.168.1.100
Initializing MQTT...
MQTT_EVENT_CONNECTED
Published outdoor data, msg_id=1: {"temperature":22.50,...}
Published indoor data, msg_id=2: {"temperature":23.10,...}
```

## Крок 3: Тестування в HiveMQ Web Client 🌐

1. Увійдіть в HiveMQ Cloud Console
2. Перейдіть до "Web Client"
3. Натисніть "Connect"
4. Додайте підписки:
   ```
   weather/outdoor
   weather/indoor
   weather/status
   ```
5. Ви маєте побачити JSON повідомлення!

## Крок 4: Налаштування .NET MAUI додатку 📱

### 4.1 Створіть новий .NET MAUI проект

```bash
dotnet new maui -n WeatherApp
cd WeatherApp
```

### 4.2 Додайте NuGet пакет

```bash
dotnet add package MQTTnet
```

### 4.3 Скопіюйте файли

- `DotNetMAUI_Example.cs` → `Services/WeatherMqttService.cs`
- `DotNetMAUI_ViewModel.cs` → `ViewModels/WeatherViewModel.cs`
- `DotNetMAUI_MainPage.xaml` → `MainPage.xaml`

### 4.4 Відредагуйте credentials

У файлі `Services/WeatherMqttService.cs`:

```csharp
private const string BROKER_URL = "abc123.s1.eu.hivemq.cloud";  // ✏️
private const string USERNAME = "weather_esp32";                 // ✏️
private const string PASSWORD = "YourPassword";                  // ✏️
```

### 4.5 Запустіть додаток

```bash
dotnet build
dotnet run
```

## Структура MQTT повідомлень 📨

### Outdoor (BME280)
```json
{
  "temperature": 22.50,
  "pressure": 101325.0,
  "humidity": 45.20,
  "timestamp": 1704567890
}
```

### Indoor (HTU21)
```json
{
  "temperature": 23.10,
  "humidity": 48.50,
  "timestamp": 1704567890
}
```

### Status
```json
{
  "status": "online"
}
```

## Топіки MQTT 📬

| Топік | Опис | QoS |
|-------|------|-----|
| `weather/outdoor` | Дані BME280 (температура, тиск, вологість) | 1 |
| `weather/indoor` | Дані HTU21 (температура, вологість) | 1 |
| `weather/status` | Статус пристрою (online/offline) | 1 |

## Діагностика проблем 🔧

### ESP32 не підключається до WiFi
```
❌ Symptom: "Connect to the AP failed"
✅ Fix: Перевірте WIFI_SSID і WIFI_PASSWORD в config.h
```

### ESP32 не підключається до MQTT
```
❌ Symptom: "MQTT_EVENT_ERROR"
✅ Fix: 
  - Перевірте MQTT_BROKER_URL (без https://)
  - Перевірте MQTT_USERNAME і MQTT_PASSWORD
  - Перевірте що кластер активний
```

### Датчики не знайдені
```
❌ Symptom: "Found 0 device(s)"
✅ Fix: Перевірте I2C підключення (SDA=41, SCL=42)
```

### .NET MAUI не отримує дані
```
❌ Symptom: Показує "--" для всіх значень
✅ Fix:
  - Перевірте що ESP32 підключений
  - Перевірте credentials в MqttService
  - Перевірте інтернет з'єднання
```

## Наступні кроки 🎯

- [ ] Додайте історію даних (база даних SQLite)
- [ ] Додайте графіки температури
- [ ] Додайте push notifications
- [ ] Додайте більше датчиків
- [ ] Створіть веб-дашборд

## Підтримка 💬

- HiveMQ Docs: https://docs.hivemq.com/
- ESP-IDF Docs: https://docs.espressif.com/
- .NET MAUI Docs: https://learn.microsoft.com/dotnet/maui/

Успіхів! 🎉
