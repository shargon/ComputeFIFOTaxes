using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ComputeFIFOTaxes.Types
{
    public class FIFO
    {
        class FIFOInternal
        {
            /// <summary>
            /// Date
            /// </summary>
            public DateTime Date;

            /// <summary>
            /// Amount
            /// </summary>
            public decimal Amount;

            /// <summary>
            /// Price
            /// </summary>
            public decimal Price;

            /// <summary>
            /// String representation
            /// </summary>
            /// <returns>Json string</returns>
            public override string ToString() => JsonConvert.SerializeObject(this);
        }

        private readonly Queue<FIFOInternal> _list = new Queue<FIFOInternal>();

        public delegate void delOnFifoSell(DateTime date, decimal buyValue, decimal sellValue, decimal amount);

        public event delOnFifoSell OnFifoSell;

        /// <summary>
        /// Fiat beneficts
        /// </summary>
        public decimal FiatBeneficts { get; private set; } = 0M;

        /// <summary>
        /// Coin
        /// </summary>
        public ECoin Coin { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coin">Coin</param>
        public FIFO(ECoin coin)
        {
            Coin = coin;
        }

        /// <summary>
        /// Add buy
        /// </summary>
        /// <param name="trade">Trade</param>
        public void AddBuy(Trade trade)
        {
            var fifo = new FIFOInternal()
            {
                Date = trade.Date,
                Amount = trade.To.Value,
                Price = trade.FiatCostWithoutFees.Value / trade.To.Value
            };

            _list.Enqueue(fifo);
        }

        /// <summary>
        /// Add sell
        /// </summary>
        /// <param name="trade">Trade</param>
        public void AddSell(Trade trade)
        {
            var fifo = new FIFOInternal()
            {
                Date = trade.Date,
                Amount = trade.From.Value,
                Price = trade.FiatCostWithoutFees.Value / trade.From.Value
            };

            while (Sell(fifo) && fifo.Amount > 0)
            {

            }
        }

        /// <summary>
        /// Sell fifo
        /// </summary>
        /// <param name="sell">Sell</param>
        /// <returns>Return true if was sold</returns>
        private bool Sell(FIFOInternal sell)
        {
            if (_list.Count <= 0) return false;

            decimal amount;
            var buy = _list.Peek();

            if (buy.Amount >= sell.Amount)
            {
                amount = sell.Amount;
                buy.Amount -= sell.Amount;
                sell.Amount = 0;
            }
            else
            {
                amount = buy.Amount;
                sell.Amount -= buy.Amount;
                buy.Amount = 0;
            }

            // Burn fifo

            if (buy.Amount <= 0)
            {
                _list.Dequeue();
            }

            FiatBeneficts += (amount * sell.Price) - (amount * buy.Price);
            OnFifoSell?.Invoke(sell.Date, amount * buy.Price, amount * sell.Price, amount);

            return true;
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}