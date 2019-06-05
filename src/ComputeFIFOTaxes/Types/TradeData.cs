using Newtonsoft.Json;
using System.Collections.Generic;

namespace ComputeFIFOTaxes.Types
{
    public class TradeData
    {
        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        public List<object[]> Data { get; set; }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}