using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Marketplace.Metering;
using Microsoft.Marketplace.Metering.Models;
using Microsoft.Azure.Cosmos;
using SaaS.Metered.Processing;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using SaaS.Metered.Processing.Model;
using SaaS.Metered.Processing.Services;

namespace SaaS.Metering.Processing
{
    public static class SaaSMeteredBilling
    {
        private static CosmosClient client = new CosmosClient(GetEnvironmentVariable("CosmosDb_Uri"), GetEnvironmentVariable("CosmosDb_Key"));
        private static CosmosDbService cosmosDbService;
        private static readonly IMarketplaceMeteringClient meteringClient;
        [FunctionName("SaaSMeteredBilling")]
        // run every hour
        public static async Task RunAsync([TimerTrigger("0 0 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //1-  Call database and get unique subs
            var containerName = GetEnvironmentVariable("CosmosDb_Collection");
            var databaseName = GetEnvironmentVariable("CosmosDb_Database");
            var sqlQueryText = "SELECT distinct c.SubscriptionId,c.PlanId, c.DimensionId FROM c where c.MeterProcessStatus = false";
            log.LogInformation("Running query: {0}\n", sqlQueryText);

            cosmosDbService = new CosmosDbService(client, databaseName, containerName);

            var distinctItems = await cosmosDbService.GetMultipleDistinctAsync(sqlQueryText);
            var subscription = distinctItems.Select(s => s.SubscriptionId).Distinct();
            foreach (string sub in subscription)
                await ProcesSubscriptionPlans(sub, distinctItems,log);
            
        }

        public static async Task ProcesSubscriptionPlans(string subscriptionId, IEnumerable<DistinctItem> distinctItems, ILogger log)
        {
             log.LogInformation("get all plans for this subscription {0}\n", subscriptionId);
            // get all plans for this subscription
            var plans = distinctItems.Where(p => p.SubscriptionId == subscriptionId).ToList();

            // get unique plan
            var distinctPlan = plans.Select(p => p.PlanId).Distinct();

            foreach (string plan in distinctPlan)
            {
                log.LogInformation("Process plan {0} for this subscription {1}\n",plan,subscriptionId);
                await ProcessPlanDimensions(subscriptionId, plan, distinctItems, log);
            }


        }


        public static async Task ProcessPlanDimensions(string subscriptionId, string planId, IEnumerable<DistinctItem> distinctItems, ILogger log)
        {
            log.LogInformation("Get all Dimension for this subscription and plan");
            var dimensions = distinctItems.Where(p => p.SubscriptionId == subscriptionId && p.PlanId==planId).ToList();

            // get dimension 
            log.LogInformation("get distinct Dimensions \n");
            var distinctDimensions = dimensions.Select(d => d.DimensionId).Distinct();

            foreach (string dimension in distinctDimensions)
            {
                log.LogInformation("Process Dim {0} for plan {1} for  subscription {2}\n", dimension, planId, subscriptionId);
                await ProcessMeteredDimension(subscriptionId, planId, dimension, log);
            }
        }


        public static async Task ProcessMeteredDimension(string subscriptionId, string planId, string dimensionId, ILogger log)
        {
            
            try
            {
               
                var sqlQueryText = String.Format("select * FROM c where c.MeterProcessStatus = false and c.SubscriptionId='{0}' and c.PlanId='{1}' and c.DimensionId='{2}'", subscriptionId, planId, dimensionId);
                var items = await cosmosDbService.GetMultipleAsync(sqlQueryText);
               var total = items.Sum(x => x.Quantity);

                log.LogInformation("Total for Dim {0} is {1}\n",dimensionId,total.ToString());

                // Cal Meter API

                log.LogInformation("Call Metered API");
                UsageEventOkResponse updateResult;
                var creds = AuthHelper.GetCreds();
                var meteringClient = new MarketplaceMeteringClient(creds);
                
                var usage = new UsageEvent()
                {
                    ResourceId = new Guid(subscriptionId),
                    PlanId =  planId,
                    Dimension = dimensionId,
                    Quantity = total,
                    EffectiveStartTime = DateTime.UtcNow 
                };

                try
                {
                    log.LogInformation("Invoke Metered API");
                    updateResult = (await meteringClient.Metering.PostUsageEventAsync(usage)).Value;
                    
                }
                catch (Exception)
                {
                    throw ;
                    
                }
                //if (result.StatusCode == System.Net.HttpStatusCode.OK || result.StatusCode == System.Net.HttpStatusCode.Created)
                if (updateResult.Status==UsageEventStatusEnum.Accepted)
                {
                    log.LogInformation("Metered Call is Accepted");
                    // Update Cosmos
                    foreach (Item item in items)
                    {
                        item.MeterProcessStatus = true;
                        await cosmosDbService.UpdateAsync(item.id,item);
                        log.LogInformation("Updated DB");
                    }
                }
                else
                {
                    // what to do now?
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }


        public static string GetEnvironmentVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName);
        }


    }





}
