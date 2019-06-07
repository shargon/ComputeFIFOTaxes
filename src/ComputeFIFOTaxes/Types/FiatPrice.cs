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
        /// Constructor
        /// </summary>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        public FiatPrice(decimal min, decimal max)
        {
            Min = min;
            Max = max;
            Average = (min + max) / 2M;
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}