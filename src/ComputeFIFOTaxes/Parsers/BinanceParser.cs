using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ComputeFIFOTaxes.Parsers
{
    public class BinanceParser : ITradeParser
    {
        private readonly string[] WantCoulmns = "Date(UTC),Market,Type,Price,Amount,Total,Fee,Fee Coin".Split(",");
        public string Name => "Binance";

        public IEnumerable<Trade> GetTrades(TradeDataSource dataSource, TradeData current)
        {
            var fetch = current.Data.GetEnumerator();
            if (!fetch.MoveNext()) yield break;

            var header = fetch.Current.Select(u => u.ToString()).ToArray();

            var date = Array.IndexOf(header, "Date(UTC)");
            var type = Array.IndexOf(header, "Type");
            var asset = Array.IndexOf(header, "Market");
            var amount = Array.IndexOf(header, "Amount");
            var total = Array.IndexOf(header, "Total");
            var fee = Array.IndexOf(header, "Fee");
            var feeCoin = Array.IndexOf(header, "Fee Coin");

            if (date < 0 || type < 0 || asset < 0 || amount < 0 || fee < 0 || feeCoin < 0 || total < 0) yield break;

            var list = new List<Trade>();

            while (fetch.MoveNext())
            {
                var row = fetch.Current;

                if (row.Length < 8 || !ParseCoin(row[asset].ToString(), out var from, out var to)) continue;

                var dfee = decimal.Parse(row[fee].ToString(), CultureInfo.InvariantCulture);

                switch (row[type].ToString().ToUpperInvariant())
                {
                    case "BUY":
                        {
                            var trade = new BuyTrade()
                            {
                                Exchange = this,
                                From = new Quantity()
                                {
                                    Coin = to,
                                    Value = decimal.Parse(row[total].ToString(), CultureInfo.InvariantCulture)
                                },
                                To = new Quantity()
                                {
                                    Coin = from,
                                    Value = decimal.Parse(row[amount].ToString(), CultureInfo.InvariantCulture)
                                },
                                Fees = dfee == 0 ? new Quantity[0] : new Quantity[]
                                {
                                    new Quantity()
                                    {
                                        Coin = ParseCoin(row[feeCoin].ToString()),
                                        Value = dfee
                                    }
                                },
                                Date = DateTime.ParseExact(row[date].ToString(), "yyyy-MM-dd H:mm:ss", CultureInfo.InvariantCulture)
                            };
                            yield return trade;
                            break;
                        }
                    case "SELL":
                        {
                            var trade = new SellTrade()
                            {
                                Exchange = this,
                                From = new Quantity()
                                {
                                    Coin = from,
                                    Value = decimal.Parse(row[amount].ToString(), CultureInfo.InvariantCulture)
                                },
                                To = new Quantity()
                                {
                                    Coin = to,
                                    Value = decimal.Parse(row[total].ToString(), CultureInfo.InvariantCulture)
                                },
                                Fees = dfee == 0 ? new Quantity[0] : new Quantity[]
                                {
                                    new Quantity()
                                    {
                                        Coin = ParseCoin(row[feeCoin].ToString()),
                                        Value = dfee
                                    }
                                },
                                Date = DateTime.ParseExact(row[date].ToString(), "yyyy-MM-dd H:mm:ss", CultureInfo.InvariantCulture)
                            };
                            yield return trade;
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
                case "ETHUSDT": from = ECoin.ETH; to = ECoin.USDT; break;
                case "BNBUSDT": from = ECoin.BNB; to = ECoin.USDT; break;
                case "BNBBTC": from = ECoin.BNB; to = ECoin.BTC; break;
                case "BTCUSDT": from = ECoin.BTC; to = ECoin.USDT; break;
                case "NEOUSDT": from = ECoin.NEO; to = ECoin.USDT; break;
                case "XRPUSDT": from = ECoin.XRP; to = ECoin.USDT; break;
                case "ONTUSDT": from = ECoin.ONT; to = ECoin.USDT; break;
                case "XMRUSDT": from = ECoin.XMR; to = ECoin.USDT; break;

                case "ETHBTC": from = ECoin.ETH; to = ECoin.BTC; break;
                case "OMGBTC": from = ECoin.OMG; to = ECoin.BTC; break;
                case "BTTBTC": from = ECoin.BTT; to = ECoin.BTC; break;
                case "HOTBTC": from = ECoin.HOT; to = ECoin.BTC; break;
                case "NEOBTC": from = ECoin.NEO; to = ECoin.BTC; break;
                case "GASBTC": from = ECoin.GAS; to = ECoin.BTC; break;
                case "XRPBTC": from = ECoin.XRP; to = ECoin.BTC; break;
                case "ONTBTC": from = ECoin.ONT; to = ECoin.BTC; break;

                case "BNBETH": from = ECoin.BNB; to = ECoin.ETH; break;
                case "NEOETH": from = ECoin.NEO; to = ECoin.ETH; break;
                case "OMGETH": from = ECoin.OMG; to = ECoin.ETH; break;
                case "NCASHETH": from = ECoin.NCASH; to = ECoin.ETH; break;
                case "STORMETH": from = ECoin.STORM; to = ECoin.ETH; break;

                case "BTTBNB": from = ECoin.BTT; to = ECoin.BNB; break;

                default: throw new ArgumentException(coin);
            }

            return true;
        }

        private ECoin ParseCoin(string coin)
        {
            switch (coin.ToUpperInvariant())
            {
                case "BTC": return ECoin.BTC;
                case "ETH": return ECoin.ETH;
                case "USDT": return ECoin.USDT;
                case "BTT": return ECoin.BTT;
                case "HOT": return ECoin.HOT;
                case "OMG": return ECoin.OMG;
                case "EOS": return ECoin.EOS;
                case "NEO": return ECoin.NEO;
                case "GAS": return ECoin.GAS;
                case "XRP": return ECoin.XRP;
                case "BNB": return ECoin.BNB;
                case "NCASH": return ECoin.NCASH;
                case "STORM": return ECoin.STORM;

                default: throw new ArgumentException(coin);
            }
        }

        public bool IsThis(TradeData data)
        {
            var first = data.Data.FirstOrDefault();

            if (first == null) return false;

            return WantCoulmns.All(u => first.Contains(u));
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}