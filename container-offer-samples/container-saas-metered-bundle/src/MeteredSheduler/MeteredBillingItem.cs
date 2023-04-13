using System;
using System.Collections.Generic;
using System.Text;

namespace MeteredSheduler
{
    public class MeteredBillingItem
    {
        public string ResourceId { get; set; }
        public int Quantity { get; set; }
        public string Dimension { get; set; }
        public string EffectiveStartTime { get; set; }
        public string PlanId { get; set; }
    }
}
