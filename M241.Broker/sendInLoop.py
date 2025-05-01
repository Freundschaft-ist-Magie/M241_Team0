import paho.mqtt.publish as publish
import json
import time
import random

while True:
    payload = {
        "humidity": round(random.uniform(30, 70), 3),
        "temperature": round(random.uniform(15, 33), 3),
        "pressure": random.randint(95000, 105000),
        "gas": random.randint(100000, 800000),
        "macAddress": "ABC",
    }

    publish.single(
        topic="room/data",
        payload=json.dumps(payload),
        hostname="localhost",
        port=1883,
        auth={"username": "mosquitto", "password": "admin"},  # if needed
    )
    print("Data sent:", payload)
    time.sleep(5)
