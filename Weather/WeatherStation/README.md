WeatherStation — minimal sensor demo

What I added
- BME280 (GY-BME280/BMP280) driver in lib/bme280 — detects device, reads temperature, pressure, humidity, compensates using calibration.
- HTU21 driver in lib/htu21 — reads temperature and humidity.
- Minimal `src/main.c` — initializes I2C, polls both sensors every 2s, prints values to serial. If a sensor isn't present it prints `--` and attempts reconnection periodically.

Wiring assumptions
- SDA -> GPIO21, SCL -> GPIO22 (adjust `I2C_SDA_PIN`/`I2C_SCL_PIN` in `src/main.c` if your board uses different pins)
- 3.3V power for sensors (GY-BME280 modules often are 5V tolerant but ESP32's I2C pins expect 3.3V)

Build & run (ESP-IDF)
1. Open an ESP-IDF-enabled terminal (run the `export.sh`/`export.ps1` that sets up IDF tools).
2. From project root:

```bash
cd /Users/yurkin/Documents/MyHome/Weather/WeatherStation
idf.py build
idf.py -p /dev/ttyUSB0 flash monitor
```

Notes
- The editor may show include-path warnings for ESP-IDF headers; that's normal unless you open the project inside an ESP-IDF-configured workspace.
- If a sensor is unplugged or fails I2C reads, the code marks it absent and will keep retrying to initialize it (dynamic reconnect).

Next steps (optional)
- Add a config header to set pins/addresses
- Add unit tests or a mock I2C layer for host testing
- Expose readings over MQTT or web API
