using System.Text.Json;
using System.Text;
using System.Net.Http;

namespace MeteredSheduler
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                string azurefuntionurl = "ADD AZURE FUNCTION LINK HERE";
                string subscriptionId = Environment.GetEnvironmentVariable("SUSBSCRIPTION_ID");
                string planId = Environment.GetEnvironmentVariable("PLAN_ID");
                string dimensionDemo = "cpu1";
                using StringContent jsonContent = new(
                    JsonSerializer.Serialize(new
                    {
                        Dimension = dimensionDemo,
                        Quantity = 1,
                        EffectiveStartTime = DateTime.Now.ToUniversalTime().ToString(),
                        PlanId = planId,
                        ResourceId = subscriptionId,
                    }),
                    Encoding.UTF8,
                    "application/json");
                HttpClient client = new HttpClient();
                using HttpResponseMessage response = await client.PostAsync(
                    azurefuntionurl,
                    jsonContent);

                var jsonResponse = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Metered usage submitted and response was {jsonResponse}\n");

                //sleep 5 min
                await Task.Delay(300000, stoppingToken);
            }



        }
    }
}
