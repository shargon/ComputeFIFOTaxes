using Newtonsoft.Json;

namespace ComputeFIFOTaxes.Types
{
    public class SellTrade : Trade
    {
        /// <summary>
        /// Price
        /// </summary>
        [JsonIgnore]
        public override decimal Price => To.Value / From.Value;
    }
}