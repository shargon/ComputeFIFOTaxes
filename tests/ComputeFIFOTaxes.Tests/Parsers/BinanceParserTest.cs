using ComputeFIFOTaxes.Exchanges;
using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputeFIFOTaxes.Tests.Parsers
{
    [TestClass]
    public class BinanceParserTest
    {
        public IExchange _exchange;

        [TestInitialize]
        public void Init()
        {
            _exchange = new BinanceExchange();
        }

        [TestMethod]
        public void ParseYes()
        {
            var data = new TradeData();

            // Header

            data.Data.Add(new object[] { "Date(UTC)", "Market", "Type", "Price", "Amount", "Total", "Fee", "Fee Coin" });

            // Buy

            data.Data.Add(new object[] { "2019-03-19 9:45:47", "ETHUSDT", "BUY", "138.15", "100.5359", "13889.03459", "0.1005359", "ETH" });

            // Sell

            data.Data.Add(new object[] { "2019-02-24 17:02:45", "OMGBTC", "SELL", "0.000334", "4402.61", "1.47047174", "0.42969395", "BNB" });

            Assert.IsTrue(_exchange.IsThis(data));

            var source = new TradeDataSource() { Data = new TradeData[] { data } };
            var trades = _exchange.GetTrades(source, data).ToArray();

            Assert.AreEqual(trades.Length, 2);

            // Check

            Assert.IsInstanceOfType(trades[0], typeof(BuyTrade));
            Assert.AreEqual(trades[0].Date, new DateTime(2019, 03, 19, 9, 45, 47, DateTimeKind.Utc));
            Assert.AreEqual(trades[0].Fees.Length, 1);
            Assert.AreEqual(trades[0].Fees[0].Coin, ECoin.ETH);
            Assert.AreEqual(trades[0].Fees[0].Value, 0.1005359M);
            Assert.AreEqual(trades[0].From.Coin, ECoin.USDT);
            Assert.AreEqual(trades[0].From.Value, 13889.03459M);
            Assert.AreEqual(trades[0].To.Coin, ECoin.ETH);
            Assert.AreEqual(trades[0].To.Value, 100.5359M);
            Assert.AreEqual(trades[0].Price, 138.15000004973347828984472213M);

            Assert.IsInstanceOfType(trades[1], typeof(SellTrade));
            Assert.AreEqual(trades[1].Date, new DateTime(2019, 02, 24, 17, 02, 45, DateTimeKind.Utc));
            Assert.AreEqual(trades[1].Fees.Length, 1);
            Assert.AreEqual(trades[1].Fees[0].Coin, ECoin.BNB);
            Assert.AreEqual(trades[1].Fees[0].Value, 0.42969395M);
            Assert.AreEqual(trades[1].From.Coin, ECoin.OMG);
            Assert.AreEqual(trades[1].From.Value, 4402.61M);
            Assert.AreEqual(trades[1].To.Coin, ECoin.BTC);
            Assert.AreEqual(trades[1].To.Value, 1.47047174M);
            Assert.AreEqual(trades[1].Price, 0.000334M);
        }

        [TestMethod]
        public void ParseNo()
        {
            var goodHeader = new object[] { "Date(UTC)", "Market", "Type", "Price", "Amount", "Total", "Fee", "Fee Coin" };

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