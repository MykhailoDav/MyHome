# Налаштування HiveMQ Cloud

## Крок 1: Створення облікового запису HiveMQ Cloud

1. Перейдіть на https://www.hivemq.com/mqtt-cloud-broker/
2. Натисніть "Get Started Free"
3. Створіть обліковий запис

## Крок 2: Створення кластера

1. Увійдіть в консоль HiveMQ Cloud
2. Натисніть "Create Cluster"
3. Виберіть план "Free" (безкоштовний)
4. Оберіть регіон (наприклад, EU - для Європи)
5. Введіть назву кластера (наприклад, "WeatherStation")
6. Натисніть "Create"

## Крок 3: Налаштування Access Management

1. Після створення кластера, перейдіть до вкладки "Access Management"
2. Натисніть "+ Credentials" або "Add Credentials"
3. Введіть:
   - Username: наприклад, `weather_esp32`
   - Password: створіть складний пароль (зберігайте його!)
4. Натисніть "Add"

**ВАЖЛИВО:** Збережіть username та password - вони знадобляться для ESP32 та .NET MAUI додатку!

## Крок 4: Отримання інформації про кластер

У розділі "Overview" вашого кластера ви знайдете:

- **Cluster URL**: наприклад, `abc123def456.s1.eu.hivemq.cloud`
- **Port**: 8883 (для MQTT over TLS)

## Крок 5: Конфігурація ESP32

Відредагуйте файл `include/config.h`:

```c
#define WIFI_SSID "YourWiFiName"
#define WIFI_PASSWORD "YourWiFiPassword"

#define MQTT_BROKER_URL "abc123def456.s1.eu.hivemq.cloud"  // Ваш Cluster URL
#define MQTT_PORT 8883
#define MQTT_USERNAME "weather_esp32"  // Username з Access Management
#define MQTT_PASSWORD "YourSecurePassword"  // Password з Access Management
```

## Крок 6: Тестування з Web Client

1. У консолі HiveMQ Cloud перейдіть до "Web Client"
2. Натисніть "Connect" (використовуються ваші credentials)
3. Підпишіться на топіки:
   - `weather/outdoor`
   - `weather/indoor`
   - `weather/status`
4. Після запуску ESP32 ви побачите повідомлення в Web Client

## Крок 7: Налаштування .NET MAUI додатку

У вашому .NET MAUI проекті:

1. Додайте NuGet пакет: `MQTTnet`
2. Використайте приклад коду з `DotNetMAUI_Example.cs`
3. Замініть константи:
   ```csharp
   private const string BROKER_URL = "abc123def456.s1.eu.hivemq.cloud";
   private const string USERNAME = "weather_esp32";
   private const string PASSWORD = "YourSecurePassword";
   ```

## Обмеження Free плану

- Максимум 100 з'єднань
- До 10 GB трафіку на місяць
- 1 кластер
- Без SLA

Для більшості проектів IoT це цілком достатньо!

## Безпека

✅ HiveMQ Cloud автоматично використовує TLS шифрування
✅ Сертифікати Let's Encrypt (ISRG Root X1) вже включені
✅ Username/Password аутентифікація

## Корисні посилання

- HiveMQ Cloud Console: https://console.hivemq.cloud/
- Документація: https://docs.hivemq.com/hivemq-cloud/
- MQTT Клієнт: http://www.hivemq.com/demos/websocket-client/
