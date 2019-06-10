using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Parsers;
using ComputeFIFOTaxes.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace ComputeFIFOTaxes.Tests.Parsers
{
    [TestClass]
    public class KrakenLedgerParserTest
    {
        public ITradeParser _exchange;

        [TestInitialize]
        public void Init()
        {
            _exchange = new KrakenLedgerParser();
        }

        [TestMethod]
        public void ParseYes()
        {
            var data = new TradeData();

            // Header

            data.Data.Add(new object[] { "txid", "refid", "time", "type", "aclass", "asset", "amount", "fee", "balance" });

            // Ordered

            data.Data.Add(new object[] { "LOCEOI-OZ3UZ-U4IGW3", "TK6PI7-LU6O6-4TFNSX", "'2019-01-08 23:14:00.7934", "trade", "currency", "XETH", "-495.336348", "0", "0.00000001" });
            data.Data.Add(new object[] { "LOW427-2YEAY-KRSPN4", "TK6PI7-LU6O6-4TFNSX", "'2019-01-08 23:14:00.7934", "trade", "currency", "ZEUR", "64735.5072", "142.4183", "64593.0889" });

            // Unordered

            data.Data.Add(new object[] { "LOW427-2YEAY-KRSPN1", "TK6PI7-LU6O6-4TFNSA", "'2019-01-08 23:14:00.7934", "trade", "currency", "ZEUR", "64735.5072", "142.4183", "64593.0889" });
            data.Data.Add(new object[] { "LOCEOI-OZ3UZ-U4IGW2", "TK6PI7-LU6O6-4TFNSA", "'2019-01-08 23:14:00.7934", "trade", "currency", "XETH", "-495.336348", "0", "0.00000001" });


            Assert.IsTrue(_exchange.IsThis(data));

            var source = new TradeDataSource() { Data = new TradeData[] { data } };
            var trades = _exchange.GetTrades(source, data).ToArray();

            Assert.AreEqual(trades.Length, 0);

            // Check

            var vars = (Dictionary<string, IList<Quantity>>)(source.Variables[KrakenLedgerParser.KrakenLedgerVariableName]);
            var trade = vars["TK6PI7-LU6O6-4TFNSX"].ToArray();

            Assert.AreEqual(trade.Length, 1);
            Assert.AreEqual(trade[0].Coin, ECoin.EUR);
            Assert.AreEqual(trade[0].Value, 142.4183M);

            trade = vars["TK6PI7-LU6O6-4TFNSA"].ToArray();

            Assert.AreEqual(trade.Length, 1);
            Assert.AreEqual(trade[0].Coin, ECoin.EUR);
            Assert.AreEqual(trade[0].Value, 142.4183M);
        }

        [TestMethod]
        public void ParseNo()
        {
            var goodHeader = new object[] { "txid", "refid", "time", "type", "aclass", "asset", "amount", "fee", "balance" };

            for (int x = 0; x < goodHeader.Length; x++)
            {
                var data = new TradeData();

                // Header

                var row = new List<object>(goodHeader);
                row.RemoveAt(x);
                data.Data.Add(row.ToArray());

                Assert.IsFalse(_exchange.IsThis(data));
            }
        }
    }
}