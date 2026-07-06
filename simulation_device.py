import time
import random
import json
import urllib.request

# CONFIGURATION: Update the port number if your Rider project uses a different localhost port
API_URL = "http://localhost:5215/api/telemetry"

# Target Hardware Profile (Make sure this MAC address exists in your Microcontrollers database table!)
DEVICE_MAC = "00:1A:2B:3C:4D:5E"
DATA_PIN = "GPIO 4"

print("🚀 Starting Virtual IoT Device Stream...")
print(f"📡 Target Endpoint: {API_URL}")
print(f"🔧 Emulating MCU [{DEVICE_MAC}] on Pin [{DATA_PIN}]\n")

while True:
    try:
        # 1. Simulate a fluctuating temperature reading (around 22.0°C - 26.0°C)
        mock_temperature = round(random.uniform(22.0, 40.0), 2)

        # 2. Package the data to match your C# TelemetryPayload model exactly
        payload = {
            "macAddress": DEVICE_MAC,
            "dataPin": DATA_PIN,
            "sensorValue": mock_temperature
        }

        # 3. Convert dictionary to raw JSON bytes
        json_data = json.dumps(payload).encode('utf-8')

        # 4. Build the HTTP request
        req = urllib.request.Request(
            API_URL,
            data=json_data,
            headers={'Content-Type': 'application/json'},
            method='POST'
        )

        # 5. Send the packet across localhost to your ASP.NET server
        with urllib.request.urlopen(req) as response:
            response_body = response.read().decode('utf-8')
            server_reply = json.loads(response_body)

            print(f"[SENT] Value: {mock_temperature}°C | Server Response: {server_reply.get('message')}")

    except urllib.error.HTTPError as e:
        print(f"❌ Server Error ({e.code}): {e.read().decode('utf-8')}")
    except urllib.error.URLError as e:
        print("❌ Connection Error! Is your ASP.NET Core web application currently running?")

    # Wait 5 seconds before sending the next telemetry reading
    time.sleep(5)