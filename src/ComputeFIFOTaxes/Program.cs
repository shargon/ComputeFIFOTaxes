using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Parsers;
using ComputeFIFOTaxes.Providers;
using ComputeFIFOTaxes.Types;
using System.Collections.Generic;

namespace ComputeFIFOTaxes
{
    class Program
    {
        static void Main(string[] args)
        {
            var sheet = args[0];
            var provider = new GoogleSheetsProvider(sheet);
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
                }
            }

            // Sort trades by date

            trades.Sort((a, b) => a.Date.CompareTo(b.Date));

            // Get prices

            foreach (var trade in trades)
            {
                trade.From.FiatPrice = fiatPriceProvider.GetFiatPrice(trade.From.Coin, trade.Date) * trade.From.Value;
                trade.To.FiatPrice = fiatPriceProvider.GetFiatPrice(trade.To.Coin, trade.Date) * trade.To.Value;
                trade.Fee.FiatPrice = fiatPriceProvider.GetFiatPrice(trade.Fee.Coin, trade.Date) * trade.Fee.Value;
            }

            // Compute fifo


        }
    }
}