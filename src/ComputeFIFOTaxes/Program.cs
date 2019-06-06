using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Parsers;
using ComputeFIFOTaxes.Providers;
using ComputeFIFOTaxes.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ComputeFIFOTaxes
{
    class Program
    {
        private static IFiatPriceProvider _priceProvider;

        static void Main(string[] args)
        {
            if (!File.Exists("config.json"))
            {
                File.WriteAllText("config.json", JsonConvert.SerializeObject(new Config(), Formatting.Indented));
            }

            var trades = new List<Trade>();
            var cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            var provider = new GoogleSheetsProvider(cfg);
            var parsers = new IParser[] { new KrakenParser(), new BinanceParser() };
            _priceProvider = new CoinMarketCapPriceProvider(cfg.CoinMarketCap);

            // Parser trades

            foreach (var data in provider.GetData())
            {
                foreach (var parser in parsers)
                {
                    if (!parser.IsThis(data)) continue;

                    trades.AddRange(parser.GetTrades(data));
                    break;
                }
            }

            // Sort trades by date

            trades.Sort((a, b) => a.Date.CompareTo(b.Date));

            // Get prices

            foreach (var trade in trades)
            {
                if (!trade.FiatCostWithoutFees.HasValue)
                {
                    trade.FiatCostWithoutFees = ChoosePriceForTrade(trade, trade.Date);
                }

                if (!trade.FiatFees.HasValue)
                {
                    trade.FiatFees = 0;

                    foreach (var fee in trade.Fees)
                    {
                        trade.FiatFees += ChoosePriceForFee(fee, trade.Date);
                    }
                }
            }

            // Compute fifo


        }

        /// <summary>
        /// Choose the best price for this trade
        /// </summary>
        /// <param name="trade">Trade</param>
        /// <param name="date">date</param>
        /// <returns>Decimal value</returns>
        private static Decimal ChoosePriceForFee(Quantity trade, DateTime date)
        {
            return _priceProvider.GetFiatPrice(trade.Coin, date).Max * trade.Value;
        }

        /// <summary>
        /// Choose the best price for this trade
        /// </summary>
        /// <param name="trade">Trade</param>
        /// <param name="date">date</param>
        /// <returns>Decimal value</returns>
        private static Decimal ChoosePriceForTrade(Trade trade, DateTime date)
        {
            if (trade.From.Coin == _priceProvider.Coin)
            {
                return trade.From.Value;
            }
            else if (trade.To.Coin == _priceProvider.Coin)
            {
                return trade.To.Value;
            }

            var price = _priceProvider.GetFiatPrice(trade.From.Coin, date);

            return (trade is SellTrade ? price.Min : price.Max) * trade.From.Value;
        }
    }
}