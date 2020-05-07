using Newtonsoft.Json;

namespace ComputeFIFOTaxes.Types
{
    public class YearProfit
    {
        /// <summary>
        /// Year
        /// </summary>
        [JsonIgnore]
        public int Year { get; set; } = 0;

        /// <summary>
        /// Total buy
        /// </summary>
        public decimal Buyed { get; set; } = 0;

        /// <summary>
        /// Total sold
        /// </summary>
        public decimal Sold { get; set; } = 0;

        /// <summary>
        /// Profit
        /// </summary>
        public decimal Profit { get; set; } = 0M;

        /// <summary>
        /// Fee
        /// </summary>
        public decimal Fee { get; set; } = 0M;

        /// <summary>
        /// Total
        /// </summary>
        public decimal Total => Profit - Fee;

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
