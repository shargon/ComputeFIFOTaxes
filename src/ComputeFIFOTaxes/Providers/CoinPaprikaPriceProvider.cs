using ComputeFIFOTaxes.Helpers;
using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ComputeFIFOTaxes.Providers
{
    /// <summary>
    /// https://api.coinpaprika.com/#tag/Tickers/paths/~1tickers~1{coin_id}/get
    /// </summary>
    public class CoinPaprikaPriceProvider : FiatProviderBase
    {
        private class CoinInfo
        {
            public string Id { get; set; }
            public string Symbol { get; set; }
            public int Rank { get; set; }
            public string Type { get; set; }
        }

        private class ExchageInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public bool Active { get; set; }
        }

        private class TickInfo
        {
            public DateTime Date => DateTime.ParseExact(Timestamp, "yyyy-MM-dd'T'HH:mm:ss'Z'", DateTimeFormatInfo.InvariantInfo);

            /// <summary>
            /// 2017-05-04T14:00:00Z
            /// </summary>
            public string Timestamp { get; set; }
            public decimal Price { get; set; }
        }

        private readonly DateTime UnixTimestamp = new DateTime(1970, 1, 1);

        private readonly static IDictionary<string, string> _exchangeCache = new Dictionary<string, string>();
        private readonly static IDictionary<string, string> _coinCache = new Dictionary<string, string>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Config</param>
        public CoinPaprikaPriceProvider(Config.FiatProviderConfig config) : base(config) { }

        protected override decimal InternalGetFiatPrice(ITradeParser parser, ECoin coin, DateTime date)
        {
            if (coin == ECoin.USD)
            {
                return FiatProviderHelper.UsdPerCoin(Coin, date);
            }

            if (_coinCache.Count == 0)
            {
                // Fill cache

                var coins = DownloadHelper.Download<CoinInfo[]>("https://api.coinpaprika.com/v1/coins", false);

                foreach (var c in coins.Where(u => u.Rank > 0).OrderBy(u => u.Rank))
                {
                    if (!_coinCache.ContainsKey(c.Symbol))
                    {
                        _coinCache[c.Symbol] = c.Id;
                    }
                }

                var exchages = DownloadHelper.Download<ExchageInfo[]>("https://api.coinpaprika.com/v1/exchanges", false);

                foreach (var exchage in exchages.OrderBy(u => u.Active))
                {
                    if (!_exchangeCache.ContainsKey(exchage.Name))
                    {
                        _exchangeCache[exchage.Name] = exchage.Id;
                    }
                }
            }

            var preparedDate = date.AddSeconds(-date.Second);
            preparedDate = preparedDate.AddMinutes(-5);

            var ticks = DownloadHelper.Download<TickInfo[]>(
                $"https://api.coinpaprika.com/v1/tickers/{GetCoinId(coin)}/historical?start={PrepareDate(preparedDate)}&limit=10&quote=usd&interval=5m");

            var usdPrice = 0M;

            foreach (var tick in ticks.OrderBy(u => u.Date))
            {
                if (tick.Date > date)
                {
                    break;
                }

                usdPrice = tick.Price;
            }

            return UsdToPrice(usdPrice, date);
        }

        /// <summary>
        /// Convert usd to the fiat price
        /// </summary>
        /// <param name="usdValue">Usd value</param>
        /// <param name="date">Date</param>
        /// <returns>Value</returns>
        private decimal UsdToPrice(decimal usdValue, DateTime date)
        {
            switch (Coin)
            {
                case ECoin.USD: return usdValue;

                default: return usdValue * GetFiatPrice(null, ECoin.USD, date);
            }
        }

        /// <summary>
        /// Prepare date
        /// </summary>
        /// <param name="date">Date</param>
        /// <returns>RFC3999</returns>
        private int PrepareDate(DateTime date) => (int)(date.Subtract(UnixTimestamp).TotalSeconds);

        /// <summary>
        /// Get coin id
        /// </summary>
        /// <param name="coin">Coin</param>
        /// <returns>Return id</returns>
        private string GetCoinId(ECoin coin)
        {
            switch (coin)
            {
                case ECoin.BTC:
                case ECoin.ETH:
                case ECoin.BNB:
                case ECoin.USDT:
                    {
                        if (!_coinCache.TryGetValue(coin.ToString(), out var value))
                        {
                            throw new ArgumentException(nameof(coin));
                        }

                        return value;
                    }

                default: throw new ArgumentException(nameof(coin));
            }
        }
    }
}
