import requests  # Library for making HTTP requests
import json
import time
import random

# The target URL for the POST requests
url = "http://localhost:8081/api/RoomDatas"

print(f"Starting script to send POST requests to: {url}")
print("Press Ctrl+C to stop.")

while True:
    # Generate the random payload data (same as before)
    payload = {
        "humidity": round(random.uniform(30, 70), 3),
        "temperature": round(random.uniform(15, 33), 3),
        "pressure": random.randint(95000, 105000),
        "gas": random.randint(100000, 800000),
        "macAddress": "ABC", # You might want to vary this if needed
    }

    try:
        # Send the POST request
        # The 'json' parameter automatically serializes the dict to JSON
        # and sets the 'Content-Type' header to 'application/json'
        response = requests.post(url, json=payload, timeout=10) # Added a 10-second timeout

        # Print the data that was sent and the server's response status
        print(f"Data sent: {json.dumps(payload)}")
        print(f"Response Status Code: {response.status_code}")

        # Optional: Check if the request was successful (e.g., 200 OK, 201 Created)
        if 200 <= response.status_code < 300:
            print("Server accepted the data.")
            # You could print the response body if the server sends one:
            # print(f"Response Body: {response.text}")
        else:
            print(f"Server returned an error status: {response.status_code}")
            print(f"Response Body: {response.text}") # Print error details from server

    except requests.exceptions.ConnectionError as e:
        print(f"Connection Error: Could not connect to {url}. Is the server running?")
        # print(f"Details: {e}") # Uncomment for more details
    except requests.exceptions.Timeout as e:
        print(f"Timeout Error: The request to {url} timed out.")
    except requests.exceptions.RequestException as e:
        # Catch any other request-related errors
        print(f"An error occurred during the request: {e}")

    # Wait for 5 seconds before sending the next request
    time.sleep(3.4)