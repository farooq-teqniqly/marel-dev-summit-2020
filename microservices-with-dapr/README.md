# Microservices with DAPR

## Introduction
This is a sample pub/sub application written on top of the DAPR framework. 

## Running the sample

### Run the Telemetry Sender app
The Telemetry Sender app sends messages to an Azure Event Hub.

1. Open a terminal window.
2. Switch to the **TelemetrySender** folder.
3. Run ``` dapr run --app-id telemetrysender --dapr-http-port 3500 --components-path ./components dotnet run```

The above command starts up a DAPR sidecar that listens on port 3500. DAPR starts the .NET application which is connected to the sidecar. This lets the console app send Event Hub messages as HTTP POST requests to the sidecar which in turn sends the message to the Event Hub. The sidecar is listening for these requests on the ```/telemetry-queue``` endpoint. Without DAPR, the console app would need to use the Event Hub SDK.

### Run the Telemetry Receiver app
The Telemetry Receiver app reads messages from an Azure Event Hub.

1. Open a terminal window.
2. Switch to the **TelemetryReceiver** folder.
3. Run ```dapr run --app-id telemetryreceiver --app-port 4500 --components-path ./components dotnet run```

The above command starts up a ASP.NET Core app hosted in a console app. The web server listens on port 4500. A DAPR sidecar also starts. The sidecar reads messages from the Event Hub and forwards it to the console app. 

The receiving side is more complex but it follows a standard convention. When the DAPR sidecar starts it issues a GET request to the ```/dapr/subscribe``` endpoint. The endpoint should return the endpoints that should receive Event Hub messages via a POST. When DAPR receives a message it determines that it needs to call the ```telemetry-queue``` endpoint. It then issues a POST to ```/telemetry-queue```. The Event Hub message is in the POST's body. The console app simply outputs the ```data``` portion of the body that represents the temperature reading to the console.

This may look overly complex but this system is designed to support other pub-sub systems such as Azure Service Bus. Hence, the concept of subscriptions still applies to Event Hubs in DAPR.

Again without DAPR, the receiver would need to be coupled to the Event Hub SDK.