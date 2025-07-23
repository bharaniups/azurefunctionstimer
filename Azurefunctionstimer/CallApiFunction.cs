using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Azurefunctionstimer
{
    public class CallApiFunction
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger<CallApiFunction> _logger;
        public CallApiFunction(IHttpClientFactory httpClient, ILogger<CallApiFunction> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        [Function("CallApiFunction")]
        public async Task Run([TimerTrigger("*/10 * * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"Timer triggered at: {DateTime.Now}");
            var client = _httpClient.CreateClient();
            var payload = new
            {
                BlobContainer = "originals",
                FileName = "pexels-izafi-33061425.jpg" // You can loop through all files if needed
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://azurefunctionswebapi-c2c9d6dkd8gdbjc2.centralindia-01.azurewebsites.net/resize", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Resize request sent successfully.");
            }
            else
            {
                _logger.LogError("Resize request failed: " + response.StatusCode);
            }
        }
    }

}
