using ComputeFIFOTaxes.Helpers;
using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Parsers;
using ComputeFIFOTaxes.Providers;
using ComputeFIFOTaxes.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ComputeFIFOTaxes.Tests")]

namespace ComputeFIFOTaxes
{
    class Program
    {
        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            var cfg = Config.FromFile("config.json");
            var provider = new GoogleSheetsProvider(cfg);
            var parsers = new ITradeParser[]
            {
                new KrakenLedgerParser(), // Kraken Ledfer before Trades ALWAYS
                new KrakenTradesParser(),
                new BittrexParser(),
                new BinanceParser()
            };
            var priceProvider = new CoinPaprikaPriceProvider(cfg.FiatProvider);

            // Parser trades

            var trades = new List<Trade>();
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

            var notFound = 0;
            var profits = new Dictionary<int, YearProfit>();
            trades.ComputeFifo(out var fifo,
                (trade, fee) =>
                {
                    if (!profits.TryGetValue(trade.Date.Year, out var profit))
                    {
                        profit = new YearProfit()
                        {
                            Year = trade.Date.Year
                        };

                        profits.Add(profit.Year, profit);
                    }

                    profit.Fee += fee;
                },
                (trade, buyPrice, sellPrice, amount) =>
                {
                    if (buyPrice <= 0)
                    {
                        notFound++;
                        Console.WriteLine("Buy not found for: " + trade.ToString());
                    }

                    if (!profits.TryGetValue(trade.Date.Year, out var profit))
                    {
                        profit = new YearProfit()
                        {
                            Year = trade.Date.Year
                        };

                        profits.Add(profit.Year, profit);
                    }

                    profit.Buyed += (amount * buyPrice);
                    profit.Sold += (amount * sellPrice);
                    profit.Profit += (amount * sellPrice) - (amount * buyPrice);
                }
            );

            File.WriteAllText("trades.json", JsonConvert.SerializeObject(trades, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Buy orders not found: " + notFound);
            Console.WriteLine(JsonConvert.SerializeObject(profits, Formatting.Indented));
            Console.WriteLine("Total: " + profits.Last().Value.Total.ToString("#,###,###,##0.00"));
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
