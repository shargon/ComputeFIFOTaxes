using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Parsers;
using ComputeFIFOTaxes.Providers;
using ComputeFIFOTaxes.Types;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ComputeFIFOTaxes
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists("config.json"))
            {
                File.WriteAllText("config.json", JsonConvert.SerializeObject(new Config(), Formatting.Indented));
            }

            var cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            var provider = new GoogleSheetsProvider(cfg);
            var trades = new List<Trade>();
            var fiatPriceProvider = new CoinMarketCapPriceProvider(ECoin.EUR);
            var parsers = new IParser[] { new KrakenParser(), new BinanceParser() };

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
                trade.From.FiatPrice = fiatPriceProvider.GetFiatPrice(trade.From.Coin, trade.Date) * trade.From.Value;
                trade.To.FiatPrice = fiatPriceProvider.GetFiatPrice(trade.To.Coin, trade.Date) * trade.To.Value;

                foreach (var fee in trade.Fees)
                {
                    fee.FiatPrice = fiatPriceProvider.GetFiatPrice(fee.Coin, trade.Date) * fee.Value;
                }
            }

            // Compute fifo


        }
    }
}