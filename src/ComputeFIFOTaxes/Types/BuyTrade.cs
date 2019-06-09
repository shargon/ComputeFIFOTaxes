using Newtonsoft.Json;

namespace ComputeFIFOTaxes.Types
{
    public class BuyTrade : Trade
    {
        /// <summary>
        /// Price
        /// </summary>
        [JsonIgnore]
        public override decimal Price => From.Value / To.Value;
    }
}