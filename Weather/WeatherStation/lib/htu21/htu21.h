#pragma once

#include <stdint.h>
#include <stdbool.h>

// Simple AVR-like I2C API (implemented on top of ESP-IDF I2C).
// This provides compatibility with AVR-style libs (I2C_Init, I2C_Write, etc.)
// and is used by the HTU21 functions below.

void I2C_Init(void); // initialize I2C driver with default pins
void I2C_Deinit(void);
// write a single byte to bus (address or data depending on use). Returns 0 on ACK, non-zero on NAK/error
int I2C_WriteByte(uint8_t addr7bit, uint8_t data);
// read 'len' bytes from device at addr7bit into buf. Returns 0 on success
int I2C_ReadBytes(uint8_t addr7bit, uint8_t *buf, size_t len);
// Scan devices and call lprint callback for each printed token
void I2C_ScanConnectedDevices(void (*lprint)(const char *));

typedef struct {
    uint8_t addr; // 7-bit I2C address
    bool present;
} htu21_t;

// Initialize HTU21 device at address (usually 0x40)
bool htu21_init(htu21_t *dev, uint8_t addr);
// Read temperature (C) and humidity (%) — returns true on success
bool htu21_read(htu21_t *dev, float *temperature, float *humidity);
