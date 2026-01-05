/*
 * HTU21 driver implemented with AVR-like I2C wrappers built on top of ESP-IDF
 * This file exposes simple I2C_* functions compatible with AVR-style libs
 * and provides HTU21 init/read functions.
 */

#include "htu21.h"
#include <string.h>
#include <stdio.h>
#include "esp_log.h"
#include "driver/i2c.h"

static const char *TAG = "htu21";

// Default I2C config (you can change these macros if needed)
#define I2C_PORT_NUM I2C_NUM_0
#define I2C_SDA_GPIO 21
#define I2C_SCL_GPIO 22
#define I2C_FREQ_HZ 100000

// HTU21 commands
#define HTU21_ADDR 0x40
#define HTU21_CMD_TRIG_TEMP 0xE3
#define HTU21_CMD_TRIG_HUM  0xE5

// Internal helper: low-level write of a 1-byte command (to a 7-bit address)
static esp_err_t _i2c_write_cmd(uint8_t addr7, uint8_t cmd) {
    i2c_cmd_handle_t cmdh = i2c_cmd_link_create();
    i2c_master_start(cmdh);
    i2c_master_write_byte(cmdh, (addr7 << 1) | I2C_MASTER_WRITE, true);
    i2c_master_write_byte(cmdh, cmd, true);
    i2c_master_stop(cmdh);
    esp_err_t r = i2c_master_cmd_begin(I2C_PORT_NUM, cmdh, pdMS_TO_TICKS(1000));
    i2c_cmd_link_delete(cmdh);
    return r;
}

static esp_err_t _i2c_read(uint8_t addr7, uint8_t *buf, size_t len, TickType_t timeout_ms) {
    i2c_cmd_handle_t cmdh = i2c_cmd_link_create();
    i2c_master_start(cmdh);
    i2c_master_write_byte(cmdh, (addr7 << 1) | I2C_MASTER_READ, true);
    if (len > 1) i2c_master_read(cmdh, buf, len - 1, I2C_MASTER_ACK);
    i2c_master_read_byte(cmdh, buf + len - 1, I2C_MASTER_NACK);
    i2c_master_stop(cmdh);
    esp_err_t r = i2c_master_cmd_begin(I2C_PORT_NUM, cmdh, pdMS_TO_TICKS(timeout_ms));
    i2c_cmd_link_delete(cmdh);
    return r;
}

// ---------- AVR-like I2C wrappers (simple) ----------
void I2C_Init(void) {
    i2c_config_t conf = {0};
    conf.mode = I2C_MODE_MASTER;
    conf.sda_io_num = I2C_SDA_GPIO;
    conf.scl_io_num = I2C_SCL_GPIO;
    conf.sda_pullup_en = GPIO_PULLUP_ENABLE;
    conf.scl_pullup_en = GPIO_PULLUP_ENABLE;
    conf.master.clk_speed = I2C_FREQ_HZ;
    i2c_param_config(I2C_PORT_NUM, &conf);
    i2c_driver_install(I2C_PORT_NUM, conf.mode, 0, 0, 0);
}

void I2C_Deinit(void) {
    i2c_driver_delete(I2C_PORT_NUM);
}

int I2C_WriteByte(uint8_t addr7bit, uint8_t data) {
    esp_err_t r = _i2c_write_cmd(addr7bit, data);
    return (r == ESP_OK) ? 0 : -1;
}

int I2C_ReadBytes(uint8_t addr7bit, uint8_t *buf, size_t len) {
    esp_err_t r = _i2c_read(addr7bit, buf, len, 1000);
    return (r == ESP_OK) ? 0 : -1;
}

void I2C_ScanConnectedDevices(void (*lprint)(const char *)) {
    char buf[32];
    for (int addr = 0; addr < 128; ++addr) {
        // skip special addresses
        if (addr == 0) continue;
        i2c_cmd_handle_t cmd = i2c_cmd_link_create();
        i2c_master_start(cmd);
        i2c_master_write_byte(cmd, (addr << 1) | I2C_MASTER_WRITE, true);
        i2c_master_stop(cmd);
        esp_err_t r = i2c_master_cmd_begin(I2C_PORT_NUM, cmd, pdMS_TO_TICKS(100));
        i2c_cmd_link_delete(cmd);
        if (r == ESP_OK) {
            sprintf(buf, "Device at 0x%02X\n", addr);
            lprint(buf);
        }
    }
}

// ---------- HTU21 functions using wrappers ----------
bool htu21_init(htu21_t *dev, uint8_t addr) {
    if (!dev) return false;
    dev->addr = addr;
    dev->present = false;
    I2C_Init();

    // try simple probe: write no-command read of 1 byte (some devices NACK; better approach: try reading register)
    uint8_t buf[1];
    if (I2C_ReadBytes(addr, buf, 1) != 0) {
        ESP_LOGW(TAG, "HTU21 not present at 0x%02X", addr);
        dev->present = false;
        return false;
    }
    dev->present = true;
    ESP_LOGI(TAG, "HTU21 present at 0x%02X", addr);
    return true;
}

bool htu21_read(htu21_t *dev, float *temperature, float *humidity) {
    if (!dev || !dev->present) return false;
    uint8_t buf[3];

    if (I2C_WriteByte(dev->addr, HTU21_CMD_TRIG_TEMP) != 0) {
        dev->present = false; return false;
    }
    vTaskDelay(pdMS_TO_TICKS(50));
    if (I2C_ReadBytes(dev->addr, buf, 3) != 0) { dev->present = false; return false; }
    uint16_t rawT = (buf[0] << 8) | buf[1];
    rawT &= ~0x0003;
    float t = -46.85f + 175.72f * ((float)rawT / 65536.0f);

    if (I2C_WriteByte(dev->addr, HTU21_CMD_TRIG_HUM) != 0) { dev->present = false; return false; }
    vTaskDelay(pdMS_TO_TICKS(30));
    if (I2C_ReadBytes(dev->addr, buf, 3) != 0) { dev->present = false; return false; }
    uint16_t rawH = (buf[0] << 8) | buf[1];
    rawH &= ~0x0003;
    float h = -6.0f + 125.0f * ((float)rawH / 65536.0f);

    if (temperature) *temperature = t;
    if (humidity) *humidity = h;
    return true;
}
