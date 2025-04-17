#include <Arduino_JSON.h>
#include <WiFiNINA.h>
#include <SPI.h>
#include "secrets.h"
#include "config.h"
#include <PubSubClient.h>
#include <Wire.h>
#include <SPI.h>
#include <Adafruit_Sensor.h>
#include "Adafruit_BME680.h"

#define BME_SCK 13
#define BME_MISO 12
#define BME_MOSI 11
#define BME_CS 10
#define SEALEVELPRESSURE_HPA (1013.25)

char wifiSsid[] = WIFI_SSID;
char wifiPass[] = WIFI_PASS;
char mqttUser[] = MQTT_USER;
char mqttPass[] = MQTT_PASS;
char mqttHost[] = MQTT_HOST;
int mqttPort = MQTT_PORT;
char mqttQueue[] = MQTT_QUEUE;

String macAddress;

Adafruit_BME680 bme(BME_CS);
WiFiClient wifiClient;
PubSubClient mqttClient(wifiClient);

void setup() {
  Serial.begin(9600);
  while (!Serial);

  if (!bme.begin()) {
    Serial.println("Could not find a valid BME680 sensor, check wiring!");
    while (1);
  }

  connectWifi();

  macAddress = getMacAddress();
  Serial.print("Mac Address: ");
  Serial.println(macAddress);

  mqttClient.setServer(mqttHost, mqttPort);
  connectMqtt();
}

void loop() {
  if (bme.performReading()) {
    if (WiFi.status() == WL_CONNECTED) {
      if (mqttClient.connected()) {
        JSONVar json(1024);
        json["id"] = 0;
        json["macAddress"] = macAddress;
        json["temperature"] = bme.temperature;
        json["humidity"] = bme.humidity;
        json["pressure"] = bme.pressure;
        json["gas"] = bme.gas_resistance;

        mqttClient.publish(mqttQueue, JSON.stringify(json).c_str());
        Serial.println("Published data");
      } else {
        Serial.println("Disconnected from MQTT server, trying to reconnect...");
        connectMqtt();
      }
    } else {
      reconnectWifi();
    }
  } else {
    Serial.println("Failed to perform BME680 sensor reading :(");
  }
}

void connectWifi() {
  int wifiStatus = WL_IDLE_STATUS;
  while (wifiStatus != WL_CONNECTED) {
    Serial.print("Attempting to connect to Network named: ");
    Serial.println(wifiSsid);

    wifiStatus = WiFi.begin(wifiSsid, wifiPass);

    delay(10000);
  }
  
  Serial.print("Connected to network named: ");
  Serial.println(wifiSsid);
  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());
}

void reconnectWifi() {
  Serial.println("Disconnected from WiFi, trying to reconnect...");
  connectWifi();
}

void connectMqtt() {
  while (!mqttClient.connected()) {
    if (WiFi.status() == WL_CONNECTED) {
      Serial.print("Attempting to connect to MQTT ");
      Serial.print(mqttHost);
      Serial.print(":");
      Serial.println(mqttPort);
      if (!mqttClient.connect("arduinoClient", mqttUser, mqttPass)) {
        delay(5000);
      }
    } else {
      reconnectWifi();
    }
  }
  Serial.println("MQTT connection established!");
}

String getMacAddress() {
  byte mac[6];
  WiFi.macAddress(mac);
  String macStr = "";

  for (int i = 0; i < 6; i++) {
    if (mac[i] < 16) {
      macStr += "0";
    }
    macStr += String(mac[i], HEX);
    if (i < 5) macStr += ":";
  }

  macStr.toUpperCase();
  return macStr;
}
