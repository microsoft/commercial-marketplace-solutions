using System.Text.Json;
using System.Text;
using System.Net.Http;
using MeteredSheduler.Services;
using System.Numerics;
using Amazon.Auth.AccessControlPolicy;
using System.Net.Http.Json;
using Newtonsoft.Json;
using MeteredSheduler.Entities;
using System.Net.Http.Headers;
using Amazon.Runtime;
using System.Runtime.ConstrainedExecution;
using Newtonsoft.Json.Linq;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;

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

            while (!stoppingToken.IsCancellationRequested )
            {
                
            var token = await getMsiToken();
                if (token == null || token == "")
                {
                    _logger.LogInformation("Can not get token");
                }
                else
                {
                    try
                    {
                        var _mongodbService = new MongoDbService();
                        _logger.LogInformation($"Worker running at: {DateTimeOffset.Now.ToString()}" );
                        string dimensions = Environment.GetEnvironmentVariable("DIMS_LIST");
                        string planId = Environment.GetEnvironmentVariable("PLAN_ID");
                        var marketplaceAPI = Environment.GetEnvironmentVariable("MARKETPLACE_API");
                        var subscriptionId = Environment.GetEnvironmentVariable("EXTENSION_RESOURCE_ID");

                        Console.WriteLine($"dimensions is {dimensions}" );
                        Console.WriteLine($"planId is {planId}" );
                        Console.WriteLine($"marketplaceAPI is {marketplaceAPI}");
                        Console.WriteLine($"subscriptionId is {subscriptionId}" );


                        var dimensionList = dimensions.Split(',');



                        foreach (var dimension in dimensionList)
                        {
                            Console.WriteLine($"current dimensions is {dimension}" );
                            var dimkey = dimension.Split('|');
                            var items = await _mongodbService.GetQueuedItemAsync(dimkey[0]);
                            decimal count = Convert.ToDecimal(items.Count()) / Convert.ToDecimal(dimkey[1]);
                            
                            Console.WriteLine($"current count name is {count.ToString()}" );




                            using StringContent jsonContent = new(
                            System.Text.Json.JsonSerializer.Serialize(new
                            {
                                Dimension = dimkey[0],
                                Quantity = count,
                                EffectiveStartTime = DateTime.Now.ToUniversalTime().ToString(),
                                PlanId = planId,
                                ResourceUri = subscriptionId,
                            }),
                            Encoding.UTF8,
                            "application/json");


                            HttpClient client = new HttpClient();
                            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                            using HttpResponseMessage response = await client.PostAsync(
                                marketplaceAPI,
                                jsonContent);

                            var jsonResponse = await response.Content.ReadAsStringAsync();
                            _logger.LogInformation($"Metered usage submitted and response was {jsonResponse}\n");

                            if (response.IsSuccessStatusCode)
                            {
                                // update database now
                                foreach (var item in items)
                                {
                                    item.MeterProcessStatus = true;
                                    await _mongodbService.UpdateAsync(item.Id, item);

                                }
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                //sleep 1hr
                for (int x = 0; x < 12; x++)
                {
                    _logger.LogInformation($"Sleep another 5 min");
                    await Task.Delay(300000, stoppingToken);
                    
                }

            }
        }
    
        public async Task<string> getMsiToken()
        {
            Console.WriteLine("Getting Token from MSI");
            var resource = "20e940b3-4c77-4b0b-9a53-9e16a1b010a7";
            var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");

            Console.WriteLine($"Current clientId: {clientId}");
            var token = "";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Metadata", "true");
                var url = $"http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&client_id={clientId}&resource={resource}";
                Console.WriteLine(url);
                using HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsonResponse);

                token = JsonConvert.DeserializeObject<TokenDefinition>(jsonResponse).Access_token;

            }
            catch(Exception e)
            {
                Console.WriteLine($"Error during calling MSI point {e.Message}");
                
            }
            return token;
        } 

    }
}
