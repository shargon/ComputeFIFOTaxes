using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputeFIFOTaxes.Helpers
{
    public static class TradeHelper
    {
        /// <summary>
        /// Available markets
        /// </summary>
        public static readonly ECoin[] AvailableMarkets = new ECoin[] { ECoin.BTC, ECoin.ETH, ECoin.USDT, ECoin.BNB };

        /// <summary>
        /// Sort by date
        /// </summary>
        /// <param name="trades">Trades</param>
        public static void SortByDate(this List<Trade> trades)
        {
            trades.Sort((a, b) =>
            {
                var ret = a.Date.CompareTo(b.Date);
                return ret == 0 ? ((byte)a.Type).CompareTo((byte)b.Type) : ret;
            });
        }

        /// <summary>
        /// Compute fiat values
        /// </summary>
        /// <param name="trades">Trades</param>
        /// <param name="provider">Provider</param>
        public static void ComputeFiatValues(this IEnumerable<Trade> trades, FiatProviderBase provider)
        {
            foreach (var trade in trades)
            {
                if (!trade.FiatCostWithoutFees.HasValue)
                {
                    trade.FiatCostWithoutFees = trade.ChooseFiatValueForTrade(provider, trade.Date);
                }

                if (!trade.FiatFees.HasValue)
                {
                    trade.FiatFees = 0;

                    foreach (var fee in trade.Fees)
                    {
                        trade.FiatFees += trade.ChooseFiatPriceForFee(provider, fee);
                    }
                }
            }
        }

        /// <summary>
        /// Choose the best price for this trade
        /// </summary>
        /// <param name="provider">Provider</param>
        /// <param name="fee">Trade</param>
        /// <param name="trade">Trade</param>
        /// <returns>Decimal value</returns>
        public static decimal ChooseFiatPriceForFee(this Trade trade, FiatProviderBase provider, Quantity fee)
        {
            return provider.GetFiatPrice(trade.Exchange, fee.Coin, trade.Date) * fee.Value;
        }

        /// <summary>
        /// Compute fifo
        /// </summary>
        /// <param name="trades">Trades</param>
        /// <param name="fifoCollection">Collection</param>
        /// <param name="onFees">On fee</param>
        /// <param name="onFifoSell">On fifo sell</param>
        public static void ComputeFifo(this IEnumerable<Trade> trades, out Dictionary<ECoin, FIFO> fifoCollection, Action<DateTime, decimal> onFees, FIFO.delOnFifoSell onFifoSell)
        {
            fifoCollection = new Dictionary<ECoin, FIFO>();

            foreach (var trade in trades)
            {
                if (trade.FiatFees.HasValue)
                {
                    onFees?.Invoke(trade.Date, trade.FiatFees.Value);
                }

                if (trade is BuyTrade)
                {
                    if (!fifoCollection.TryGetValue(trade.To.Coin, out var fifo))
                    {
                        fifoCollection[trade.To.Coin] = fifo = new FIFO(trade.To.Coin);
                        fifo.OnFifoSell += onFifoSell;
                    }

                    fifo.AddBuy(trade);
                }
                else
                {
                    if (!fifoCollection.TryGetValue(trade.From.Coin, out var fifo))
                    {
                        fifoCollection[trade.From.Coin] = fifo = new FIFO(trade.From.Coin);
                        fifo.OnFifoSell += onFifoSell;
                    }

                    fifo.AddSell(trade);
                }
            }
        }

        /// <summary>
        /// Choose the best fiat value for this trade
        /// </summary>
        /// <param name="trade">Trade</param>
        /// <param name="provider">Provider</param>
        /// <param name="date">date</param>
        /// <returns>Decimal value</returns>
        public static decimal ChooseFiatValueForTrade(this Trade trade, FiatProviderBase provider, DateTime date)
        {
            // Check same as provider

            if (trade.From.Coin == provider.Coin)
            {
                trade.Fees = trade.SumarizeFees(trade.From.Coin == provider.Coin ? trade.To.Coin : trade.From.Coin);
                return trade.From.Value;
            }
            else if (trade.To.Coin == provider.Coin)
            {
                trade.Fees = trade.SumarizeFees(trade.From.Coin == provider.Coin ? trade.To.Coin : trade.From.Coin);
                return trade.To.Value;
            }

            // Check BTC first

            foreach (var market in AvailableMarkets)
            {
                if (!trade.FromOrToIs(market)) continue;

                var price = provider.GetFiatPrice(trade.Exchange, market, date);

                price *= (trade.From.Coin == market ? trade.From.Value : trade.To.Value);
                trade.Fees = trade.SumarizeFees(trade.From.Coin == market ? trade.To.Coin : trade.From.Coin);

                return price;
            }

            throw new ArgumentException("Market not found");
        }

        /// <summary>
        /// Convert fees for save price checks
        /// </summary>
        /// <param name="trade">Trade</param>
        /// <param name="from">Coin to be converted into the other from the pair</param>
        /// <returns>Return unified fees</returns>
        public static Quantity[] SumarizeFees(this Trade trade, ECoin from)
        {
            var price = Math.Abs(trade.Price);
            var ret = new Quantity[trade.Fees.Length];

            for (int x = 0, max = ret.Length; x < max; x++)
            {
                if (trade.Fees[x].Coin == from)
                {
                    var isFrom = trade.From.Coin == from;
                    var value = trade.Fees[x].Value;

                    if (trade is BuyTrade)
                    {
                        value = !isFrom ? value * price : value / price;
                    }
                    else if (trade is SellTrade)
                    {
                        value = isFrom ? value * price : value / price;
                    }
                    else throw new ArgumentException();

                    ret[x] = new Quantity()
                    {
                        Coin = isFrom ? trade.To.Coin : trade.From.Coin,
                        Value = value
                    };
                }
                else
                {
                    ret[x] = trade.Fees[x];
                }
            }

            // Join Fees

            return ret.GroupBy(u => u.Coin, p => p.Value, (key, val) => new Quantity() { Coin = key, Value = val.Sum() }).ToArray();
        }
    }
}