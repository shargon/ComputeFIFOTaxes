using Newtonsoft.Json;

namespace ComputeFIFOTaxes.Types
{
    public class FiatPrice
    {
        /// <summary>
        /// Minimum
        /// </summary>
        public decimal Min { get; set; } = 0;

        /// <summary>
        /// Maximum
        /// </summary>
        public decimal Max { get; set; } = 0;

        /// <summary>
        /// Average
        /// </summary>
        public decimal Average { get; set; } = 0;

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}