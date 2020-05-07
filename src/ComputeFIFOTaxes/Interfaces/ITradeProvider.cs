using ComputeFIFOTaxes.Types;
using System.Collections.Generic;

namespace ComputeFIFOTaxes.Interfaces
{
    public interface ITradeProvider
    {
        /// <summary>
        /// Get data
        /// </summary>
        /// <returns>Return data collection</returns>
        IEnumerable<TradeData> GetData();
    }
}
