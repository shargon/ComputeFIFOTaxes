using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ComputeFIFOTaxes.Types
{
    public class TradeData
    {
        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// Data
        /// </summary>
        public IList<object[]> Data { get; } = new List<object[]>();

        /// <summary>
        /// Parsed
        /// </summary>
        public bool Parsed { get; set; } = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public TradeData() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sheet">Sheet</param>
        /// <param name="data">Data</param>
        public TradeData(string sheet, IEnumerable<object[]> data)
        {
            Title = sheet ?? "";
            data.All(u => { Data.Add(u); return true; });
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}