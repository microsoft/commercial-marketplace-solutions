using System;
using System.Collections.Generic;
using System.Text;

namespace MeteredSheduler.Entities
{
    public class MeteredBillingItem
    {
        public string ResourceUri { get; set; }
        public decimal Quantity { get; set; }
        public string Dimension { get; set; }
        public string EffectiveStartTime { get; set; }
        public string PlanId { get; set; }
    }
}
