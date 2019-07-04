using ComputeFIFOTaxes.Helpers;
using ComputeFIFOTaxes.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
#endif
using System.IO;
using System.Linq;

namespace ComputeFIFOTaxes.Interfaces
{
    public abstract class FiatProviderBase
    {
        /// <summary>
        /// Cache
        /// </summary>
        private readonly Dictionary<ECoin, Dictionary<ECoin, Dictionary<DateTime, decimal>>> _cache;

        /// <summary>
        /// Cache file
        /// </summary>
        private readonly string CacheFile;

        /// <summary>
        /// Coin
        /// </summary>
        public ECoin Coin { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Config</param>
        public FiatProviderBase(Config.FiatProviderConfig config)
        {
            Coin = config.FiatCoin;

            if (Coin != ECoin.EUR && Coin != ECoin.USD)
            {
                throw new ArgumentException(nameof(Coin));
            }

            CacheFile = "marketCache.json";

            if (File.Exists(CacheFile))
            {
                // Load cache

                try
                {
                    var data = JsonConvert.DeserializeObject<Dictionary<ECoin, Dictionary<ECoin, Dictionary<DateTime, decimal>>>>(File.ReadAllText(CacheFile));
                    if (data != null) _cache = data;

#if DEBUG
                    Debugger.Log(0, "MarketCache", "Loaded " + data.Values.Sum(u => u.Values.Sum(x => x.Count)) + " entries");
#endif
                }
                catch { }
            }

            if (_cache == null)
            {
                _cache = new Dictionary<ECoin, Dictionary<ECoin, Dictionary<DateTime, decimal>>>();
            }
        }

        /// <summary>
        /// Get fiat price for one coin in specific date
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="coin">Coin</param>
        /// <param name="date">Date</param>
        /// <returns>Price</returns>
        protected abstract decimal InternalGetFiatPrice(ITradeParser parser, ECoin coin, DateTime date);

        /// <summary>
        /// Get fiat price for one coin in specific date
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="coin">Coin</param>
        /// <param name="date">Date</param>
        /// <returns>Price</returns>
        public decimal GetFiatPrice(ITradeParser parser, ECoin coin, DateTime date)
        {
            // Check same coin

            if (coin == Coin)
            {
                return 1;
            }

            if (coin == ECoin.KFEE)
            {
                // Kraken fees

                return 0;
            }

            if (!_cache.TryGetValue(coin, out var cache))
            {
                cache = _cache[coin] = new Dictionary<ECoin, Dictionary<DateTime, decimal>>();
            }

            // Search inside the cache

            if (!cache.TryGetValue(Coin, out var cacheEntry))
            {
                cacheEntry = cache[Coin] = new Dictionary<DateTime, decimal>();
            }

            if (cacheEntry.TryGetValue(date, out var result) && result != 0)
            {
                return result;
            }

            // Add to cache

            var ret = cacheEntry[date] = InternalGetFiatPrice(parser, coin, date);

            File.WriteAllText(CacheFile, JsonConvert.SerializeObject(_cache, Formatting.Indented));

            return ret;
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}