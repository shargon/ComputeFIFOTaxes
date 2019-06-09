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
            trades.Sort((a, b) => a.Date.CompareTo(b.Date));
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
                    trade.FiatCostWithoutFees = trade.ChooseFiatPriceForTrade(provider, trade.Date);
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
            return provider.GetFiatPrice(trade.Exchange, fee.Coin, trade.Date).Max * fee.Value;
        }

        /// <summary>
        /// Choose the best price for this trade
        /// </summary>
        /// <param name="trade">Trade</param>
        /// <param name="provider">Provider</param>
        /// <param name="date">date</param>
        /// <returns>Decimal value</returns>
        public static decimal ChooseFiatPriceForTrade(this Trade trade, FiatProviderBase provider, DateTime date)
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
                if (trade.FromOrToIs(market))
                {
                    var price = provider.GetFiatPrice(trade.Exchange, market, date);
                    price = price.Plus(trade.Price);

                    trade.Fees = trade.SumarizeFees(trade.From.Coin == market ? trade.To.Coin : trade.From.Coin);
                    return trade is SellTrade ? price.Min : price.Max * trade.From.Value;
                }
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
                    ret[x] = new Quantity()
                    {
                        Coin = trade.From.Coin == from ? trade.To.Coin : trade.From.Coin,
                        Value = trade.From.Coin == from ? trade.Fees[x].Value * price : trade.Fees[x].Value / price
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