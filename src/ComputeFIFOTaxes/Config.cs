using ComputeFIFOTaxes.Types;
using Newtonsoft.Json;

namespace ComputeFIFOTaxes
{
    public class Config
    {
        public class SheetConfig
        {
            public string Id { get; set; }
        }

        public class CoinMarketCapConfig
        {
            public string ApiKey { get; set; }
            public ECoin FiatCoin { get; set; }
        }

        public SheetConfig SpreadSheet { get; set; }
        public CoinMarketCapConfig CoinMarketCap { get; set; }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}