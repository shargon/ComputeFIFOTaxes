using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Parsers;
using ComputeFIFOTaxes.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputeFIFOTaxes.Tests.Parsers
{
    [TestClass]
    public class KrakenTradesParserTest
    {
        public ITradeParser _exchange;

        [TestInitialize]
        public void Init()
        {
            _exchange = new KrakenTradesParser();
        }

        [TestMethod]
        public void ParseYes()
        {
            var data = new TradeData();

            // Header

            data.Data.Add(new object[] { "txid", "ordertxid", "pair", "time", "type", "ordertype", "price", "cost", "fee", "vol", "margin", "misc", "ledgers" });

            // Sell

            data.Data.Add(new object[] { "TK6PI7-LU6O6-4TFNSU", "OL4PWT-PATIC-H34G7C", "XETHZEUR", "2019-01-08 23:14", "sell", "limit", "130.69", "64735.5073", "142.41814", "495.336348", "0", "", "LOW427-2YEAY-KRSPNH,LOCEOI-OZ3UZ-U4IGWP" });

            // Buy

            data.Data.Add(new object[] { "TLREIQ-ESBRY-SYVMIE", "ORS77A-PB5OK-GJLNBM", "XETHZEUR", "2019-01-09 21:35", "buy", "limit", "129.37", "6468.5", "14.2307", "50", "0", "", "LRUCE5-2P6JX-UPBY2F,LF5JER-3CDMZ-WJIGQK" });


            Assert.IsTrue(_exchange.IsThis(data));

            var source = new TradeDataSource() { Data = new TradeData[] { data } };
            source.Variables[KrakenLedgerParser.KrakenLedgerVariableName] = new Dictionary<string, IList<Quantity>>()
            {
                { "TK6PI7-LU6O6-4TFNSU", new Quantity[]{ new Quantity(){ Coin= ECoin.EUR, Value= 142.4183M } } },
                { "TLREIQ-ESBRY-SYVMIE", new Quantity[]{ new Quantity(){ Coin= ECoin.EUR, Value= 142.4183M } } }
            };
            var trades = _exchange.GetTrades(source, data).ToArray();

            Assert.AreEqual(trades.Length, 2);

            // Check

            Assert.IsInstanceOfType(trades[0], typeof(SellTrade));
            Assert.AreEqual(trades[0].Date, new DateTime(2019, 01, 08, 23, 14, 0, DateTimeKind.Utc));
            Assert.AreEqual(trades[0].Fees.Length, 1);
            Assert.AreEqual(trades[0].Fees[0].Coin, ECoin.EUR);
            Assert.AreEqual(trades[0].Fees[0].Value, 142.4183M);
            Assert.AreEqual(trades[0].From.Coin, ECoin.ETH);
            Assert.AreEqual(trades[0].From.Value, 495.336348M);
            Assert.AreEqual(trades[0].To.Coin, ECoin.EUR);
            Assert.AreEqual(trades[0].To.Value, 64735.5073M);
            Assert.AreEqual(trades[0].Price, 130.68999995938113550269886514M);

            Assert.IsInstanceOfType(trades[1], typeof(BuyTrade));
            Assert.AreEqual(trades[1].Date, new DateTime(2019, 01, 09, 21, 35, 0, DateTimeKind.Utc));
            Assert.AreEqual(trades[1].Fees.Length, 1);
            Assert.AreEqual(trades[1].Fees[0].Coin, ECoin.EUR);
            Assert.AreEqual(trades[1].Fees[0].Value, 142.4183M);
            Assert.AreEqual(trades[1].From.Coin, ECoin.EUR);
            Assert.AreEqual(trades[1].From.Value, 6468.5M);
            Assert.AreEqual(trades[1].To.Coin, ECoin.ETH);
            Assert.AreEqual(trades[1].To.Value, 50M);
            Assert.AreEqual(trades[1].Price, 129.37M);
        }

        [TestMethod]
        public void ParseNo()
        {
            var goodHeader = new object[] { "txid", "ordertxid", "pair", "time", "type", "ordertype", "price", "cost", "fee", "vol", "margin", "misc", "ledgers" };

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