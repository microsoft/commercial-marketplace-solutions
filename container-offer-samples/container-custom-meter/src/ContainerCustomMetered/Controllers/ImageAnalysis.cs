using Microsoft.AspNetCore.Mvc;
using ContainerCustomMetered.Models;
using ContainerCustomMetered.Entities;
using ContainerCustomMetered.Services;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


using System.Text;

namespace ContainerCustomMetered.Controllers
{
    public class ImageAnalysis : Controller
    {

        public IActionResult Index()
        {

            Console.WriteLine($"Enter Image Analysis");
            AzureImageViewModel view = new AzureImageViewModel();
            view.Detail = "";
            return View(view);
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(string imageUrl)
        {
            Console.WriteLine($"Start Image Analysis");
            AzureImageViewModel view = new AzureImageViewModel();
            // Create a client
            view.Detail = await MakeAnalysisRequest(imageUrl);


            return this.View(view);

        }

 

        public async Task<string> MakeAnalysisRequest(string imageUrl)
        {
            var subscriptionKey = Environment.GetEnvironmentVariable("COGNITIVE_KEY");

            Console.WriteLine($"Sub Key {subscriptionKey}");

            var visionEndPoint = Environment.GetEnvironmentVariable("COGNITIVE_ENDPOINT");
            Console.WriteLine($"visionEndPoint {visionEndPoint}");

            if (!String.IsNullOrEmpty(subscriptionKey)
                && !String.IsNullOrEmpty(visionEndPoint))
            {
                try
                {
                    HttpClient client = new HttpClient();
                    string uriBase = visionEndPoint + "computervision/imageanalysis:analyze";
                    // Request headers.
                    client.DefaultRequestHeaders.Add(
                        "Ocp-Apim-Subscription-Key", subscriptionKey);

                    string requestParameters = "api-version=2023-02-01-preview&features=Tags&language=en";

                    // Assemble the URI for the REST API method.
                    string uri = uriBase + "?" + requestParameters;
                    Console.WriteLine($"Image URL {uri}");

                    var content = new StringContent("{\"url\":\"" + imageUrl + "\"}", Encoding.UTF8, "application/json");
                    Console.WriteLine($"Image content {content}");

                    HttpResponseMessage response=await client.PostAsync(uri, content);

                    // Asynchronously get the JSON response.
                    string contentString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"response {contentString}");

                    // Display the JSON response.
                    string result = JToken.Parse(contentString).ToString();

                    if (response.StatusCode==System.Net.HttpStatusCode.OK)
                    {
                        RecordUsage();
                    }
                    
                    return result;
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            else
            {
                return "Missing OCR Configuration Information";
            }
        }

        public async void RecordUsage()
        {
            Console.WriteLine($"RecordUsage start");
            try
            {

                Item item = new Item();
                // Add to DB direct using Repository Pattern
                //item.Id = Guid.NewGuid().ToString().Replace("-","");
                item.DimensionId = "image";
                item.Count = 1;
                item.MeterProcessStatus = false;
                item.CreatedDate = DateTime.Now;
                var _mongodbService = new MongoDbService();
                await _mongodbService.CreateAsync(item);

            }
            catch (Exception)
            {
                throw;
            }
            Console.WriteLine($"RecordUsage Completed");
        }
        
    }
}
