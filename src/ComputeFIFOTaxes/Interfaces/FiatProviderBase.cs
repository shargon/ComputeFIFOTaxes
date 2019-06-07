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
        private readonly Dictionary<ECoin, Dictionary<DateTime, Dictionary<ECoin, FiatPrice>>> _cache;

        /// <summary>
        /// Cache file
        /// </summary>
        private readonly string CacheFile;

        /// <summary>
        /// Coint
        /// </summary>
        public ECoin Coin { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coin">Coin</param>
        public FiatProviderBase(ECoin coin)
        {
            Coin = coin;

            if (Coin != ECoin.EUR && Coin != ECoin.USD)
            {
                throw new ArgumentException(nameof(coin));
            }

            CacheFile = "marketCache.json";

            if (File.Exists(CacheFile))
            {
                // Load cache

                try
                {
                    var data = JsonConvert.DeserializeObject<Dictionary<ECoin, Dictionary<DateTime, Dictionary<ECoin, FiatPrice>>>>(File.ReadAllText(CacheFile));
                    if (data != null) _cache = data;

#if DEBUG
                    Debugger.Log(0, "CACHE", "Loaded " + data.Values.Sum(u => u.Values.Sum(x => x.Count)) + " entries");
#endif
                }
                catch { }
            }

            if (_cache == null)
            {
                _cache = new Dictionary<ECoin, Dictionary<DateTime, Dictionary<ECoin, FiatPrice>>>();
            }
        }

        /// <summary>
        /// Get fiat price for one coin in specific date
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="coin">Coin</param>
        /// <param name="date">Date</param>
        /// <returns>Price</returns>
        protected abstract FiatPrice InternalGetFiatPrice(IExchange parser, ECoin coin, DateTime date);

        /// <summary>
        /// Get fiat price for one coin in specific date
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="coin">Coin</param>
        /// <param name="date">Date</param>
        /// <returns>Price</returns>
        public FiatPrice GetFiatPrice(IExchange parser, ECoin coin, DateTime date)
        {
            // Check same coin

            if (coin == Coin)
            {
                return new FiatPrice(1, 1);
            }

            // Search inside the cache

            Dictionary<ECoin, FiatPrice> value;

            if (_cache.TryGetValue(coin, out var data))
            {
                if (!data.TryGetValue(date, out value))
                {
                    data[date] = value = new Dictionary<ECoin, FiatPrice>();
                }
            }
            else
            {
                data = _cache[coin] = new Dictionary<DateTime, Dictionary<ECoin, FiatPrice>>();
                data[date] = value = new Dictionary<ECoin, FiatPrice>();
            }

            if (value.TryGetValue(Coin, out var result) && result.Average != 0)
            {
                return result;
            }

            // Add to cache

            var ret = value[Coin] = InternalGetFiatPrice(parser, coin, date);

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