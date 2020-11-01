using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TelemetrySender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var contractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };
            var serializerSettings = new JsonSerializerSettings { ContractResolver = contractResolver, Formatting = Formatting.None };

            var sender = new DaprMessageSender(
                "telemetry-queue",
                "dapr-eh",
                3500,
                new HttpClient());

            // Send temperature readings every second to Azure Event Hub.
            Console.WriteLine("Sending temperature readings. Press ESC to stop.");

            using (sender)
            {
                while (true)
                {
                    var reading = TemperatureReadingFactory.CreateTemperatureReading();
                    var json = JsonConvert.SerializeObject(reading, serializerSettings);

                    await sender.Send(json);
                    
                    Console.WriteLine($"Message sent: {json}");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }
    }
}
