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

            // Remove until first buy

            var firstTrade = 0;

            for (var m = trades.Count; firstTrade < m; firstTrade++)
            {
                if (trades[firstTrade].Type == ETradeType.Buy) break;
            }

            trades.RemoveRange(0, firstTrade);

            // Compute fiat values

            trades.ComputeFiatValues(priceProvider);

            // Compute fifo

            var totalFees = 0M;
            var dic = new Dictionary<ECoin, FIFO>();

            foreach (var trade in trades)
            {
                totalFees += trade.FiatFees.Value;

                if (trade is BuyTrade)
                {
                    if (!dic.TryGetValue(trade.To.Coin, out var fifo))
                    {
                        dic[trade.To.Coin] = fifo = new FIFO(trade.To.Coin);
                    }

                    fifo.AddBuy(trade);
                }
                else
                {
                    if (!dic.TryGetValue(trade.From.Coin, out var fifo))
                    {
                        dic[trade.From.Coin] = fifo = new FIFO(trade.From.Coin);
                    }

                    fifo.AddSell(trade);
                }
            }

            Console.WriteLine("TotalFees: " + totalFees.ToString("0,000.00", CultureInfo.InvariantCulture) + " " + priceProvider.Coin.ToString());
            Console.WriteLine("Beneficts: " + 
                dic.Values.Select(u => u.FiatBeneficts).Sum()
                .ToString("0,000.00", CultureInfo.InvariantCulture) + " " + priceProvider.Coin.ToString());
        }
    }
}