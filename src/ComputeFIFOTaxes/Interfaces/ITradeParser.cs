using ComputeFIFOTaxes.Types;
using System.Collections.Generic;

namespace ComputeFIFOTaxes.Interfaces
{
    public interface ITradeParser
    {
        /// <summary>
        /// Parser name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Is this provider
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return true if is this provider</returns>
        bool IsThis(TradeData data);

        /// <summary>
        /// Get trades
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <param name="current">Current</param>
        /// <returns>Return Trades</returns>
        IEnumerable<Trade> GetTrades(TradeDataSource dataSource, TradeData current);
    }
}
