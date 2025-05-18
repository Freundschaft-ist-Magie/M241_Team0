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
#define CACHE_LIMIT 60
#define MEASURE_INTERVAL_CONNECTED 1000
#define MEASURE_INTERVAL_DISCONNECTED 30000
#define RECONNECT_INTERVAL 5000

String macAddress;
WiFiSSLClient wiFiClient;
MQTTClient mqttClient(1024);
Adafruit_BME680 bme(10);
WiFiUDP ntpUDP;
NTPClient timeClient(ntpUDP, "pool.ntp.org", 0, 60000);

struct CachedReading {
  unsigned long timestamp;
  float temperature;
  float humidity;
  float pressure;
  float gas;
};

CachedReading cache[CACHE_LIMIT];
int cacheIndex = 0;
bool isCaching = false;

unsigned long lastMeasureTime = 0;
unsigned long lastReconnectAttempt = 0;

void setup() {
  Serial.begin(9600);
  if (debuggingEnabled) while (!Serial);

  pinMode(LED_BUILTIN, OUTPUT);
  pinMode(SENSOR_POWER_PIN, OUTPUT);
  digitalWrite(SENSOR_POWER_PIN, HIGH);
  delay(150);

  initSensor();
  macAddress = getMacAddress();
  attemptWiFiConnection();

  timeClient.begin();
  timeClient.update();

  mqttClient.begin(mqttHost, mqttPort, wiFiClient);
  mqttClient.onMessage(messageReceived);
  attemptMqttConnection();
}

void loop() {
  timeClient.update();
  mqttClient.loop();

  bool isWiFiConnected = WiFi.status() == WL_CONNECTED;
  bool isMQTTConnected = mqttClient.connected();
  unsigned long now = millis();

  if (now - lastReconnectAttempt >= RECONNECT_INTERVAL) {
    lastReconnectAttempt = now;
    if (!isWiFiConnected) {
      attemptWiFiConnection();
    } else if (!isMQTTConnected) {
      attemptMqttConnection();
    }
  }

  unsigned long interval = (isWiFiConnected && isMQTTConnected) ? MEASURE_INTERVAL_CONNECTED : MEASURE_INTERVAL_DISCONNECTED;
  if (now - lastMeasureTime >= interval) {
    lastMeasureTime = now;

    if (bme.performReading()) {
      if (isWiFiConnected && isMQTTConnected) {
        if (isCaching && cacheIndex > 0) sendCachedReadings();
        publishSensorData();
      } else {
        cacheReading();
      }
    } else {
      Serial.println("Failed to perform BME680 sensor reading :(");
    }
  }
}

void sendCachedReadings() {
  Serial.print("Sending ");
  Serial.print(cacheIndex);
  Serial.println(" cached readings...");

  for (int i = 0; i < cacheIndex; i++) {
    JSONVar json;
    json["macAddress"] = macAddress;
    json["timestamp"] = cache[i].timestamp;
    json["temperature"] = cache[i].temperature;
    json["humidity"] = cache[i].humidity;
    json["pressure"] = cache[i].pressure;
    json["gas"] = cache[i].gas;

    bool success = mqttClient.publish(mqttQueue, JSON.stringify(json).c_str(), false, 2);
    if (!success) {
      Serial.print("Failed to publish cached reading #");
      Serial.println(i);
      return;
    }
  }

  cacheIndex = 0;
  isCaching = false;
  Serial.println("All cached readings published successfully.");
}

void cacheReading() {
  if (cacheIndex < CACHE_LIMIT) {
    isCaching = true;
    CachedReading r;
    r.timestamp = timeClient.getEpochTime();
    r.temperature = bme.temperature;
    r.humidity = bme.humidity;
    r.pressure = bme.pressure;
    r.gas = bme.gas_resistance;
    cache[cacheIndex++] = r;
    Serial.print("Cached reading #");
    Serial.println(cacheIndex);
  } else {
    Serial.println("Cache full. Skipping reading.");
  }
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

int attemptWiFiConnection() {
  Serial.print("Attempting to connect to Network named: ");
  Serial.println(wiFiSsid);
  return WiFi.begin(wiFiSsid, wiFiPass);
}

bool attemptMqttConnection() {
  Serial.print("Attempting to connect to MQTT ");
  Serial.print(mqttHost);
  Serial.print(":");
  Serial.println(mqttPort);
  bool result = mqttClient.connect(("arduinoClient-" + macAddress).c_str(), mqttUser, mqttPass);
  if (result) {
    mqttClient.subscribe(mqttPingTopic);
    Serial.println("MQTT connection established!");
  }
  return result;
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
    Serial.println("Failed to publish live reading");
  }
}
