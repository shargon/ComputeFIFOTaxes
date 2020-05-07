using ComputeFIFOTaxes.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace ComputeFIFOTaxes.Types
{
    public abstract class Trade
    {
        /// <summary>
        /// Type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ETradeType Type { get; }

        /// <summary>
        /// Date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Exchange
        /// </summary>
        [JsonIgnore]
        internal ITradeParser Exchange { get; set; }

        /// <summary>
        /// From
        /// </summary>
        public Quantity From { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        [JsonIgnore]
        public abstract decimal Price { get; }

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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        protected Trade(ETradeType type)
        {
            Type = type;
        }

        /// <summary>
        /// From or to is
        /// </summary>
        /// <param name="coin">Coin</param>
        /// <returns></returns>
        public bool FromOrToIs(ECoin coin)
        {
            return From.Coin == coin || To.Coin == coin;
        }
    }
}
