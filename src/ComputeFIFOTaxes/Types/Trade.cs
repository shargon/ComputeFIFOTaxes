using System;

namespace ComputeFIFOTaxes.Types
{
    public class Trade
    {
        /// <summary>
        /// Date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// From
        /// </summary>
        public Quantity From { get; set; }

        /// <summary>
        /// To
        /// </summary>
        public Quantity To { get; set; }

        /// <summary>
        /// Fee
        /// </summary>
        public Quantity Fee { get; set; }
    }
}