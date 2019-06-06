using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ComputeFIFOTaxes.Types
{
    public class Quantity
    {
        /// <summary>
        /// Coin
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ECoin Coin { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}