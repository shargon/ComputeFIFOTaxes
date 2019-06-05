using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ComputeFIFOTaxes.Providers
{
    public class CoinMarketCapPriceProvider : IFiatPriceProvider
    {
        /// <summary>
        /// Cache
        /// </summary>
        private Dictionary<ECoin, Dictionary<DateTime, FiatPrice>> _cache = new Dictionary<ECoin, Dictionary<DateTime, FiatPrice>>();

        /// <summary>
        /// Cache file
        /// </summary>
        private readonly string CacheFile;

        /// <summary>
        /// Config
        /// </summary>
        private readonly Config.CoinMarketCapConfig _config;

        /// <summary>
        /// Fiat coin
        /// </summary>
        public ECoin Coin { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Config</param>
        public CoinMarketCapPriceProvider(Config.CoinMarketCapConfig config)
        {
            if (config.FiatCoin != ECoin.EUR && config.FiatCoin != ECoin.USD)
            {
                throw new ArgumentException(nameof(config.FiatCoin));
            }

            _config = config;
            Coin = config.FiatCoin;
            CacheFile = "marketCache_" + config.FiatCoin.ToString() + ".json";

            if (File.Exists(CacheFile))
            {
                // Load cache

                var data = JsonConvert.DeserializeObject<Dictionary<ECoin, Dictionary<DateTime, FiatPrice>>>(File.ReadAllText(CacheFile));
                if (data != null) _cache = data;
            }
        }

        /// <summary>
        /// Get fiat price for one coin in specific date
        /// </summary>
        /// <param name="coin">Coin</param>
        /// <param name="date">Date</param>
        /// <returns>Price</returns>
        public FiatPrice GetFiatPrice(ECoin coin, DateTime date)
        {
            // Search inside the cache

            if (_cache.TryGetValue(coin, out var data))
            {
                if (data.TryGetValue(date, out var value)) return value;
            }
            else
            {
                data = _cache[coin] = new Dictionary<DateTime, FiatPrice>();
            }

            // Add to cache

            data[date] = InternalGetFiatPrice(coin, date);
            SaveCache();

            return data[date];
        }

        /// <summary>
        /// Get fiat price for one coin in specific date
        /// </summary>
        /// <param name="coin">Coin</param>
        /// <param name="date">Date</param>
        /// <returns>Price</returns>
        private FiatPrice InternalGetFiatPrice(ECoin coin, DateTime date)
        {
            // https://coinmarketcap.com/api/documentation/v1/#operation/getV1CryptocurrencyOhlcvHistorical

            var min = 0;
            var max = 0;
            var avg = 0;

            return new FiatPrice()
            {
                Average = avg,
                Min = min,
                Max = max,
            };
        }

        /// <summary>
        /// Get symbol
        /// </summary>
        /// <param name="coin">Coin</param>
        /// <returns>CoinMarketCap symbol</returns>
        private string GetSymbol(ECoin coin)
        {
            switch (coin)
            {
                default: return coin.ToString();
            }

            throw new ArgumentException(nameof(coin));
        }

        /// <summary>
        /// Save cache
        /// </summary>
        private void SaveCache()
        {
            File.WriteAllText(CacheFile, JsonConvert.SerializeObject(_cache, Formatting.Indented));
        }
    }
}