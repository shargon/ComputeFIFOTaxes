using ComputeFIFOTaxes.Helpers;
using ComputeFIFOTaxes.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ComputeFIFOTaxes.Tests
{
    [TestClass]
    public class FeeTest
    {
        #region Without conversion

        [TestMethod]
        public void Sumarize_None()
        {
            Sumarize_None(new SellTrade());
            Sumarize_None(new BuyTrade());
        }

        public void Sumarize_None(Trade trade)
        {
            trade.Date = DateTime.UtcNow;
            trade.From = new Quantity()
            {
                Coin = ECoin.ETH,
                Value = 1
            };
            trade.To = new Quantity()
            {
                Coin = ECoin.BTC,
                Value = 0.5M
            };
            trade.Fees = new Quantity[]
            {
                new Quantity()
                {
                    Coin = ECoin.BTC,
                    Value = 0.001M
                }
            };

            CollectionAssert.AreEqual(trade.Fees, trade.SumarizeFees(ECoin.ETH));

            trade.Fees = new Quantity[]
            {
                new Quantity()
                {
                    Coin = ECoin.BNB,
                    Value = 10.001M
                }
            };

            CollectionAssert.AreEqual(trade.Fees, trade.SumarizeFees(ECoin.ETH));
        }

        #endregion

        #region Join

        [TestMethod]
        public void Sumarize_Join()
        {
            var trade = new SellTrade()
            {
                Date = DateTime.UtcNow,
                From = new Quantity()
                {
                    Coin = ECoin.ETH,
                    Value = 10
                },
                To = new Quantity()
                {
                    Coin = ECoin.BTC,
                    Value = 0.5M
                },
                Fees = new Quantity[]
               {
                    new Quantity()
                    {
                         Coin= ECoin.BTC,
                         Value= 0.5M
                    },
                    new Quantity()
                    {
                         Coin= ECoin.ETH,
                         Value= 0.1M
                    }
               }
            };

            Assert.AreEqual(trade.Price, 0.05M);

            CollectionAssert.AreEqual(new Quantity[]
            {
                new Quantity()
                {
                     Coin = ECoin.BTC,
                     Value = 0.505M
                }
            },
            trade.SumarizeFees(ECoin.ETH));
        }

        #endregion

        #region Sell

        [TestMethod]
        public void Sumarize_SellFeeTo()
        {
            var trade = new SellTrade()
            {
                Date = DateTime.UtcNow,
                From = new Quantity()
                {
                    Coin = ECoin.ETH,
                    Value = 10
                },
                To = new Quantity()
                {
                    Coin = ECoin.BTC,
                    Value = 1M
                },
                Fees = new Quantity[]
                {
                    new Quantity()
                    {
                         Coin= ECoin.BTC,
                         Value= 0.5M
                    }
                }
            };

            Assert.AreEqual(trade.Price, 0.1M);

            CollectionAssert.AreEqual(new Quantity[]
            {
                new Quantity()
                {
                     Coin = ECoin.ETH,
                     Value = 5M
                }
            },
            trade.SumarizeFees(trade.Fees[0].Coin));
        }

        [TestMethod]
        public void Sumarize_SellFeeFrom()
        {
            var trade = new SellTrade()
            {
                Date = DateTime.UtcNow,
                From = new Quantity()
                {
                    Coin = ECoin.ETH,
                    Value = 10M
                },
                To = new Quantity()
                {
                    Coin = ECoin.BTC,
                    Value = 1M
                },
                Fees = new Quantity[]
                {
                    new Quantity()
                    {
                         Coin= ECoin.ETH,
                         Value= 0.5M
                    }
                }
            };

            Assert.AreEqual(trade.Price, 0.1M);

            CollectionAssert.AreEqual(new Quantity[]
            {
                new Quantity()
                {
                     Coin = ECoin.BTC,
                     Value = 0.05M
                }
            },
            trade.SumarizeFees(trade.Fees[0].Coin));
        }

        #endregion

        #region Sell

        [TestMethod]
        public void Sumarize_BuyFeeTo()
        {
            var trade = new BuyTrade()
            {
                Date = DateTime.UtcNow,
                From = new Quantity()
                {
                    Coin = ECoin.ETH,
                    Value = 10M
                },
                To = new Quantity()
                {
                    Coin = ECoin.BTC,
                    Value = 0.5M
                },
                Fees = new Quantity[]
                {
                    new Quantity()
                    {
                         Coin= ECoin.ETH,
                         Value= 0.1M
                    }
                }
            };

            Assert.AreEqual(trade.Price, 20M);

            CollectionAssert.AreEqual(new Quantity[]
            {
                new Quantity()
                {
                     Coin = ECoin.BTC,
                     Value = 2M
                }
            },
            trade.SumarizeFees(trade.Fees[0].Coin));
        }

        [TestMethod]
        public void Sumarize_BuyFeeFrom()
        {
            var trade = new BuyTrade()
            {
                Date = DateTime.UtcNow,
                From = new Quantity()
                {
                    Coin = ECoin.ETH,
                    Value = 10M
                },
                To = new Quantity()
                {
                    Coin = ECoin.BTC,
                    Value = 0.5M
                },
                Fees = new Quantity[]
                {
                    new Quantity()
                    {
                         Coin= ECoin.ETH,
                         Value= 0.1M
                    }
                }
            };

            Assert.AreEqual(trade.Price, 20M);

            CollectionAssert.AreEqual(new Quantity[]
            {
                new Quantity()
                {
                     Coin = ECoin.BTC,
                     Value = 2M
                }
            },
            trade.SumarizeFees(trade.Fees[0].Coin));
        }

        #endregion
    }
}