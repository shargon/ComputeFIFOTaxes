using ComputeFIFOTaxes.Models.Converters;
using Newtonsoft.Json;

namespace ComputeFIFOTaxes.Models
{
    [JsonConverter(typeof(JArrayToObjectConverter))]
    public class KrakenOHLC
    {
        /// <summary>
        /// Unix timestamp.
        /// </summary>
        public long Time { get; set; }

        public decimal Open { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Close { get; set; }

        /// <summary>
        /// Volume-weighted average price.
        /// </summary>
        public decimal Vwap { get; set; }

        public decimal Volume { get; set; }

        public int Count { get; set; }
    }
}