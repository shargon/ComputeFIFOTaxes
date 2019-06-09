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
        public FiatPrice(decimal min, decimal max) : this(min, max, (min + max) / 2) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <param name="average">Average</param>
        public FiatPrice(decimal min, decimal max, decimal average)
        {
            Min = min;
            Max = max;
            Average = average;
        }

        /// <summary>
        /// Plus with this price
        /// </summary>
        /// <param name="price">Price</param>
        public FiatPrice Plus(decimal price)
        {
            return new FiatPrice(Min * price, Max * price, Average * price);
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}