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
#define I2C_SDA_PIN 41
#define I2C_SCL_PIN 42
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

static void i2c_scan(void) {
	ESP_LOGI(TAG, "Scanning I2C bus...");
	int found = 0;
	for (int addr = 1; addr < 127; addr++) {
		i2c_cmd_handle_t cmd = i2c_cmd_link_create();
		i2c_master_start(cmd);
		i2c_master_write_byte(cmd, (addr << 1) | I2C_MASTER_WRITE, true);
		i2c_master_stop(cmd);
		esp_err_t ret = i2c_master_cmd_begin(I2C_PORT, cmd, pdMS_TO_TICKS(100));
		i2c_cmd_link_delete(cmd);
		if (ret == ESP_OK) {
			ESP_LOGI(TAG, "  Found device at 0x%02X", addr);
			found++;
		}
	}
	ESP_LOGI(TAG, "Scan complete. Found %d device(s)", found);
}

void app_main(void) {
	ESP_LOGI(TAG, "WeatherStation starting");
	i2c_master_init();
	i2c_scan();

	bme280_t outdoor;
	htu21_t indoor;

	uint8_t bme_addr = 0x76;
	uint8_t htu_addr = 0x40;

	bme280_init(&outdoor, I2C_PORT, bme_addr);
	htu21_init(&indoor, I2C_PORT, htu_addr);

	while (1) {
		if (!outdoor.present) {
			// try to (re)initialize
			bme280_init(&outdoor, I2C_PORT, bme_addr);
		}
		if (!indoor.present) {
			htu21_init(&indoor, I2C_PORT, htu_addr);
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