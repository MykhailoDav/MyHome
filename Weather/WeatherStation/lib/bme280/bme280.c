#include "bme280.h"
#include <string.h>
#include <stdio.h>
#include "esp_log.h"

static const char *TAG = "bme280";

// BME280 registers
#define BME280_REG_ID 0xD0
#define BME280_REG_RESET 0xE0
#define BME280_REG_CTRL_HUM 0xF2
#define BME280_REG_STATUS 0xF3
#define BME280_REG_CTRL_MEAS 0xF4
#define BME280_REG_CONFIG 0xF5
#define BME280_REG_PRESS_MSB 0xF7

static esp_err_t write_register(i2c_port_t port, uint8_t addr, uint8_t reg, uint8_t val) {
    i2c_cmd_handle_t cmd = i2c_cmd_link_create();
    i2c_master_start(cmd);
    i2c_master_write_byte(cmd, (addr << 1) | I2C_MASTER_WRITE, true);
    i2c_master_write_byte(cmd, reg, true);
    i2c_master_write_byte(cmd, val, true);
    i2c_master_stop(cmd);
    esp_err_t ret = i2c_master_cmd_begin(port, cmd, pdMS_TO_TICKS(1000));
    i2c_cmd_link_delete(cmd);
    return ret;
}

static esp_err_t read_registers(i2c_port_t port, uint8_t addr, uint8_t reg, uint8_t *buf, size_t len) {
    i2c_cmd_handle_t cmd = i2c_cmd_link_create();
    i2c_master_start(cmd);
    i2c_master_write_byte(cmd, (addr << 1) | I2C_MASTER_WRITE, true);
    i2c_master_write_byte(cmd, reg, true);
    i2c_master_start(cmd);
    i2c_master_write_byte(cmd, (addr << 1) | I2C_MASTER_READ, true);
    if (len > 1) i2c_master_read(cmd, buf, len - 1, I2C_MASTER_ACK);
    i2c_master_read_byte(cmd, buf + len - 1, I2C_MASTER_NACK);
    i2c_master_stop(cmd);
    esp_err_t ret = i2c_master_cmd_begin(port, cmd, pdMS_TO_TICKS(1000));
    i2c_cmd_link_delete(cmd);
    return ret;
}

static uint8_t read_reg8(bme280_t *dev, uint8_t reg, bool *ok) {
    uint8_t v = 0;
    esp_err_t r = read_registers(dev->i2c_port, dev->addr, reg, &v, 1);
    if (r != ESP_OK) {
        if (ok) *ok = false;
        return 0;
    }
    if (ok) *ok = true;
    return v;
}

bool bme280_init(bme280_t *dev, i2c_port_t i2c_port, uint8_t addr) {
    if (!dev) return false;
    memset(dev, 0, sizeof(*dev));
    dev->i2c_port = i2c_port;
    dev->addr = addr;

    bool ok = false;
    uint8_t id = read_reg8(dev, BME280_REG_ID, &ok);
    if (!ok) {
        ESP_LOGW(TAG, "no response at 0x%02x", addr);
        dev->present = false;
        return false;
    }
    if (id != 0x60 && id != 0x58) { // 0x60 BME280, 0x58 BMP280
        ESP_LOGW(TAG, "unexpected id 0x%02x at 0x%02x", id, addr);
        dev->present = false;
        return false;
    }
    dev->present = true;

    // read calibration data
    uint8_t calib[26];
    if (read_registers(i2c_port, addr, 0x88, calib, 26) != ESP_OK) {
        ESP_LOGW(TAG, "failed read calib");
        dev->present = false;
        return false;
    }
    dev->dig_T1 = (uint16_t)(calib[1] << 8 | calib[0]);
    dev->dig_T2 = (int16_t)(calib[3] << 8 | calib[2]);
    dev->dig_T3 = (int16_t)(calib[5] << 8 | calib[4]);

    dev->dig_P1 = (uint16_t)(calib[7] << 8 | calib[6]);
    dev->dig_P2 = (int16_t)(calib[9] << 8 | calib[8]);
    dev->dig_P3 = (int16_t)(calib[11] << 8 | calib[10]);
    dev->dig_P4 = (int16_t)(calib[13] << 8 | calib[12]);
    dev->dig_P5 = (int16_t)(calib[15] << 8 | calib[14]);
    dev->dig_P6 = (int16_t)(calib[17] << 8 | calib[16]);
    dev->dig_P7 = (int16_t)(calib[19] << 8 | calib[18]);
    dev->dig_P8 = (int16_t)(calib[21] << 8 | calib[20]);
    dev->dig_P9 = (int16_t)(calib[23] << 8 | calib[22]);
    dev->dig_H1 = calib[25];

    // read humidity calib (BME280 only)
    uint8_t hcal[7];
    if (read_registers(i2c_port, addr, 0xE1, hcal, 7) == ESP_OK) {
        dev->dig_H2 = (int16_t)(hcal[1] << 8 | hcal[0]);
        dev->dig_H3 = hcal[2];
        dev->dig_H4 = (int16_t)(hcal[3] << 4 | (hcal[4] & 0x0F));
        dev->dig_H5 = (int16_t)((hcal[4] >> 4) | (hcal[5] << 4));
        dev->dig_H6 = (int8_t)hcal[6];
    }

    // set oversampling and normal mode
    write_register(i2c_port, addr, BME280_REG_CTRL_HUM, 0x01); // osrs_h = 1
    write_register(i2c_port, addr, BME280_REG_CTRL_MEAS, 0x27); // osrs_t=1 osrs_p=1 mode=normal
    write_register(i2c_port, addr, BME280_REG_CONFIG, 0xA0); // t_sb=1000ms, filter off

    ESP_LOGI(TAG, "BME280/BMP280 found at 0x%02x", addr);
    return true;
}

static int32_t compensate_T(bme280_t *dev, int32_t adc_T) {
    int32_t var1, var2, T;
    var1 = ((((adc_T>>3) - ((int32_t)dev->dig_T1<<1))) * ((int32_t)dev->dig_T2)) >> 11;
    var2 = (((((adc_T>>4) - ((int32_t)dev->dig_T1)) * ((adc_T>>4) - ((int32_t)dev->dig_T1))) >> 12) * ((int32_t)dev->dig_T3)) >> 14;
    dev->t_fine = var1 + var2;
    T = (dev->t_fine * 5 + 128) >> 8;
    return T;
}

static uint32_t compensate_P(bme280_t *dev, int32_t adc_P) {
    int64_t var1, var2, p;
    var1 = ((int64_t)dev->t_fine) - 128000;
    var2 = var1 * var1 * (int64_t)dev->dig_P6;
    var2 = var2 + ((var1*(int64_t)dev->dig_P5)<<17);
    var2 = var2 + (((int64_t)dev->dig_P4)<<35);
    var1 = ((var1 * var1 * (int64_t)dev->dig_P3)>>8) + ((var1 * (int64_t)dev->dig_P2)<<12);
    var1 = (((((int64_t)1)<<47)+var1))*((int64_t)dev->dig_P1)>>33;
    if (var1 == 0) return 0;
    p = 1048576 - adc_P;
    p = (((p<<31) - var2)*3125) / var1;
    var1 = (((int64_t)dev->dig_P9) * (p>>13) * (p>>13)) >> 25;
    var2 = (((int64_t)dev->dig_P8) * p) >> 19;
    p = ((p + var1 + var2) >> 8) + (((int64_t)dev->dig_P7)<<4);
    return (uint32_t)p;
}

static uint32_t compensate_H(bme280_t *dev, int32_t adc_H) {
    int32_t v_x1_u32r;
    v_x1_u32r = (dev->t_fine - ((int32_t)76800));
    v_x1_u32r = (((((adc_H << 14) - (((int32_t)dev->dig_H4) << 20) - (((int32_t)dev->dig_H5) * v_x1_u32r)) + ((int32_t)16384)) >> 15) * (((((((v_x1_u32r * ((int32_t)dev->dig_H6)) >> 10) * (((v_x1_u32r * ((int32_t)dev->dig_H3)) >> 11) + ((int32_t)32768))) >> 10) + ((int32_t)2097152)) * ((int32_t)dev->dig_H2) + 8192) >> 14));
    v_x1_u32r = v_x1_u32r - (((((v_x1_u32r >> 15) * (v_x1_u32r >> 15)) >> 7) * ((int32_t)dev->dig_H1)) >> 4);
    v_x1_u32r = (v_x1_u32r < 0) ? 0 : v_x1_u32r;
    v_x1_u32r = (v_x1_u32r > 419430400) ? 419430400 : v_x1_u32r;
    return (uint32_t)(v_x1_u32r>>12);
}

bool bme280_read(bme280_t *dev, float *temperature, float *pressure, float *humidity) {
    if (!dev || !dev->present) return false;

    uint8_t data[8];
    if (read_registers(dev->i2c_port, dev->addr, BME280_REG_PRESS_MSB, data, 8) != ESP_OK) {
        dev->present = false; // dynamic disconnect
        ESP_LOGW(TAG, "read failed, marking absent");
        return false;
    }

    int32_t adc_P = (int32_t)(((uint32_t)data[0] << 12) | ((uint32_t)data[1] << 4) | ((data[2] >> 4) & 0x0F));
    int32_t adc_T = (int32_t)(((uint32_t)data[3] << 12) | ((uint32_t)data[4] << 4) | ((data[5] >> 4) & 0x0F));
    int32_t adc_H = (int32_t)((uint32_t)data[6] << 8) | data[7];

    int32_t T = compensate_T(dev, adc_T);
    uint32_t P = compensate_P(dev, adc_P);
    uint32_t H = compensate_H(dev, adc_H);

    if (temperature) *temperature = T / 100.0f;
    if (pressure) *pressure = P / 256.0f; // convert to Pa approx
    if (humidity) *humidity = H / 1024.0f;

    return true;
}
