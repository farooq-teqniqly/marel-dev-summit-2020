using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TelemetryReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = WebHost
                .CreateDefaultBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .UseUrls("http://*:4500")
                .Build();

            host.Run();
        }
    }

    public class Subscription
    {
        [JsonProperty("pubsubname")]
        public string Name { get; set; }

        [JsonProperty("route")]
        public string Route { get; set; }
    }


    public class Startup
    {
        private static readonly int daprPort = 9999;
        private static readonly string stateStoreName = "statestore-redis";
        private static readonly string stateStoreUri = $"http://localhost:{daprPort}/v1.0/state/{stateStoreName}";
        private static readonly HttpClient http = new HttpClient();

        public Startup()
        {
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Map("/dapr/subscribe", SubscribeHandler);
            app.Map("/telemetry-queue", TelemetryHandler);
        }

        private static void SubscribeHandler(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                if (context.Request.Method == "GET")
                {
                    var subscriptions = new List<Subscription>
                    {
                        new Subscription { Name = "telemetry-queue", Route = "telemetry-queue"}
                    };

                    var json = JsonConvert.SerializeObject(subscriptions);
                    await context.Response.WriteAsync(json);
                }
            });
        }

        private static void TelemetryHandler(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                if (context.Request.Method == "POST")
                {
                    if (context.Request.ContentType != "application/cloudevents+json")
                    {
                        context.Response.StatusCode = 415;
                        await context.Response.WriteAsync($"Unsupported content-type {context.Request.ContentType}");
                    }
                    else
                    {
                        var reader = new StreamReader(context.Request.Body);
                        var body = reader.ReadToEnd();

                        var dataObj = JObject.Parse(body)["data"];
                        var saveStateResult = await SaveState(Guid.NewGuid().ToString(), dataObj);
                        
                        await context.Response.WriteAsync(dataObj.ToString());
                        //await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
                else
                {
                    context.Response.StatusCode = 405;
                    await context.Response.WriteAsync(string.Empty);
                }
            });
        }

        private async static Task<string> SaveState(string key, object value)
        {
            var keyValuePair = new { key, value };
            var content = new StringContent(JsonConvert.SerializeObject(new[] { keyValuePair }));
            var response = await http.PostAsync(stateStoreUri, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }
    }
}
