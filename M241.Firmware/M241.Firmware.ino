#include <Wire.h>
#include <SPI.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BME680.h>
#include <Arduino_JSON.h>
#include <WiFiNINA.h>
#include <MQTTClient.h>
#include <WiFiUdp.h>
#include <NTPClient.h>
#include "secrets.h"
#include "config.h"

const char* wiFiSsid = WIFI_SSID;
const char* wiFiPass = WIFI_PASS;
const char* mqttUser = MQTT_USER;
const char* mqttPass = MQTT_PASS;
const char* mqttHost = MQTT_HOST;
const int mqttPort = MQTT_PORT;
const char* mqttQueue = MQTT_QUEUE;
const char* mqttPingTopic = MQTT_PING_TOPIC;
const boolean debuggingEnabled = DEBUGGING_ENABLED;

#define SENSOR_POWER_PIN 8

String macAddress;
WiFiSSLClient wiFiClient;
MQTTClient mqttClient(1024);
Adafruit_BME680 bme(10);
WiFiUDP ntpUDP;
NTPClient timeClient(ntpUDP, "pool.ntp.org", 0, 60000);

void setup() {
  Serial.begin(9600);
  if (debuggingEnabled) {
    while (!Serial);
  }

  pinMode(LED_BUILTIN, OUTPUT);
  pinMode(SENSOR_POWER_PIN, OUTPUT);
  digitalWrite(SENSOR_POWER_PIN, HIGH);
  delay(150);

  initSensor();
  macAddress = getMacAddress();
  Serial.print("MAC address: ");
  Serial.println(macAddress);
  connectWiFi();

  timeClient.begin();
  timeClient.update();

  mqttClient.begin(mqttHost, mqttPort, wiFiClient);
  mqttClient.onMessage(messageReceived);
  connectMqtt();
}

void loop() {
  timeClient.update();
  mqttClient.loop();

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

  delay(1000);
}

void messageReceived(String &topic, String &payload) {
  if (topic == mqttPingTopic && payload == macAddress) {
    Serial.println("Received ping → LED blink");
    deactivateSensor();

    for (int i = 0; i < 15; i++) {
      digitalWrite(LED_BUILTIN, HIGH);
      delay(100);
      digitalWrite(LED_BUILTIN, LOW);
      delay(100);
    }

    pinMode(LED_BUILTIN, INPUT);

    digitalWrite(SENSOR_POWER_PIN, HIGH);
    delay(150);
    initSensor();
  }
}

void initSensor() {
  if (!bme.begin()) {
    Serial.println("Could not find a valid BME680 sensor, check wiring!");
    while (1);
  }
}

void deactivateSensor() {
    digitalWrite(SENSOR_POWER_PIN, LOW);
    delay(100);
    pinMode(13, OUTPUT);
    pinMode(11, OUTPUT);
    pinMode(12, OUTPUT);
    pinMode(10, OUTPUT);
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
    if (wiFiStatus != WL_IDLE_STATUS) {
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
  mqttClient.subscribe(mqttPingTopic);
}

bool attemptMqttConnection() {
  Serial.print("Attempting to connect to MQTT ");
  Serial.print(mqttHost);
  Serial.print(":");
  Serial.println(mqttPort);
  return mqttClient.connect(("arduinoClient-" + macAddress).c_str(), mqttUser, mqttPass);
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
  json["macAddress"] = macAddress;
  json["timestamp"] = timeClient.getEpochTime();
  json["temperature"] = bme.temperature;
  json["humidity"] = bme.humidity;
  json["pressure"] = bme.pressure;
  json["gas"] = bme.gas_resistance;

  if (mqttClient.publish(mqttQueue, JSON.stringify(json).c_str(), false, 2)) {
    Serial.println("Published data (QoS = exactly once)");
  } else {
    Serial.println("Failed to publish");
  }
}
