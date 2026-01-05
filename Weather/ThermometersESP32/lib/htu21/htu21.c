/*
 * HTU21 driver for ESP-IDF
 */

#include "htu21.h"
#include <string.h>
#include <stdio.h>
#include "esp_log.h"
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"

static const char *TAG = "htu21";

// HTU21 commands
#define HTU21_CMD_TRIG_TEMP_NOHOLD 0xF3  // Trigger temp measurement (no hold)
#define HTU21_CMD_TRIG_HUM_NOHOLD  0xF5  // Trigger humidity measurement (no hold)

static esp_err_t write_cmd(i2c_port_t port, uint8_t addr, uint8_t cmd) {
    i2c_cmd_handle_t cmdh = i2c_cmd_link_create();
    i2c_master_start(cmdh);
    i2c_master_write_byte(cmdh, (addr << 1) | I2C_MASTER_WRITE, true);
    i2c_master_write_byte(cmdh, cmd, true);
    i2c_master_stop(cmdh);
    esp_err_t ret = i2c_master_cmd_begin(port, cmdh, pdMS_TO_TICKS(1000));
    i2c_cmd_link_delete(cmdh);
    return ret;
}

static esp_err_t read_bytes(i2c_port_t port, uint8_t addr, uint8_t *buf, size_t len) {
    i2c_cmd_handle_t cmdh = i2c_cmd_link_create();
    i2c_master_start(cmdh);
    i2c_master_write_byte(cmdh, (addr << 1) | I2C_MASTER_READ, true);
    if (len > 1) i2c_master_read(cmdh, buf, len - 1, I2C_MASTER_ACK);
    i2c_master_read_byte(cmdh, buf + len - 1, I2C_MASTER_NACK);
    i2c_master_stop(cmdh);
    esp_err_t ret = i2c_master_cmd_begin(port, cmdh, pdMS_TO_TICKS(1000));
    i2c_cmd_link_delete(cmdh);
    return ret;
}

bool htu21_init(htu21_t *dev, i2c_port_t i2c_port, uint8_t addr) {
    if (!dev) return false;
    memset(dev, 0, sizeof(*dev));
    dev->i2c_port = i2c_port;
    dev->addr = addr;
    
    // Try to read User Register (0xE7) to verify device presence
    uint8_t user_reg;
    if (write_cmd(i2c_port, addr, 0xE7) != ESP_OK) {
        ESP_LOGW(TAG, "HTU21 not present at 0x%02X", addr);
        dev->present = false;
        return false;
    }
    vTaskDelay(pdMS_TO_TICKS(10));
    if (read_bytes(i2c_port, addr, &user_reg, 1) != ESP_OK) {
        ESP_LOGW(TAG, "HTU21 not present at 0x%02X", addr);
        dev->present = false;
        return false;
    }
    
    dev->present = true;
    ESP_LOGI(TAG, "HTU21 present at 0x%02X (user_reg=0x%02X)", addr, user_reg);
    return true;
}

bool htu21_read(htu21_t *dev, float *temperature, float *humidity) {
    if (!dev || !dev->present) return false;
    uint8_t buf[3];
    esp_err_t ret;

    // Trigger temperature measurement (no hold master)
    ret = write_cmd(dev->i2c_port, dev->addr, HTU21_CMD_TRIG_TEMP_NOHOLD);
    if (ret != ESP_OK) {
        ESP_LOGW(TAG, "Failed to trigger temp measurement: 0x%X", ret);
        dev->present = false;
        return false;
    }
    
    // Wait for measurement to complete (datasheet: max 50ms for 14-bit)
    vTaskDelay(pdMS_TO_TICKS(60));
    
    // Read temperature result
    ret = read_bytes(dev->i2c_port, dev->addr, buf, 3);
    if (ret != ESP_OK) {
        ESP_LOGW(TAG, "Failed to read temp: 0x%X", ret);
        dev->present = false;
        return false;
    }
    uint16_t rawT = (buf[0] << 8) | buf[1];
    rawT &= ~0x0003;
    float t = -46.85f + 175.72f * ((float)rawT / 65536.0f);

    // Trigger humidity measurement (no hold master)
    ret = write_cmd(dev->i2c_port, dev->addr, HTU21_CMD_TRIG_HUM_NOHOLD);
    if (ret != ESP_OK) {
        ESP_LOGW(TAG, "Failed to trigger humidity measurement: 0x%X", ret);
        dev->present = false;
        return false;
    }
    
    // Wait for measurement to complete (datasheet: max 16ms for 12-bit)
    vTaskDelay(pdMS_TO_TICKS(20));
    
    // Read humidity result
    ret = read_bytes(dev->i2c_port, dev->addr, buf, 3);
    if (ret != ESP_OK) {
        ESP_LOGW(TAG, "Failed to read humidity: 0x%X", ret);
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
