using System;
using System.Collections.Generic;
using System.Text;

namespace SaaS.Metered.Processing.Model
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
