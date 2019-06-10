using ComputeFIFOTaxes.Helpers;
using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using Newtonsoft.Json.Linq;
using System;

namespace ComputeFIFOTaxes.Providers
{
    /// <summary>
    /// https://coinlayer.com/documentation
    /// http://api.coinlayer.com/2018-04-30?access_key=376b5e6614ba3f732608946ae69aebe8&symbols=ETH&target=EUR
    /// </summary>
    public class CoinLayerDotComPriceProvider : FiatProviderBase
    {
        class Entry
        {
            public decimal Rate { get; set; }
            public decimal High { get; set; }
            public decimal Low { get; set; }
        }

        /// <summary>
        /// Api Key
        /// </summary>
        public string ApiKey { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Config</param>
        public CoinLayerDotComPriceProvider(Config.FiatProviderConfig config) : base(config.FiatCoin)
        {
            ApiKey = config.ApiKey;
        }

        /// <summary>
        /// Get fiat price for one coin in specific date
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="coin">Coin</param>
        /// <param name="date">Date</param>
        /// <returns>Price</returns>
        protected override decimal InternalGetFiatPrice(ITradeParser parser, ECoin coin, DateTime date)
        {
            var ret = DownloadHelper.Download<JObject>
                (
                $"http://api.coinlayer.com/{date.ToString("yyyy-MM-dd")}?access_key={ApiKey}&symbols={GetCoin(coin)}&target={GetCoin(Coin)}&expand=1"
                );

            if (!ret.TryGetValue("success", out var sucess) || 
                !sucess.Value<bool>() || 
                !ret.TryGetValue("rates", out var rates))
            {
                throw new ArgumentException();
            }

            var val = ((JProperty)rates.First).Value.ToObject<Entry>();

            return val.Rate;
        }

        /// <summary>
        /// Get coin
        /// </summary>
        /// <param name="coin">Coin</param>
        /// <returns>Coin</returns>
        private string GetCoin(ECoin coin)
        {
            switch (coin)
            {
                case ECoin.EUR:
                case ECoin.USD:
                case ECoin.USDT:
                case ECoin.BNB:
                case ECoin.BTC:
                case ECoin.ETH: return coin.ToString();
            }

            throw new ArgumentException(nameof(coin));
        }
    }
}