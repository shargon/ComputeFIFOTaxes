using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using System.Collections.Generic;

namespace ComputeFIFOTaxes.Providers
{
    public class GoogleSheetsProvider : ITradeProvider
    {
        /// <summary>
        /// Sheet
        /// </summary>
        public string Sheet { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sheet">Sheet</param>
        public GoogleSheetsProvider(string sheet)
        {
            Sheet = sheet;
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <returns>Return data</returns>
        public IEnumerable<TradeData> GetData()
        {
            yield break;
        }
    }
}