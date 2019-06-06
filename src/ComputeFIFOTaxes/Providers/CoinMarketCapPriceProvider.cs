using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
        /// Counter for api
        /// </summary>
        private long _counter = 0;

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

            // Check same coin

            if (coin == Coin)
            {
                return new FiatPrice() { Average = 1, Max = 1, Min = 1 };
            }

            // Add to cache

            var ret = InternalGetFiatPrice(coin, date);
            ret.Wait();

            data[date] = ret.Result;
            File.WriteAllText(CacheFile, JsonConvert.SerializeObject(_cache, Formatting.Indented));

            return ret.Result;
        }

        /// <summary>
        /// Get fiat price for one coin in specific date
        /// </summary>
        /// <param name="coin">Coin</param>
        /// <param name="date">Date</param>
        /// <returns>Price</returns>
        private async Task<FiatPrice> InternalGetFiatPrice(ECoin coin, DateTime date)
        {
            // https://coinmarketcap.com/api/documentation/v1/#operation/getV1CryptocurrencyOhlcvHistorical

            //string pageContents;

            //using (HttpClient client = new HttpClient())
            //{
            //    var url = new UriBuilder("https://pro-api.coinmarketcap.com/v1/cryptocurrency/ohlcv/historical");

            //    var queryString = HttpUtility.ParseQueryString(string.Empty);
            //    queryString["id"] = (++_counter).ToString();
            //    queryString["time_start"] = "2019-01-01";
            //    queryString["time_end"] = "2019-01-03";
            //    queryString["symbol"] = GetSymbol(coin);
            //    queryString["convert"] = GetSymbol(Coin);

            //    client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", _config.ApiKey);
            //    client.DefaultRequestHeaders.Add("Accept", "application/json");

            //    var response = await client.GetAsync(url.ToString());
            //    pageContents = await response.Content.ReadAsStringAsync();
            //}

            //var json = JsonConvert.DeserializeObject(pageContents);

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
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}