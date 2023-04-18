using System;
using System.Collections.Generic;
using System.Text;

namespace SaaS.Metered.Processing.Model
{
    public class DistinctItem
    {
        public string SubscriptionId { get; set; }
        public string DimensionId { get; set; }

        public string PlanId { get; set; }
    }

}
