using ComputeFIFOTaxes.Helpers;
using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Parsers;
using ComputeFIFOTaxes.Providers;
using ComputeFIFOTaxes.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleToAttribute("ComputeFIFOTaxes.Tests")]

namespace ComputeFIFOTaxes
{
    class Program
    {
        static void Main()
        {
            if (!File.Exists("config.json"))
            {
                File.WriteAllText("config.json", JsonConvert.SerializeObject(new Config(), Formatting.Indented));
            }

            var trades = new List<Trade>();
            var cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            var provider = new GoogleSheetsProvider(cfg);
            var parsers = new ITradeParser[]
            {
                new KrakenLedgerParser(), // Kraken Ledfer before Trades ALWAYS
                new KrakenTradesParser(),
                new BinanceParser()
            };
            var priceProvider = new CoinLayerDotComPriceProvider(cfg.FiatProvider);

            // Parser trades

            var dataSource = new TradeDataSource() { Data = provider.GetData().ToArray() };

            foreach (var parser in parsers)
            {
                foreach (var data in dataSource.Data)
                {
                    if (data.Parsed || !parser.IsThis(data)) continue;

                    trades.AddRange(parser.GetTrades(dataSource, data));
                    data.Parsed = true;
                }
            }

            // Sort trades by date

            trades.SortByDate();

            // Compute fiat values

            trades.ComputeFiatValues(priceProvider);

            // Compute fifo

            var totalFees = trades.Select(u => u.FiatFees.Value).Sum();

            Console.WriteLine("TotalFees: " + totalFees.ToString(CultureInfo.InvariantCulture) + " " + priceProvider.Coin.ToString());

            
        }
    }
}