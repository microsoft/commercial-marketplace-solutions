using ContainerCustomMetered.Entities;
using ContainerCustomMetered.Models;
using ContainerCustomMetered.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ContainerCustomMetered.Controllers
{
    public class TextSummary : Controller
    {
        public IActionResult Index()
        {
            Console.WriteLine($"Text Loaded");
            AzureTextViewModel view = new AzureTextViewModel();
            view.TextSummary = "";
            return View(view);
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(string textDetail)
        {
            Console.WriteLine($"Text Started");
            AzureTextViewModel view = new AzureTextViewModel();
            // Create a client
            view.TextSummary = await MakeSummaryRequest(textDetail);


            return this.View(view);

        }

        public async Task<string> MakeSummaryRequest(string detail)
        {
            var subscriptionKey = Environment.GetEnvironmentVariable("COGNITIVE_KEY");
            var textEndPoint = Environment.GetEnvironmentVariable("COGNITIVE_ENDPOINT");
            
            Console.WriteLine($"visionEndPoint {textEndPoint}");
            Console.WriteLine($"subscriptionKey {subscriptionKey}");

            if (!String.IsNullOrEmpty(subscriptionKey)
                && !String.IsNullOrEmpty(textEndPoint))
            {
                try
                {
                    HttpClient client = new HttpClient();
                    string uriBase = textEndPoint + "language/:analyze-text?api-version=2022-05-01";
                     Console.WriteLine($"uriBase {uriBase}");
                    // Request headers.
                    client.DefaultRequestHeaders.Add(
                        "Ocp-Apim-Subscription-Key", subscriptionKey);

                    string body="{\"kind\": \"KeyPhraseExtraction\",\"parameters\": {\"modelVersion\": \"latest\"},\"analysisInput\":{\"documents\":[{\"id\":\"1\",\"language\":\"en\",\"text\": \""+detail+"\"}]}}";
                    Console.WriteLine($"body {body}");

                    var content = new StringContent(body, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(uriBase, content);

                    // Asynchronously get the JSON response.
                    string contentString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"contentString {contentString}");
                    // Display the JSON response.
                    string result = JToken.Parse(contentString).ToString();



                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
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
             Console.WriteLine($"RecordUsage Started");

            try
            {

                Item item = new Item();
                // Add to DB direct using Repository Pattern
                //item.Id = Guid.NewGuid().ToString();
                item.DimensionId = "text";
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
