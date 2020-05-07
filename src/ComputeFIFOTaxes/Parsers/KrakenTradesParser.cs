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
        public string Name => "Kraken-Trades";

        public IEnumerable<Trade> GetTrades(TradeDataSource dataSource, TradeData current)
        {
            var fetch = current.Data.GetEnumerator();
            if (!fetch.MoveNext()) yield break;

            var header = fetch.Current.Select(u => u.ToString()).ToArray();

            var vol = Array.IndexOf(header, "vol");
            var pair = Array.IndexOf(header, "pair");
            var time = Array.IndexOf(header, "time");
            var txid = Array.IndexOf(header, "txid");
            var cost = Array.IndexOf(header, "cost");
            var type = Array.IndexOf(header, "type");

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
                                Date = ParseDate(row[time].ToString()),
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
                                Date = ParseDate(row[time].ToString()),
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

        private DateTime ParseDate(string date)
        {
            // Remove kraken shit

            date = date.Trim(new char[] { '\'', ' ' });

            // Remove ms

            var ix = date.LastIndexOf('.');
            var ms = 0;
            if (ix > 0)
            {
                ms = Convert.ToInt32(date.Substring(ix + 1));
                date = date.Substring(0, ix);
            }

            var ret = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            ret = ret.AddTicks(ms);

            return ret;
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
                fees.Remove(txid);
                return fee.ToArray();
            }

            return new Quantity[] { };
        }

        private bool ParseCoin(string coin, out ECoin from, out ECoin to)
        {
            switch (coin.ToUpperInvariant())
            {
                case "XXRPZEUR": from = ECoin.XRP; to = ECoin.EUR; break;
                case "XLTCZEUR": from = ECoin.LTC; to = ECoin.EUR; break;
                case "XETHZEUR": from = ECoin.ETH; to = ECoin.EUR; break;
                case "XXBTZEUR": from = ECoin.BTC; to = ECoin.EUR; break;
                case "XXLMZEUR": from = ECoin.XLM; to = ECoin.EUR; break;
                case "EOSEUR": from = ECoin.EOS; to = ECoin.EUR; break;
                case "DASHEUR": from = ECoin.DASH; to = ECoin.EUR; break;

                case "NEXZUSD": from = ECoin.NEX; to = ECoin.USD; break;

                case "USDTZUSD": from = ECoin.USDT; to = ECoin.USD; break;
                case "XETHZUSD": from = ECoin.ETH; to = ECoin.USD; break;

                case "XICNXETH": from = ECoin.ICONOMI; to = ECoin.ETH; break;

                case "XXRPXXBT": from = ECoin.XRP; to = ECoin.BTC; break;
                case "XXDGXXBT": from = ECoin.DOGE; to = ECoin.BTC; break;
                case "XICNXXBT": from = ECoin.ICONOMI; to = ECoin.BTC; break;
                case "XETHXXBT": from = ECoin.ETH; to = ECoin.BTC; break;
                case "XLTCXXBT": from = ECoin.LTC; to = ECoin.BTC; break;

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
