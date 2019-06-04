using System;
using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;

namespace ComputeFIFOTaxes.Providers
{
    public class CoinMarketCapPriceProvider : IFiatPriceProvider
    {
        /// <summary>
        /// Fiat coin
        /// </summary>
        public ECoin Coin { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fiatCoin">Fiat coin</param>
        public CoinMarketCapPriceProvider(ECoin fiatCoin)
        {
            if (fiatCoin != ECoin.EUR || fiatCoin != ECoin.USD)
            {
                throw new ArgumentException(nameof(fiatCoin));
            }

            Coin = fiatCoin;
        }

        /// <summary>
        /// Get fiat price for one coin in specific date
        /// </summary>
        /// <param name="coin">Coin</param>
        /// <param name="date">Date</param>
        /// <returns>Price</returns>
        public Decimal GetFiatPrice(ECoin coin, DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}