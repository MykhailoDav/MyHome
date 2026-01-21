#include <Arduino.h>
#include <Wire.h>
#include "bme280.h"
#include "htu21.h"

// I2C pins for ESP-01S
#define I2C_SDA_PIN 0  // GPIO0
#define I2C_SCL_PIN 2  // GPIO2

bme280_t outdoor;
htu21_t indoor;

void i2c_scan() {
    Serial.println("Scanning I2C bus...");
    int found = 0;
    for (int addr = 1; addr < 127; addr++) {
        Wire.beginTransmission(addr);
        if (Wire.endTransmission() == 0) {
            Serial.print("  Found device at 0x");
            Serial.println(addr, HEX);
            found++;
        }
    }
    Serial.print("Scan complete. Found ");
    Serial.print(found);
    Serial.println(" device(s)");
}

void setup() {
    Serial.begin(115200);
    delay(100);
    Serial.println("\nWeatherStation starting");
    
    Wire.begin(I2C_SDA_PIN, I2C_SCL_PIN);
    i2c_scan();
    
    bme280_init(&outdoor, 0x76);
    htu21_init(&indoor, 0x40);
}

void loop() {
    if (!outdoor.present) {
        bme280_init(&outdoor, 0x76);
    }
    if (!indoor.present) {
        htu21_init(&indoor, 0x40);
    }
    
    float t_out = 0, p_out = 0, h_out = 0;
    float t_in = 0, h_in = 0;
    
    bool have_out = bme280_read(&outdoor, &t_out, &p_out, &h_out);
    bool have_in = htu21_read(&indoor, &t_in, &h_in);
    
    if (have_out) {
        Serial.print("OUTDOOR: T=");
        Serial.print(t_out);
        Serial.print(" C, P=");
        Serial.print(p_out);
        Serial.print(" Pa, H=");
        Serial.print(h_out);
        Serial.println(" %");
    } else {
        Serial.println("OUTDOOR: T=--, P=--, H=--");
    }
    
    if (have_in) {
        Serial.print("INDOOR: T=");
        Serial.print(t_in);
        Serial.print(" C, H=");
        Serial.print(h_in);
        Serial.println(" %");
    } else {
        Serial.println("INDOOR: T=--, H=--");
    }
    
    delay(2000);
}