#include "htu21.h"
#include <Arduino.h>
#include <Wire.h>
#include <string.h>

#define HTU21_CMD_TRIG_TEMP_NOHOLD 0xF3
#define HTU21_CMD_TRIG_HUM_NOHOLD  0xF5

static bool write_cmd(uint8_t addr, uint8_t cmd) {
    Wire.beginTransmission(addr);
    Wire.write(cmd);
    return Wire.endTransmission() == 0;
}

static bool read_bytes(uint8_t addr, uint8_t *buf, size_t len) {
    Wire.requestFrom(addr, (uint8_t)len);
    for (size_t i = 0; i < len; i++) {
        if (!Wire.available()) return false;
        buf[i] = Wire.read();
    }
    return true;
}

bool htu21_init(htu21_t *dev, uint8_t addr) {
    if (!dev) return false;
    memset(dev, 0, sizeof(*dev));
    dev->addr = addr;
    
    uint8_t user_reg;
    if (!write_cmd(addr, 0xE7)) {
        Serial.print("HTU21: not present at 0x");
        Serial.println(addr, HEX);
        dev->present = false;
        return false;
    }
    delay(10);
    if (!read_bytes(addr, &user_reg, 1)) {
        Serial.print("HTU21: not present at 0x");
        Serial.println(addr, HEX);
        dev->present = false;
        return false;
    }
    
    dev->present = true;
    Serial.print("HTU21: present at 0x");
    Serial.print(addr, HEX);
    Serial.print(" (user_reg=0x");
    Serial.print(user_reg, HEX);
    Serial.println(")");
    return true;
}

bool htu21_read(htu21_t *dev, float *temperature, float *humidity) {
    if (!dev || !dev->present) return false;
    uint8_t buf[3];

    if (!write_cmd(dev->addr, HTU21_CMD_TRIG_TEMP_NOHOLD)) {
        dev->present = false;
        return false;
    }
    delay(60);
    
    if (!read_bytes(dev->addr, buf, 3)) {
        dev->present = false;
        return false;
    }
    uint16_t rawT = (buf[0] << 8) | buf[1];
    rawT &= ~0x0003;
    float t = -46.85f + 175.72f * ((float)rawT / 65536.0f);

    if (!write_cmd(dev->addr, HTU21_CMD_TRIG_HUM_NOHOLD)) {
        dev->present = false;
        return false;
    }
    delay(20);
    
    if (!read_bytes(dev->addr, buf, 3)) {
        dev->present = false;
        return false;
    }
    uint16_t rawH = (buf[0] << 8) | buf[1];
    rawH &= ~0x0003;
    float h = -6.0f + 125.0f * ((float)rawH / 65536.0f);

    if (temperature) *temperature = t;
    if (humidity) *humidity = h;
    return true;
}
