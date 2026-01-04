#include <stdio.h>
#include <string.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "driver/i2c.h"
#include "esp_log.h"

#include "bme280.h"
#include "htu21.h"

static const char *TAG = "main";

// I2C pins - adjust to your board
#define I2C_PORT I2C_NUM_0
#define I2C_SDA_PIN 21
#define I2C_SCL_PIN 22
#define I2C_FREQ_HZ 100000

static void i2c_master_init(void) {
	i2c_config_t conf = {0};
	conf.mode = I2C_MODE_MASTER;
	conf.sda_io_num = I2C_SDA_PIN;
	conf.scl_io_num = I2C_SCL_PIN;
	conf.sda_pullup_en = GPIO_PULLUP_ENABLE;
	conf.scl_pullup_en = GPIO_PULLUP_ENABLE;
	conf.master.clk_speed = I2C_FREQ_HZ;
	i2c_param_config(I2C_PORT, &conf);
	i2c_driver_install(I2C_PORT, conf.mode, 0, 0, 0);
}

void app_main(void) {
	ESP_LOGI(TAG, "WeatherStation starting");
	i2c_master_init();

	bme280_t outdoor;
	htu21_t indoor;

	// addresses commonly 0x76 for BME280 and 0x40 for HTU21
	uint8_t bme_addr = 0x76;
	uint8_t htu_addr = 0x40;

	bool bme_ok = bme280_init(&outdoor, I2C_PORT, bme_addr);
	bool htu_ok = htu21_init(&indoor, I2C_PORT, htu_addr);

	while (1) {
		if (!outdoor.present) {
			// try to (re)initialize
			bme_ok = bme280_init(&outdoor, I2C_PORT, bme_addr);
		}
		if (!indoor.present) {
			htu_ok = htu21_init(&indoor, I2C_PORT, htu_addr);
		}

		float t_out = 0, p_out = 0, h_out = 0;
		float t_in = 0, h_in = 0;

		bool have_out = bme280_read(&outdoor, &t_out, &p_out, &h_out);
		bool have_in = htu21_read(&indoor, &t_in, &h_in);

		// print with -- when missing
		if (have_out) {
			ESP_LOGI(TAG, "OUTDOOR: T=%.2f C, P=%.1f Pa, H=%.2f %%", t_out, p_out, h_out);
		} else {
			ESP_LOGI(TAG, "OUTDOOR: T=--, P=--, H=--");
		}
		if (have_in) {
			ESP_LOGI(TAG, "INDOOR: T=%.2f C, H=%.2f %%", t_in, h_in);
		} else {
			ESP_LOGI(TAG, "INDOOR: T=--, H=--");
		}

		vTaskDelay(pdMS_TO_TICKS(2000));
	}
}