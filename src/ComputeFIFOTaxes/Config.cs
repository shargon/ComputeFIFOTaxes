using ComputeFIFOTaxes.Types;

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
    }
}