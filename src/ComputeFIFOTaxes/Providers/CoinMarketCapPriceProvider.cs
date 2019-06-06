using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ComputeFIFOTaxes.Providers
{
    public class CoinMarketCapPriceProvider : FiatProviderBase
    {
        /// <summary>
        /// Config
        /// </summary>
        private readonly Config.CoinMarketCapConfig _config;

        /// <summary>
        /// Counter for api
        /// </summary>
        private long _counter = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Config</param>
        public CoinMarketCapPriceProvider(Config.CoinMarketCapConfig config) : base(config.FiatCoin)
        {
            _config = config;
        }

        /// <summary>
        /// Get fiat price for one coin in specific date
        /// </summary>
        /// <param name="coin">Coin</param>
        /// <param name="date">Date</param>
        /// <returns>Price</returns>
        protected override async Task<FiatPrice> InternalGetFiatPrice(ECoin coin, DateTime date)
        {
            var id = Interlocked.Increment(ref _counter);

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
    }
}