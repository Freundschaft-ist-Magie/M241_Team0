#include <Wire.h>
#include <SPI.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BME680.h>
#include <Arduino_JSON.h>
#include <WiFiNINA.h>
#include <PubSubClient.h>
#include "secrets.h"
#include "config.h"

const char* wiFiSsid = WIFI_SSID;
const char* wiFiPass = WIFI_PASS;
const char* mqttUser = MQTT_USER;
const char* mqttPass = MQTT_PASS;
const char* mqttHost = MQTT_HOST;
const int mqttPort = MQTT_PORT;
const char* mqttQueue = MQTT_QUEUE;
const boolean debuggingEnabled = DEBUGGING_ENABLED;

String macAddress;
WiFiSSLClient wiFiClient;

Adafruit_BME680 bme(10);
PubSubClient mqttClient(wiFiClient);

void setup() {
  Serial.begin(9600);
  if(debuggingEnabled) {
    while (!Serial);
  }

  initSensor();

  macAddress = getMacAddress();
  connectWiFi();

  mqttClient.setServer(mqttHost, mqttPort);
  connectMqtt();
}

void loop() {
  if (bme.performReading()) {
    if (WiFi.status() == WL_CONNECTED) {
      if (mqttClient.connected()) {
        publishSensorData();
      } else {
        reconnectMqtt();
      }
    } else {
      reconnectWiFi();
    }
  } else {
    Serial.println("Failed to perform BME680 sensor reading :(");
  }
}

void initSensor() {
  if (!bme.begin()) {
    Serial.println("Could not find a valid BME680 sensor, check wiring!");
    while (1);
  }
}

String getMacAddress() {
  byte mac[6];
  WiFi.macAddress(mac);
  char buf[18];
  sprintf(buf, "%02X:%02X:%02X:%02X:%02X:%02X", mac[0], mac[1], mac[2], mac[3], mac[4], mac[5]);
  return String(buf);
}

void connectWiFi() {
  int wiFiStatus = WL_IDLE_STATUS;
  while (wiFiStatus != WL_CONNECTED) {
    if(wiFiStatus != WL_IDLE_STATUS) {
      delay(5000);
    }
    wiFiStatus = attemptWiFiConnection();
  }
  
  Serial.print("Connected to network named: ");
  Serial.println(wiFiSsid);
  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());
}

int attemptWiFiConnection() {
  Serial.print("Attempting to connect to Network named: ");
  Serial.println(wiFiSsid);
  return WiFi.begin(wiFiSsid, wiFiPass);
}

void connectMqtt() {
  while (!mqttClient.connected()) {
    if (WiFi.status() == WL_CONNECTED) {
      if (!attemptMqttConnection()) {
        delay(5000);
      }
    } else {
      reconnectWiFi();
    }
  }
  Serial.println("MQTT connection established!");
}

bool attemptMqttConnection() {
  Serial.print("Attempting to connect to MQTT ");
  Serial.print(mqttHost);
  Serial.print(":");
  Serial.println(mqttPort);
  String clientId = "arduinoClient-" + macAddress;
  return mqttClient.connect(clientId.c_str(), mqttUser, mqttPass);
}

void reconnectWiFi() {
  Serial.println("Disconnected from WiFi, trying to reconnect...");
  connectWiFi();
}

void reconnectMqtt() {
  Serial.println("Disconnected from MQTT server, trying to reconnect...");
  connectMqtt();
}

void publishSensorData() {
  JSONVar json;
  json["id"] = 0;
  json["macAddress"] = macAddress;
  json["temperature"] = bme.temperature;
  json["humidity"] = bme.humidity;
  json["pressure"] = bme.pressure;
  json["gas"] = bme.gas_resistance;

  mqttClient.publish(mqttQueue, JSON.stringify(json).c_str());
  Serial.println("Published data");
}
