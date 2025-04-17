#include <Arduino_JSON.h>
#include <WiFiNINA.h>
#include <SPI.h>
#include "secrets.h"
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
char mqttPass[] = MQTT_PASS;
int status = WL_IDLE_STATUS;
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

  while (status != WL_CONNECTED) {
    Serial.print("Attempting to connect to Network named: ");
    Serial.println(wifiSsid);

    status = WiFi.begin(wifiSsid, wifiPass);
    delay(10000);
  }
  
  Serial.print("Connected to network named: ");
  Serial.println(wifiSsid);
  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());
  
  macAddress = getMacAddress();

  mqttClient.setServer("192.168.165.222", 1883);
  mqttClient.connect("arduinoClient", "mosquitto", mqttPass);
}

void loop() {
  if (bme.performReading()) {
    if (!mqttClient.connected()) {

    }
    JSONVar json(1024);
    json["id"] = 0;
    json["macAddress"] = macAddress;
    json["temperature"] = bme.temperature;
    json["humidity"] = bme.humidity;
    json["pressure"] = bme.pressure;
    json["gas"] = bme.gas_resistance;

    mqttClient.publish("room/data", JSON.stringify(json).c_str());
    Serial.println("Published Data");
  } else {
    Serial.println("Failed to perform reading :(");
  }
}

void reconnect() {
  while (!mqttClient.connected()) {
    if (mqttClient.connect("arduinoClient", "mosquitto", mqttPass)) {
    } else {
      delay(5000);
    }
  }
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
