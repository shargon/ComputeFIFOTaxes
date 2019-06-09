using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ComputeFIFOTaxes.Exchanges
{
    public class BinanceExchange : IExchange
    {
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

                if (!ParseCoin(row[asset].ToString(), out var from, out var to)) continue;

                switch (row[type].ToString().ToUpperInvariant())
                {
                    case "BUY":
                        {
                            yield return new BuyTrade()
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
                                Fees = new Quantity[]
                                {
                                    new Quantity()
                                    {
                                        Coin = ParseCoin(row[feeCoin].ToString()),
                                        Value = decimal.Parse(row[fee].ToString(), CultureInfo.InvariantCulture)
                                    }
                                },
                                Date = DateTime.ParseExact(row[date].ToString(), "yyyy-MM-dd H:mm:ss", CultureInfo.InvariantCulture)
                            };
                            break;
                        }
                    case "SELL":
                        {
                            yield return new SellTrade()
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
                                Fees = new Quantity[]
                                {
                                    new Quantity()
                                    {
                                        Coin = ParseCoin(row[feeCoin].ToString()),
                                        Value = decimal.Parse(row[fee].ToString(), CultureInfo.InvariantCulture)
                                    }
                                },
                                Date = DateTime.ParseExact(row[date].ToString(), "yyyy-MM-dd H:mm:ss", CultureInfo.InvariantCulture)
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
                case "ETHUSDT": from = ECoin.ETH; to = ECoin.USDT; break;
                case "BNBUSDT": from = ECoin.BNB; to = ECoin.USDT; break;
                case "BTCUSDT": from = ECoin.BTC; to = ECoin.USDT; break;
                case "OMGBTC": from = ECoin.OMG; to = ECoin.BTC; break;
                case "BTTBTC": from = ECoin.BTT; to = ECoin.BTC; break;
                case "HOTBTC": from = ECoin.HOT; to = ECoin.BTC; break;
                case "BNBETH": from = ECoin.BNB; to = ECoin.ETH; break;
                case "NEOETH": from = ECoin.NEO; to = ECoin.ETH; break;
                case "NEOBTC": from = ECoin.NEO; to = ECoin.BTC; break;
                case "GASBTC": from = ECoin.GAS; to = ECoin.BTC; break;

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

                default: throw new ArgumentException(coin);
            }
        }

        public bool IsThis(TradeData data)
        {
            var first = data.Data.FirstOrDefault();

            if (first == null) return false;

            return string.Join(",", first.Select(u => u.ToString()).ToArray()) == "Date(UTC),Market,Type,Price,Amount,Total,Fee,Fee Coin";
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}