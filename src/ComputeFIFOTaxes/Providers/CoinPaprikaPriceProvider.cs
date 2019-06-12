using ComputeFIFOTaxes.Helpers;
using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ComputeFIFOTaxes.Providers
{
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

        public const decimal USD_PER_EUR = 0.88M;
        private readonly DateTime UnixTimestamp = new DateTime(1970, 1, 1);

        private readonly static IDictionary<string, string> _exchangeCache = new Dictionary<string, string>();
        private readonly static IDictionary<string, string> _coinCache = new Dictionary<string, string>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Config</param>
        public CoinPaprikaPriceProvider(Config.FiatProviderConfig config) : base(config.FiatCoin)
        {
            if (_coinCache.Count == 0)
            {
                var coins = DownloadHelper.Download<CoinInfo[]>("https://api.coinpaprika.com/v1/coins", false);

                foreach (var coin in coins.Where(u => u.Rank > 0).OrderBy(u => u.Rank))
                {
                    if (!_coinCache.ContainsKey(coin.Symbol))
                    {
                        _coinCache[coin.Symbol] = coin.Id;
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
        }

        protected override decimal InternalGetFiatPrice(ITradeParser parser, ECoin coin, DateTime date)
        {
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

            return UsdToPrice(usdPrice);
        }

        /// <summary>
        /// Convert usd to the fiat price
        /// </summary>
        /// <param name="usdValue">Usd value</param>
        /// <returns>Value</returns>
        private decimal UsdToPrice(decimal usdValue)
        {
            switch (Coin)
            {
                case ECoin.USD: return usdValue;
                case ECoin.EUR: return usdValue * USD_PER_EUR;

                default: throw new ArgumentException(nameof(CoinInfo));
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