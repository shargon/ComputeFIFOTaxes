using ComputeFIFOTaxes.Types;
using System;

namespace ComputeFIFOTaxes.Interfaces
{
    public interface IFiatPriceProvider
    {
        /// <summary>
        /// Coint
        /// </summary>
        ECoin Coin { get; }

        /// <summary>
        /// Get fiat price for one coin in specific date
        /// </summary>
        /// <param name="coin">Coin</param>
        /// <param name="date">Date</param>
        /// <returns>Price</returns>
        FiatPrice GetFiatPrice(ECoin coin, DateTime date);
    }
}