using ComputeFIFOTaxes.Helpers;
using ComputeFIFOTaxes.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ComputeFIFOTaxes.Tests
{
    [TestClass]
    public class TradeHelperTest
    {
        [TestMethod]
        public void SortTest()
        {
            var time = DateTime.UtcNow;
            var list = new List<Trade>
            {
                new SellTrade() { Date = time.AddMilliseconds(1), FiatFees = 3 },
                new SellTrade() { Date = time , FiatFees = 2},
                new BuyTrade() { Date = time , FiatFees = 1}
            };

            list.SortByDate();

            Assert.AreEqual(1, list[0].FiatFees);
            Assert.AreEqual(2, list[1].FiatFees);
            Assert.AreEqual(3, list[2].FiatFees);
        }
    }
}