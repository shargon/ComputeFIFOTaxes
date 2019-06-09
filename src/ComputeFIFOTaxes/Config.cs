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

        public class FiatProviderConfig
        {
            public ECoin FiatCoin { get; set; }
            public string ApiKey { get; set; }
        }

        public SheetConfig SpreadSheet { get; set; }
        public FiatProviderConfig FiatProvider { get; set; }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}