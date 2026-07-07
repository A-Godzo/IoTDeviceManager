import time
import random
import json
import urllib.request


API_URL = "http://localhost:5215/api/telemetry"


DEVICE_MAC = "00:1A:2B:3C:4D:5E"
DATA_PIN = "GPIO 4"

print("Starting Virtual IoT Device Stream...")
print(f"Target Endpoint: {API_URL}")
print(f"Emulating MCU [{DEVICE_MAC}] on Pin [{DATA_PIN}]\n")

while True:
    try:

        mock_temperature = round(random.uniform(22.0, 40.0), 2)


        payload = {
            "macAddress": DEVICE_MAC,
            "dataPin": DATA_PIN,
            "sensorValue": mock_temperature
        }


        json_data = json.dumps(payload).encode('utf-8')


        req = urllib.request.Request(
            API_URL,
            data=json_data,
            headers={'Content-Type': 'application/json'},
            method='POST'
        )


        with urllib.request.urlopen(req) as response:
            response_body = response.read().decode('utf-8')
            server_reply = json.loads(response_body)

            print(f"[SENT] Value: {mock_temperature}°C | Server Response: {server_reply.get('message')}")

    except urllib.error.HTTPError as e:
        print(f"❌ Server Error ({e.code}): {e.read().decode('utf-8')}")
    except urllib.error.URLError as e:
        print("❌ Connection Error! Is your ASP.NET Core web application currently running?")


    time.sleep(3)