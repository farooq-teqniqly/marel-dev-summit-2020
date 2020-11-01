using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TelemetrySender
{
    public class DaprMessageSender : IDisposable
    {
        private bool disposed = false;
        private readonly string sendUrl;
        private readonly HttpClient http;

        public DaprMessageSender(
            string outputBindingName,
            string targetEventHubName,
            int daprPort,
            HttpClient httpClient)
        {
            this.sendUrl = $"http://localhost:{daprPort}/v1.0/publish/{outputBindingName}/{targetEventHubName}";
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.http = httpClient;
        }

        public async Task<string> Send(string json)
        {
            var requestContent = new StringContent(json, Encoding.UTF8);
            var response = await this.http.PostAsync(this.sendUrl, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(responseContent);
            }

            return responseContent;
            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.http.Dispose();
            }

            this.disposed = true;
        }
    }
}
