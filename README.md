# Microservices with DAPR

## Introduction
This is a sample pub/sub application written on top of the DAPR framework. 

## Running the sample

### Run the Telemetry Sender app
The Telemetry Sender app sends messages to an Azure Event Hub.

1. Open a terminal window.
2. Switch to the **TelemetrySender** folder.
3. Run ``` dapr run --app-id telemetrysender --dapr-http-port 3500 --components-path ./components dotnet run```

The above command starts up a DAPR sidecar that listens on port 3500. DAPR starts the .NET application which is connected to the sidecar. This lets the console app send Event Hub messages as HTTP POST requests to the sidecar which in turn sends the message to the Event Hub. Without DAPR, the console app would need to use the Event Hub SDK.