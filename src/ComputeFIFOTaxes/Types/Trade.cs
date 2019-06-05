using Newtonsoft.Json;
using System;

namespace ComputeFIFOTaxes.Types
{
    public class Trade
    {
        /// <summary>
        /// Exchange
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Exchange { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

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
        public Quantity[] Fees { get; set; }

        /// <summary>
        /// Total cost (without fees)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Decimal? FiatCostWithoutFees { get; set; }

        /// <summary>
        /// Total fees
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Decimal? FiatFees { get; set; }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}