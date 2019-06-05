using Newtonsoft.Json;
using System;

namespace ComputeFIFOTaxes.Types
{
    public class FiatPrice
    {
        /// <summary>
        /// Minimum
        /// </summary>
        public Decimal Min { get; set; } = 0;

        /// <summary>
        /// Maximum
        /// </summary>
        public Decimal Max { get; set; } = 0;

        /// <summary>
        /// Average
        /// </summary>
        public Decimal Average { get; set; } = 0;

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}