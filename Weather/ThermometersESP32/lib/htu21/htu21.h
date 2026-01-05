#pragma once

#include <stdint.h>
#include <stdbool.h>
#include "driver/i2c.h"

typedef struct {
    i2c_port_t i2c_port;
    uint8_t addr; // 7-bit I2C address
    bool present;
} htu21_t;

// Initialize HTU21 device at address (usually 0x40)
bool htu21_init(htu21_t *dev, i2c_port_t i2c_port, uint8_t addr);
// Read temperature (C) and humidity (%) — returns true on success
bool htu21_read(htu21_t *dev, float *temperature, float *humidity);
