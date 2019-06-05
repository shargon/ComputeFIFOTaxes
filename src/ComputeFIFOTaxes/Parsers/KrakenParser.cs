using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ComputeFIFOTaxes.Parsers
{
    public class KrakenParser : IParser
    {
        public IEnumerable<Trade> GetTrades(TradeData data)
        {
            var fetch = data.Data.GetEnumerator();
            if (!fetch.MoveNext()) yield break;

            var header = fetch.Current.Select(u => u.ToString()).ToArray();

            var date = Array.IndexOf(header, "time");
            var type = Array.IndexOf(header, "type");
            var refid = Array.IndexOf(header, "refid");
            var asset = Array.IndexOf(header, "asset");
            var amount = Array.IndexOf(header, "amount");
            var fee = Array.IndexOf(header, "fee");

            if (date < 0 || type < 0 || refid < 0 || asset < 0 || amount < 0 || fee < 0) yield break;

            var list = new List<Trade>();

            while (fetch.MoveNext())
            {
                var row = fetch.Current;

                if (row[type].ToString() != "trade") continue;

                list.Add(new Trade()
                {
                    Exchange = "Kraken",
                    Id = row[refid].ToString(),
                    From = new Quantity()
                    {
                        Coin = ParseCoin(row[asset].ToString()),
                        Value = Decimal.Parse(row[amount].ToString(), CultureInfo.InvariantCulture)
                    },
                    Fees = new Quantity[]
                    {
                        new Quantity()
                        {
                            Coin = ParseCoin(row[asset].ToString()),
                            Value = Decimal.Parse(row[fee].ToString(), CultureInfo.InvariantCulture)
                        }
                    },
                    Date = DateTime.ParseExact(row[date].ToString(), "yyyy-MM-dd H:mm", CultureInfo.InvariantCulture)
                });
            }

            for (var x = 0; x < list.Count - 1; x++)
            {
                var trade = list[x];
                var nextTrade = list.Where(u => u.Id == trade.Id && u != trade).ToArray();

                if (nextTrade.Length != 1 || !list.Remove(nextTrade[0]))
                {
                    throw new ArgumentException("More than 1 register");
                }

                if (trade.From.Value < 0)
                {
                    trade.To = nextTrade[0].From;

                    if (trade.Fees[0].Value == 0)
                    {
                        if (nextTrade[0].Fees[0].Value > 0)
                        {
                            trade.Fees = nextTrade[0].Fees;
                        }
                        else
                        {
                            trade.Fees = new Quantity[0];
                        }
                    }
                    else
                    {
                        if (trade.Fees[0].Value > 0 && nextTrade[0].Fees[0].Value > 0)
                        {
                            trade.Fees = new Quantity[] { trade.Fees[0], nextTrade[0].Fees[0] };
                        }
                    }
                }
                else
                {
                    trade.To = trade.From;
                    trade.From = nextTrade[0].From;
                }
            }

            // Fetch items

            foreach (var entry in list)
            {
                if (entry.To == null) throw new ArgumentException("From not found");

                yield return entry;
            }
        }

        private ECoin ParseCoin(string coin)
        {
            switch (coin.ToUpperInvariant())
            {
                case "ZEUR": return ECoin.EUR;
                case "XETH": return ECoin.ETH;
                case "XXBT": return ECoin.BTC;
                case "EOS": return ECoin.EOS;

                default: throw new ArgumentException(nameof(coin));
            }
        }

        public bool IsThis(TradeData data)
        {
            var first = data.Data.FirstOrDefault();

            return first != null &&
                string.Join(",", first.Select(u => u.ToString()).ToArray()) == "txid,refid,time,type,aclass,asset,amount,fee,balance";
        }
    }
}