using System;

namespace ManagedWebhook.Definitions
{
    /// <summary>
    /// The usage event definition.
    /// </summary>
    class UsageEventDefinitionDto
    {

        /// <summary>
        /// The quantity of the usage.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Dimension identifier.
        /// </summary>
        public string Dimension { get; set; }

        /// <summary>
        /// Time in UTC when the usage event occurred.
        /// </summary>
        public DateTime EffectiveStartTime { get; set; }


        /// <summary>
        /// Result of emitting to marketplace
        /// </summary>
        public string PostResult { get; set; }
    }
}
