using System;
using System.Text.Json.Serialization;
namespace LandingPage.Entities
{
    public class Item
    {
        public string id { get; set; }
        public string SubscriptionId { get; set; }
        public string PlanId { get; set; }
        public string DimensionId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string OcrData { get; set; }
        public int OcrDataCount { get; set; }
        public bool MeterProcessStatus { get; set; }
    }
}
