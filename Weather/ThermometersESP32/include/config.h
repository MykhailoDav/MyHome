#ifndef CONFIG_H
#define CONFIG_H

// WiFi credentials
#define WIFI_SSID "Your_WiFi_SSID"
#define WIFI_PASSWORD "best_password"

// HiveMQ Cloud credentials
#define MQTT_BROKER_URL "0a2cbb2a55b94edca77b664f403a756a.s1.eu.hivemq.cloud"
#define MQTT_PORT 8883
#define MQTT_USERNAME "admin"
#define MQTT_PASSWORD "Your_MQTT_Password"

// MQTT Topics
#define MQTT_TOPIC_OUTDOOR "weather/outdoor"
#define MQTT_TOPIC_INDOOR "weather/indoor"
#define MQTT_TOPIC_STATUS "weather/status"

// Client ID (make it unique for each device)
#define MQTT_CLIENT_ID "ESP32-WeatherStation-001"

// Publish interval (milliseconds)
#define PUBLISH_INTERVAL_MS 5000

#endif // CONFIG_H
