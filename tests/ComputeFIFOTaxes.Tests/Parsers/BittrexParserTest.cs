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
    public class BittrexParserTest
    {
        public ITradeParser _exchange;

        [TestInitialize]
        public void Init()
        {
            _exchange = new BittrexParser();
        }

        [TestMethod]
        public void ParseYes()
        {
            var data = new TradeData();

            // Header

            data.Data.Add(new object[] { "Uuid", "Exchange", "Type", "Quantity", "Limit", "CommissionPaid", "Price", "Opened", "Closed" });

            // Buy

            data.Data.Add(new object[] { "031d20b1-6394-4ea5-b89b-52fe677fc731", "USDT-BTC", "LIMIT_BUY", "0.00117194", "8042.82891", "0.02356428", "9.42571291", "12/16/2017 7:12:09", "12/16/2017 20:04:17" });

            // Sell

            data.Data.Add(new object[] { "a19fbf2d-73f8-4526-9856-95b715c1dd42", "USDT-BTC", "LIMIT_SELL", "5.17527292", "5980", "77.39161894", "30956.6476", "12/16/2017 20:47:27", "12/16/2017 20:47:27" });

            Assert.IsTrue(_exchange.IsThis(data));

            var source = new TradeDataSource() { Data = new TradeData[] { data } };
            var trades = _exchange.GetTrades(source, data).ToArray();

            Assert.AreEqual(trades.Length, 2);

            // Check

            Assert.IsInstanceOfType(trades[0], typeof(BuyTrade));
            Assert.AreEqual(trades[0].Date, new DateTime(2017, 12, 16, 7, 12, 09, DateTimeKind.Utc));
            Assert.AreEqual(trades[0].Fees.Length, 1);
            Assert.AreEqual(trades[0].Fees[0].Coin, ECoin.USDT);
            Assert.AreEqual(trades[0].Fees[0].Value, 0.02356428M);
            Assert.AreEqual(trades[0].From.Coin, ECoin.USDT);
            Assert.AreEqual(trades[0].From.Value, 9.42571291M);
            Assert.AreEqual(trades[0].To.Coin, ECoin.BTC);
            Assert.AreEqual(trades[0].To.Value, 0.00117194M);
            Assert.AreEqual(trades[0].Price, 8042.828907623257163336006963M);

            Assert.IsInstanceOfType(trades[1], typeof(SellTrade));
            Assert.AreEqual(trades[1].Date, new DateTime(2017, 12, 16, 20, 47, 27, DateTimeKind.Utc));
            Assert.AreEqual(trades[1].Fees.Length, 1);
            Assert.AreEqual(trades[1].Fees[0].Coin, ECoin.USDT);
            Assert.AreEqual(trades[1].Fees[0].Value, 77.39161894M);
            Assert.AreEqual(trades[1].From.Coin, ECoin.BTC);
            Assert.AreEqual(trades[1].From.Value, 5.17527292M);
            Assert.AreEqual(trades[1].To.Coin, ECoin.USDT);
            Assert.AreEqual(trades[1].To.Value, 30956.6476M);
            Assert.AreEqual(trades[1].Price, 5981.6454278898203498029240166M);
        }

        [TestMethod]
        public void ParseNo()
        {
            var goodHeader = new object[] { "Uuid", "Exchange", "Type", "Quantity", "Limit", "CommissionPaid", "Price", "Opened", "Closed" };

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