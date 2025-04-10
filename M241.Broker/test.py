import paho.mqtt.publish as publish
import json

payload = {
    "humidity": 5,
    "temperature": 5,
    "pressure": 5,
    "gas": 5,
    "macAddress": "ABC",
}

publish.single(
    topic="room/data",
    payload=json.dumps(payload),
    hostname="localhost",
    port=1883,
    auth={"username": "mosquitto", "password": "admin"},  # if needed
)
