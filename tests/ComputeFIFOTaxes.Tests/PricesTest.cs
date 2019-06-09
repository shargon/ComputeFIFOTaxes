using ComputeFIFOTaxes.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ComputeFIFOTaxes.Tests
{
    [TestClass]
    public class PricesTest
    {
        [TestMethod]
        public void SellTest()
        {
            var trade = new SellTrade()
            {
                Date = DateTime.UtcNow,
                From = new Quantity()
                {
                    Coin = ECoin.OMG,
                    Value = 4_000M
                },
                To = new Quantity()
                {
                    Coin = ECoin.BTC,
                    Value = 1M
                }
            };

            Assert.AreEqual(trade.Price, 0.00025M);
        }

        [TestMethod]
        public void BuyTest()
        {
            var trade = new BuyTrade()
            {
                Date = DateTime.UtcNow,
                From = new Quantity()
                {
                    Coin = ECoin.USDT,
                    Value = 10_000M
                },
                To = new Quantity()
                {
                    Coin = ECoin.ETH,
                    Value = 100M
                }
            };

            Assert.AreEqual(trade.Price, 100M);
        }
    }
}