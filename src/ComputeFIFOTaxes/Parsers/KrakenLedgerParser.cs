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
    /// Is used because the fee contains the valid values
    /// </summary>
    public class KrakenLedgerParser : ITradeParser
    {
        internal const string KrakenLedgerVariableName = "KrakenLedger";
        public string Name => "Kraken-Ledger";

        public IEnumerable<Trade> GetTrades(TradeDataSource dataSource, TradeData current)
        {
            var fetch = current.Data.GetEnumerator();
            if (!fetch.MoveNext()) yield break;

            var header = fetch.Current.Select(u => u.ToString()).ToArray();

            var time = Array.IndexOf(header, "time");
            var asset = Array.IndexOf(header, "asset");
            var type = Array.IndexOf(header, "type");
            var fee = Array.IndexOf(header, "fee");
            var refid = Array.IndexOf(header, "refid");
            var txid = Array.IndexOf(header, "txid");
            var amount = Array.IndexOf(header, "amount");

            if (time < 0 || type < 0 || refid < 0 || asset < 0 || amount < 0 || fee < 0) yield break;

            Dictionary<string, IList<Quantity>> dic;
            if (dataSource.Variables.TryGetValue(KrakenLedgerVariableName, out var dicObj))
            {
                dic = (Dictionary<string, IList<Quantity>>)dicObj;
            }
            else
            {
                dataSource.Variables[KrakenLedgerVariableName] = dic = new Dictionary<string, IList<Quantity>>();
            }

            while (fetch.MoveNext())
            {
                var row = fetch.Current;

                if (string.IsNullOrWhiteSpace(row[txid].ToString())) continue;

                var add = false;

                if (!dic.TryGetValue(row[refid].ToString(), out var list))
                {
                    list = new List<Quantity>();
                    add = true;
                }

                var realfee = new Quantity()
                {
                    Coin = ParseCoin(row[asset].ToString()),
                    Value = Decimal.Parse(row[fee].ToString(), CultureInfo.InvariantCulture)
                };

                if (realfee.Value == 0) continue;

                if (add)
                {
                    dic[row[refid].ToString()] = list;
                }

                list.Add(realfee);
            }

            yield break;
        }

        private static ECoin ParseCoin(string coin)
        {
            switch (coin.ToUpperInvariant())
            {
                case "BSV": return ECoin.BSV;
                case "BCH": return ECoin.BCH;
                case "USDT": return ECoin.USDT;
                case "ZUSD": return ECoin.USD;
                case "ZEUR": return ECoin.EUR;
                case "XETH": return ECoin.ETH;
                case "XXRP": return ECoin.XRP;
                case "XXBT": return ECoin.BTC;
                case "EOS": return ECoin.EOS;
                case "XLTC": return ECoin.LTC;
                case "XXDG": return ECoin.DOGE;
                case "XICN": return ECoin.ICONOMI;
                case "XXLM": return ECoin.XLM;
                case "DASH": return ECoin.DASH;

                default: throw new ArgumentException(coin);
            }
        }

        public bool IsThis(TradeData data)
        {
            var first = data.Data.FirstOrDefault();

            if (first == null) return false;

            return string.Join(",", first.Select(u => u.ToString()).ToArray()) == "txid,refid,time,type,aclass,asset,amount,fee,balance";
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}