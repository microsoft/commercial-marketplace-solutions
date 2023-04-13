using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Marketplace.Metering;
using Microsoft.Marketplace.Metering.Models;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using SaaS.Metered.Processing;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Marketplace.SaaS;
using SaaS.Metered.Processing.Model;
using SaaS.Metered.Processing.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.IO;

namespace SaaS.Metering.Processing
{
    public static class SaaSMeteredRequest
    {
        private static CosmosClient client = new CosmosClient(GetEnvironmentVariable("CosmosDb_Uri"), GetEnvironmentVariable("CosmosDb_Key"));
        private static CosmosDbService cosmosDbService;
        [FunctionName("SaaSMeteredRequest")]
        public static async Task<IActionResult> Run( [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]  
        HttpRequest req, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            string requestBody = String.Empty;
            using (StreamReader streamReader = new StreamReader(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
            }
            MeteredBillingItem data = JsonConvert.DeserializeObject<MeteredBillingItem>(requestBody);
            await SendMeteredQty(data, log);
            return data != null
                ? (ActionResult)new OkObjectResult($"accepted")
                : new BadRequestObjectResult("Please pass valid request body");
        }

        public static async Task SendMeteredQty(MeteredBillingItem data, ILogger log)
        {
            log.LogInformation($"Save Item to DB");
            var containerName = GetEnvironmentVariable("CosmosDb_Collection");
            var databaseName = GetEnvironmentVariable("CosmosDb_Database");
            cosmosDbService = new CosmosDbService(client, databaseName, containerName);

                try
                {

                    Item item = new Item();
                    // Add to DB direct using Repository Pattern
                    item.id = Guid.NewGuid().ToString();
                    item.DimensionId = data.Dimension;
                    item.SubscriptionId = data.ResourceId;
                    item.PlanId = data.PlanId;
                    item.Quantity = data.Quantity;
                    item.MeterProcessStatus = false;
                    item.CreatedDate = DateTime.Now;
                    await cosmosDbService.AddAsync(item);

                log.LogInformation($"Completed saving Item to DB");
            }
                catch (Exception)
                {
                    throw;
                }
            
        }

        public static string GetEnvironmentVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName);
        }


    }





}
