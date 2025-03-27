#include <WiFiNINA.h>
#include <SPI.h>
#include "secrets.h"
#include <ArduinoHttpClient.h>
#include <Wire.h>
#include <SPI.h>
#include <Adafruit_Sensor.h>
#include "Adafruit_BME680.h"

#define BME_SCK 13
#define BME_MISO 12
#define BME_MOSI 11
#define BME_CS 10
#define SEALEVELPRESSURE_HPA (1013.25)

char ssid[] = SECRET_SSID;
char pass[] = SECRET_PASS;
int status = WL_IDLE_STATUS;
String macAddress;

Adafruit_BME680 bme(BME_CS);
WiFiClient wifiClient;
HttpClient client = HttpClient(wifiClient, "192.168.168.222", 8080);

void setup() {
  Serial.begin(9600);
  while (!Serial);

  if (!bme.begin()) {
    Serial.println("Could not find a valid BME680 sensor, check wiring!");
    while (1);
  }

  while (status != WL_CONNECTED) {
    Serial.print("Attempting to connect to Network named: ");
    Serial.println(ssid);

    status = WiFi.begin(ssid, pass);
    delay(10000);
  }
  
  Serial.print("Connected to network named: ");
  Serial.println(ssid);
  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());
  
  macAddress = getMacAddress();
  performHealthCheck();
}

void loop() {
  
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

void performHealthCheck() {
  Serial.println("Performing API health check...");
  client.beginRequest();
  client.get("/healthz");
  client.endRequest();

  Serial.print("Status code for health check: ");
  Serial.println(client.responseStatusCode());
  Serial.print("Response for health check: ");
  Serial.println(client.responseBody());
}
