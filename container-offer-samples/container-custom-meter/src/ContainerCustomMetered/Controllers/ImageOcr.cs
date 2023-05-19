using Microsoft.AspNetCore.Mvc;
using ContainerCustomMetered.Models;
using ContainerCustomMetered.Entities;
using ContainerCustomMetered.ViewModels;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ContainerCustomMetered.Services;

namespace ContainerCustomMetered.Controllers
{
    public class ImageOcr : Controller
    {

        public IActionResult Index()
        {

            Console.WriteLine($"OCR Loaded");
            AzureImageViewModel view = new AzureImageViewModel();
            view.Detail = "";
            return View(view);
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(List<IFormFile> files)
        {
            Console.WriteLine($"OCR Started");
            AzureImageViewModel view = new AzureImageViewModel();

            var filePaths = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    // full path to file in temp location
                    using (var ms = new MemoryStream())
                    {
                        formFile.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        view.Detail = await MakeOCRRequest(fileBytes);

                        // act on the Base64 data
                    }
                }
            }
            return this.View(view);

        }

        public async Task<string> MakeOCRRequest(byte[] byteData)
        {
            var subscriptionKey = Environment.GetEnvironmentVariable("COGNITIVE_KEY");
            var visionEndPoint = Environment.GetEnvironmentVariable("COGNITIVE_ENDPOINT");

            Console.WriteLine($"visionEndPoint {visionEndPoint}");
            Console.WriteLine($"subscriptionKey {subscriptionKey}");

            if (!String.IsNullOrEmpty(subscriptionKey)
                && !String.IsNullOrEmpty(visionEndPoint))
            {
                try
                {
                    HttpClient client = new HttpClient();
                    string uriBase = visionEndPoint + "vision/v2.1/ocr";
                    // Request headers.
                    client.DefaultRequestHeaders.Add(
                        "Ocp-Apim-Subscription-Key", subscriptionKey);

                    string requestParameters = "language=unk&detectOrientation=true";

                    // Assemble the URI for the REST API method.
                    string uri = uriBase + "?" + requestParameters;
                    Console.WriteLine($"uri {uri}");
                    HttpResponseMessage response;

                    // Add the byte array as an octet stream to the request body.
                    using (ByteArrayContent content = new ByteArrayContent(byteData))
                    {
                        // This example uses the "application/octet-stream" content type.
                        // The other content types you can use are "application/json"
                        // and "multipart/form-data".
                        content.Headers.ContentType =
                            new MediaTypeHeaderValue("application/octet-stream");

                        // Asynchronously call the REST API method.
                        response = await client.PostAsync(uri, content);
                    }

                    // Asynchronously get the JSON response.
                    string contentString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"contentString {contentString}");
                    // Display the JSON response.
                    string result = JToken.Parse(contentString).ToString();

                    

                    AzureOcrModel ocrResult = JsonConvert.DeserializeObject<AzureOcrModel>(result);

                    List<string> ocrText = ocrResult.regions.SelectMany(r => r.lines.SelectMany(l => l.words).Select(w => w.text)).ToList();
                    //CallMeteredAudit(ocrText);

                    string finalText = ocrText.Aggregate("", (current, s) => current + (s + ","));

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        RecordUsage();
                    }


                    return finalText;
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
                item.DimensionId = "ocr";
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
