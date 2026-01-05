#pragma once

#include <stdint.h>
#include <stdbool.h>
#include "driver/i2c.h"

typedef struct {
    i2c_port_t i2c_port;
    uint8_t addr;
    bool present;
    // calibration params
    uint16_t dig_T1;
    int16_t dig_T2;
    int16_t dig_T3;

    uint16_t dig_P1;
    int16_t dig_P2;
    int16_t dig_P3;
    int16_t dig_P4;
    int16_t dig_P5;
    int16_t dig_P6;
    int16_t dig_P7;
    int16_t dig_P8;
    int16_t dig_P9;

    uint8_t dig_H1;
    int16_t dig_H2;
    uint8_t dig_H3;
    int16_t dig_H4;
    int16_t dig_H5;
    int8_t dig_H6;

    int32_t t_fine;
} bme280_t;

// Initialize struct and detect device at addr (typically 0x76 or 0x77)
bool bme280_init(bme280_t *dev, i2c_port_t i2c_port, uint8_t addr);

// Read temperature (C), pressure (Pa), humidity (%)
// If sensor not present or read fails, return false
bool bme280_read(bme280_t *dev, float *temperature, float *pressure, float *humidity);
