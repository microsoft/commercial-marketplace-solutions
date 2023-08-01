using ManagedWebhook.Definitions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Collections.Generic;
using System.IO;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;
using Microsoft.AspNetCore.Mvc;

namespace ManagedWebhook
{
    public static class Webhook
    {
        [FunctionName("Webhook")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "resource")] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            
            var config = new ConfigurationBuilder()
             .SetBasePath(context.FunctionAppDirectory)
             .AddEnvironmentVariables()
             .Build();

            //read Request body
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var usageRequest = JsonConvert.DeserializeObject<UsageEventDefinitionDto>(requestBody);

            using (var armHttpClient = HttpClientFactory.Create())
            {
                var armToken = await Webhook.GetToken(config, armHttpClient, log, "https://management.core.windows.net/").ConfigureAwait(continueOnCapturedContext: false);
                armHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {armToken}");

                var applicationResourceId = await Webhook.GetResourceGroupManagedBy(config, armHttpClient, log).ConfigureAwait(continueOnCapturedContext: false);

                        
                        if((usageRequest.Quantity == null)||(usageRequest.Quantity<=0))
                            return new BadRequestObjectResult("Please pass a quantity in the request body");


                        if(String.IsNullOrEmpty(usageRequest.Dimension))
                            return new BadRequestObjectResult("Please pass a dimension the request body");

                if (String.IsNullOrEmpty(usageRequest.PlanId))
                    return new BadRequestObjectResult("Please pass a PlanID the request body");



                usageRequest.EffectiveStartTime=DateTime.UtcNow;

                        //log.LogInformation($"Authorization bearer token: {armToken}");
                        log.LogInformation($"Resource usage id: {applicationResourceId}");
                        log.LogInformation($"Plan name: {usageRequest.PlanId}");
                        log.LogInformation($"Dimension: {usageRequest.Dimension}");
                        log.LogInformation($"Quantity: {usageRequest.Quantity}");


                        var response = await Webhook.EmitUsageEvents(config, armHttpClient, usageRequest.Dimension,usageRequest.Quantity,usageRequest.EffectiveStartTime, applicationResourceId, usageRequest.PlanId).ConfigureAwait(continueOnCapturedContext: false);
                        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: false);
                        if (response.IsSuccessStatusCode)
                        {
                                log.LogTrace($"Successfully emitted a usage event. Reponse body: {responseBody}");
                                return new OkObjectResult($"Successfully emitted a usage event. Reponse body: {responseBody}");
                        }
                        else
                        {
                                log.LogError($"Failed to emit a usage event. Error code: {response.StatusCode}. Failure cause: {response.ReasonPhrase}. Response body: {responseBody}");
                                return new BadRequestObjectResult($"Failed to emit a usage event. Error code: {response.StatusCode}. Failure cause: {response.ReasonPhrase}. Response body: {responseBody}");
                        }

            }
        }

        /// <summary>
        /// Gets the token for the system-assigned managed identity.
        /// </summary>
        private static async Task<string> GetToken(IConfigurationRoot config, HttpClient httpClient, ILogger log, string resource)
        {
            if (Webhook.IsLocalRun(config))
            {
                return "token";
            }

            // TOKEN_RESOURCE come from the configs
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{config["MSI_ENDPOINT"]}/?resource={resource}&api-version=2017-09-01"))
            {
                request.Headers.Add("Secret", config["MSI_SECRET"]);
                var response = await httpClient.SendAsync(request).ConfigureAwait(continueOnCapturedContext: false);
                if (response?.IsSuccessStatusCode != true)
                {
                    log.LogError($"Failed to get token for system-assigned MSI. Please check that the MSI is set up properly. Error: {response.Content.ReadAsStringAsync().Result}");
                }
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: false);
                return JsonConvert.DeserializeObject<TokenDefinition>(responseBody).Access_token;
            }
        }

        /// <summary>
        /// Gets the resource group managed by property.
        /// </summary>
        private static async Task<string> GetResourceGroupManagedBy(IConfigurationRoot config, HttpClient httpClient, ILogger log)
        {
            var resourceGroupResponse = await httpClient.GetAsync($"https://management.azure.com{config["RESOURCEGROUP_ID"]}?api-version=2019-11-01").ConfigureAwait(continueOnCapturedContext: false);
            if (resourceGroupResponse?.IsSuccessStatusCode != true)
            {
                log.LogError($"Failed to get the resource group from ARM. Error: {resourceGroupResponse.Content.ReadAsStringAsync().Result}");
                return null;
            }

            var resourceGroup = await resourceGroupResponse.Content.ReadAsAsync<ResourceGroupDefinition>().ConfigureAwait(continueOnCapturedContext: false);

            if (string.IsNullOrEmpty(resourceGroup?.ManagedBy))
            {
                log.LogError("The managedBy property either empty or missing for resource group.");
            }

            return resourceGroup?.ManagedBy;
        }

  

        /// <summary>
        /// Emits the usage event to the configured MARKETPLACEAPI_URI.
        /// </summary>
        private static async Task<HttpResponseMessage> EmitUsageEvents(IConfigurationRoot config, HttpClient httpClient, string dimension,double quantity,DateTime  effectiveStartTime,string resourceUri, string planId)
        {
            var usageEvent = new UsageEventDefinition
            {
                ResourceUri = resourceUri,
                Quantity = quantity,
                Dimension = dimension,
                EffectiveStartTime = effectiveStartTime,
                PlanId = planId
            };

            if (Webhook.IsLocalRun(config))
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(usageEvent), UnicodeEncoding.UTF8, "application/json"),
                    StatusCode = HttpStatusCode.OK
                };
            }
            return await httpClient.PostAsJsonAsync(config["MARKETPLACEAPI_URI"], usageEvent).ConfigureAwait(continueOnCapturedContext: false);
        }

        /// <summary>
        /// Returns whether the function is run locally.
        /// </summary>
        private static bool IsLocalRun(IConfigurationRoot config)
        {
            return config["LOCAL_RUN"]?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}
