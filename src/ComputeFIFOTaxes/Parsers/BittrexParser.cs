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
    /// Bittrex full orders
    /// </summary>
    public class BittrexParser : ITradeParser
    {
        public string Name => "Bittrex";

        public IEnumerable<Trade> GetTrades(TradeDataSource dataSource, TradeData current)
        {
            var fetch = current.Data.GetEnumerator();
            if (!fetch.MoveNext()) yield break;

            var header = fetch.Current.Select(u => u.ToString()).ToArray();

            var vol = Array.IndexOf(header, "Quantity");
            var pair = Array.IndexOf(header, "Exchange");
            var time = Array.IndexOf(header, "Opened");
            var cost = Array.IndexOf(header, "Price");
            var fee = Array.IndexOf(header, "CommissionPaid");
            var type = Array.IndexOf(header, "Type");

            if (pair < 0 || time < 0 || fee < 0 || cost < 0 || type < 0 || vol < 0) yield break;

            var list = new List<Trade>();

            while (fetch.MoveNext())
            {
                var row = fetch.Current;

                if (!ParseCoin(row[pair].ToString(), out var from, out var to)) continue;

                switch (row[type].ToString().ToUpperInvariant())
                {
                    case "LIMIT_BUY":
                        {
                            yield return new BuyTrade()
                            {
                                Exchange = this,
                                Date = DateTime.ParseExact(row[time].ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                                From = new Quantity()
                                {
                                    Coin = from,
                                    Value = decimal.Parse(row[cost].ToString(), CultureInfo.InvariantCulture)
                                },
                                To = new Quantity()
                                {
                                    Coin = to,
                                    Value = decimal.Parse(row[vol].ToString(), CultureInfo.InvariantCulture)
                                },
                                Fees = new Quantity[]
                                {
                                    new Quantity()
                                    {
                                         Coin = from,
                                         Value = decimal.Parse(row[fee].ToString(), CultureInfo.InvariantCulture)
                                    }
                                }
                            };
                            break;
                        }
                    case "LIMIT_SELL":
                        {
                            yield return new SellTrade()
                            {
                                Exchange = this,
                                Date = DateTime.ParseExact(row[time].ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                                From = new Quantity()
                                {
                                    Coin = to,
                                    Value = decimal.Parse(row[vol].ToString(), CultureInfo.InvariantCulture)
                                },
                                To = new Quantity()
                                {
                                    Coin = from,
                                    Value = decimal.Parse(row[cost].ToString(), CultureInfo.InvariantCulture)
                                },
                                Fees = new Quantity[]
                                {
                                    new Quantity()
                                    {
                                         Coin = from,
                                         Value = decimal.Parse(row[fee].ToString(), CultureInfo.InvariantCulture)
                                    }
                                }
                            };
                            break;
                        }

                    default: throw new ArgumentException(nameof(type));
                }
            }
        }

        private bool ParseCoin(string coin, out ECoin from, out ECoin to)
        {
            switch (coin.ToUpperInvariant())
            {
                case "ETH-OMG": from = ECoin.ETH; to = ECoin.OMG; break;
                case "ETH-NEO": from = ECoin.ETH; to = ECoin.NEO; break;
                case "ETH-WAVES": from = ECoin.ETH; to = ECoin.WAVES; break;
                case "ETH-BNT": from = ECoin.ETH; to = ECoin.BNT; break;

                case "USDT-OMG": from = ECoin.USDT; to = ECoin.OMG; break;
                case "USDT-BTC": from = ECoin.USDT; to = ECoin.BTC; break;
                case "USDT-XRP": from = ECoin.USDT; to = ECoin.XRP; break;

                case "BTC-ETH": from = ECoin.BTC; to = ECoin.ETH; break;
                case "BTC-WAVES": from = ECoin.BTC; to = ECoin.WAVES; break;
                case "BTC-OMG": from = ECoin.BTC; to = ECoin.OMG; break;
                case "BTC-MONA": from = ECoin.BTC; to = ECoin.MONA; break;
                case "BTC-XEL": from = ECoin.BTC; to = ECoin.XEL; break;
                case "BTC-ZEC": from = ECoin.BTC; to = ECoin.ZEC; break;
                case "BTC-NEO": from = ECoin.BTC; to = ECoin.NEO; break;
                case "BTC-SALT": from = ECoin.BTC; to = ECoin.SALT; break;
                case "BTC-WINGS": from = ECoin.BTC; to = ECoin.WINGS; break;
                case "BTC-BRX": from = ECoin.BTC; to = ECoin.BRX; break;
                case "BTC-MCO": from = ECoin.BTC; to = ECoin.MCO; break;
                case "BTC-IOC": from = ECoin.BTC; to = ECoin.IOC; break;
                case "BTC-ARK": from = ECoin.BTC; to = ECoin.ARK; break;
                case "BTC-UBQ": from = ECoin.BTC; to = ECoin.UBQ; break;
                case "BTC-SHIFT": from = ECoin.BTC; to = ECoin.SHIFT; break;
                case "BTC-QRL": from = ECoin.BTC; to = ECoin.QRL; break;
                case "BTC-GAME": from = ECoin.BTC; to = ECoin.GAME; break;
                case "BTC-STRAT": from = ECoin.BTC; to = ECoin.STRAT; break;
                case "BTC-STEEM": from = ECoin.BTC; to = ECoin.STEEM; break;
                case "BTC-BAT": from = ECoin.BTC; to = ECoin.BAT; break;
                case "BTC-EMC2": from = ECoin.BTC; to = ECoin.EMC2; break;
                case "BTC-LSK": from = ECoin.BTC; to = ECoin.LSK; break;
                case "BTC-IOP": from = ECoin.BTC; to = ECoin.IOP; break;
                case "BTC-BNT": from = ECoin.BTC; to = ECoin.BNT; break;

                default: throw new ArgumentException(coin);
            }

            return true;
        }

        public bool IsThis(TradeData data)
        {
            var first = data.Data.FirstOrDefault();

            if (first == null) return false;

            return string.Join(",", first.Select(u => u.ToString()).ToArray()) == "Uuid,Exchange,Type,Quantity,Limit,CommissionPaid,Price,Opened,Closed";
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
