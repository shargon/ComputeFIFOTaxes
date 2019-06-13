using System;
using System.IO;
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
            public decimal UsdPerEur { get; set; }
        }

        public SheetConfig SpreadSheet { get; set; }
        public FiatProviderConfig FiatProvider { get; set; }

        /// <summary>
        /// Parse config from file
        /// </summary>
        /// <param name="file">File</param>
        /// <returns>Config</returns>
        public static Config FromFile(string file)
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file, JsonConvert.SerializeObject(new Config(), Formatting.Indented));
            }

            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(file));
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}