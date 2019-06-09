using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ComputeFIFOTaxes.Parsers
{
    /// <summary>
    /// Trades without a valid fee
    /// </summary>
    public class KrakenTradesParser : ITradeParser
    {
        public IEnumerable<Trade> GetTrades(TradeDataSource dataSource, TradeData current)
        {
            var fetch = current.Data.GetEnumerator();
            if (!fetch.MoveNext()) yield break;

            var header = fetch.Current.Select(u => u.ToString()).ToArray();

            var pair = Array.IndexOf(header, "pair");
            var time = Array.IndexOf(header, "time");
            var txid = Array.IndexOf(header, "txid");
            var cost = Array.IndexOf(header, "cost");
            var type = Array.IndexOf(header, "type");
            var vol = Array.IndexOf(header, "vol");

            if (pair < 0 || time < 0 || txid < 0 || cost < 0 || type < 0 || vol < 0) yield break;

            var list = new List<Trade>();

            while (fetch.MoveNext())
            {
                var row = fetch.Current;

                if (!ParseCoin(row[pair].ToString(), out var from, out var to)) continue;

                switch (row[type].ToString().ToUpperInvariant())
                {
                    case "BUY":
                        {
                            yield return new BuyTrade()
                            {
                                Exchange = this,
                                Date = DateTime.ParseExact(row[time].ToString(), "yyyy-MM-dd H:mm", CultureInfo.InvariantCulture),
                                From = new Quantity()
                                {
                                    Coin = to,
                                    Value = decimal.Parse(row[cost].ToString(), CultureInfo.InvariantCulture)
                                },
                                To = new Quantity()
                                {
                                    Coin = from,
                                    Value = decimal.Parse(row[vol].ToString(), CultureInfo.InvariantCulture)
                                },
                                Fees = FindFee(dataSource, row[txid].ToString())
                            };
                            break;
                        }
                    case "SELL":
                        {
                            yield return new SellTrade()
                            {
                                Exchange = this,
                                Date = DateTime.ParseExact(row[time].ToString(), "yyyy-MM-dd H:mm", CultureInfo.InvariantCulture),
                                From = new Quantity()
                                {
                                    Coin = from,
                                    Value = decimal.Parse(row[vol].ToString(), CultureInfo.InvariantCulture)
                                },
                                To = new Quantity()
                                {
                                    Coin = to,
                                    Value = decimal.Parse(row[cost].ToString(), CultureInfo.InvariantCulture)
                                },
                                Fees = FindFee(dataSource, row[txid].ToString())
                            };
                            break;
                        }

                    default: throw new ArgumentException(nameof(type));
                }
            }
        }

        private Quantity[] FindFee(TradeDataSource dataSource, string txid)
        {
            if (!dataSource.Variables.TryGetValue(KrakenLedgerParser.KrakenLedgerVariableName, out var krakenFees) ||
                !(krakenFees is Dictionary<string, IList<Quantity>> fees))
            {
                return new Quantity[] { };
            }

            if (fees.TryGetValue(txid, out var fee))
            {
                return fee.ToArray();
            }

            return new Quantity[] { };
        }

        private bool ParseCoin(string coin, out ECoin from, out ECoin to)
        {
            switch (coin.ToUpperInvariant())
            {
                case "XETHZEUR": from = ECoin.ETH; to = ECoin.EUR; break;
                case "XXBTZEUR": from = ECoin.BTC; to = ECoin.EUR; break;
                case "EOSEUR": from = ECoin.EOS; to = ECoin.EUR; break;

                default: throw new ArgumentException(coin);
            }

            return true;
        }

        public bool IsThis(TradeData data)
        {
            var first = data.Data.FirstOrDefault();

            if (first == null) return false;

            return string.Join(",", first.Select(u => u.ToString()).ToArray()) == "txid,ordertxid,pair,time,type,ordertype,price,cost,fee,vol,margin,misc,ledgers";
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}