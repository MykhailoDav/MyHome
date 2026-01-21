#include "bme280.h"
#include <Arduino.h>
#include <Wire.h>
#include <string.h>

#define BME280_REG_ID 0xD0
#define BME280_REG_CTRL_HUM 0xF2
#define BME280_REG_CTRL_MEAS 0xF4
#define BME280_REG_CONFIG 0xF5
#define BME280_REG_DATA 0xF7

static bool write_reg(uint8_t addr, uint8_t reg, uint8_t val) {
    Wire.beginTransmission(addr);
    Wire.write(reg);
    Wire.write(val);
    return Wire.endTransmission() == 0;
}

static bool read_regs(uint8_t addr, uint8_t reg, uint8_t *buf, size_t len) {
    Wire.beginTransmission(addr);
    Wire.write(reg);
    if (Wire.endTransmission() != 0) return false;
    Wire.requestFrom(addr, (uint8_t)len);
    for (size_t i = 0; i < len; i++) {
        if (!Wire.available()) return false;
        buf[i] = Wire.read();
    }
    return true;
}

bool bme280_init(bme280_t *dev, uint8_t addr) {
    if (!dev) return false;
    memset(dev, 0, sizeof(*dev));
    dev->addr = addr;
    dev->present = false;

    uint8_t id = 0;
    if (!read_regs(addr, BME280_REG_ID, &id, 1)) {
        Serial.print("BME280: no response at 0x");
        Serial.println(addr, HEX);
        return false;
    }
    if (id != 0x60 && id != 0x58) {
        Serial.print("BME280: wrong ID 0x");
        Serial.println(id, HEX);
        return false;
    }

    uint8_t cal[26];
    if (!read_regs(addr, 0x88, cal, 26)) return false;
    dev->dig_T1 = (cal[1] << 8) | cal[0];
    dev->dig_T2 = (int16_t)((cal[3] << 8) | cal[2]);
    dev->dig_T3 = (int16_t)((cal[5] << 8) | cal[4]);
    dev->dig_P1 = (cal[7] << 8) | cal[6];
    dev->dig_P2 = (int16_t)((cal[9] << 8) | cal[8]);
    dev->dig_P3 = (int16_t)((cal[11] << 8) | cal[10]);
    dev->dig_P4 = (int16_t)((cal[13] << 8) | cal[12]);
    dev->dig_P5 = (int16_t)((cal[15] << 8) | cal[14]);
    dev->dig_P6 = (int16_t)((cal[17] << 8) | cal[16]);
    dev->dig_P7 = (int16_t)((cal[19] << 8) | cal[18]);
    dev->dig_P8 = (int16_t)((cal[21] << 8) | cal[20]);
    dev->dig_P9 = (int16_t)((cal[23] << 8) | cal[22]);
    dev->dig_H1 = cal[25];

    uint8_t hcal[7];
    if (!read_regs(addr, 0xE1, hcal, 7)) return false;
    dev->dig_H2 = (int16_t)((hcal[1] << 8) | hcal[0]);
    dev->dig_H3 = hcal[2];
    dev->dig_H4 = (int16_t)((hcal[3] << 4) | (hcal[4] & 0x0F));
    dev->dig_H5 = (int16_t)((hcal[5] << 4) | (hcal[4] >> 4));
    dev->dig_H6 = (int8_t)hcal[6];

    write_reg(addr, BME280_REG_CTRL_HUM, 0x01);
    write_reg(addr, BME280_REG_CTRL_MEAS, 0x27);
    write_reg(addr, BME280_REG_CONFIG, 0xA0);

    dev->present = true;
    Serial.print("BME280: found at 0x");
    Serial.println(addr, HEX);
    return true;
}

bool bme280_read(bme280_t *dev, float *temperature, float *pressure, float *humidity) {
    if (!dev || !dev->present) return false;

    uint8_t data[8];
    if (!read_regs(dev->addr, BME280_REG_DATA, data, 8)) {
        dev->present = false;
        return false;
    }

    int32_t adc_P = ((uint32_t)data[0] << 12) | ((uint32_t)data[1] << 4) | ((data[2] >> 4) & 0x0F);
    int32_t adc_T = ((uint32_t)data[3] << 12) | ((uint32_t)data[4] << 4) | ((data[5] >> 4) & 0x0F);
    int32_t adc_H = ((uint32_t)data[6] << 8) | data[7];

    int32_t var1 = ((((adc_T >> 3) - ((int32_t)dev->dig_T1 << 1))) * ((int32_t)dev->dig_T2)) >> 11;
    int32_t var2 = (((((adc_T >> 4) - ((int32_t)dev->dig_T1)) * ((adc_T >> 4) - ((int32_t)dev->dig_T1))) >> 12) * ((int32_t)dev->dig_T3)) >> 14;
    dev->t_fine = var1 + var2;
    float T = (dev->t_fine * 5 + 128) >> 8;
    if (temperature) *temperature = T / 100.0f;

    int64_t var1_p = ((int64_t)dev->t_fine) - 128000;
    int64_t var2_p = var1_p * var1_p * (int64_t)dev->dig_P6;
    var2_p = var2_p + ((var1_p * (int64_t)dev->dig_P5) << 17);
    var2_p = var2_p + (((int64_t)dev->dig_P4) << 35);
    var1_p = ((var1_p * var1_p * (int64_t)dev->dig_P3) >> 8) + ((var1_p * (int64_t)dev->dig_P2) << 12);
    var1_p = (((((int64_t)1) << 47) + var1_p)) * ((int64_t)dev->dig_P1) >> 33;
    
    if (var1_p == 0) {
        if (pressure) *pressure = 0;
    } else {
        int64_t p = 1048576 - adc_P;
        p = (((p << 31) - var2_p) * 3125) / var1_p;
        var1_p = (((int64_t)dev->dig_P9) * (p >> 13) * (p >> 13)) >> 25;
        var2_p = (((int64_t)dev->dig_P8) * p) >> 19;
        p = ((p + var1_p + var2_p) >> 8) + (((int64_t)dev->dig_P7) << 4);
        if (pressure) *pressure = (float)p / 256.0f;
    }

    int32_t v_x1 = (dev->t_fine - ((int32_t)76800));
    v_x1 = (((((adc_H << 14) - (((int32_t)dev->dig_H4) << 20) - (((int32_t)dev->dig_H5) * v_x1)) +
                   ((int32_t)16384)) >> 15) * (((((((v_x1 * ((int32_t)dev->dig_H6)) >> 10) *
                   (((v_x1 * ((int32_t)dev->dig_H3)) >> 11) + ((int32_t)32768))) >> 10) + ((int32_t)2097152)) *
                   ((int32_t)dev->dig_H2) + 8192) >> 14));
    v_x1 = (v_x1 - (((((v_x1 >> 15) * (v_x1 >> 15)) >> 7) * ((int32_t)dev->dig_H1)) >> 4));
    v_x1 = (v_x1 < 0) ? 0 : v_x1;
    v_x1 = (v_x1 > 419430400) ? 419430400 : v_x1;
    float h = (v_x1 >> 12);
    if (humidity) *humidity = h / 1024.0f;

    return true;
}
