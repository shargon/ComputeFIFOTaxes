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

        public delegate void delOnFifoSell(Trade trade, decimal buyPrice, decimal sellPrice, decimal amount);
        public delegate void delOnFee(Trade trade, decimal fee);

        public event delOnFifoSell OnFifoSell;

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

            while (Sell(trade, fifo) && fifo.Amount > 0)
            {

            }
        }

        /// <summary>
        /// Sell fifo
        /// </summary>
        /// <param name="trade">Trade</param>
        /// <param name="sell">Sell</param>
        /// <returns>Return true if was sold</returns>
        private bool Sell(Trade trade, FIFOInternal sell)
        {
            if (_list.Count <= 0)
            {
                OnFifoSell?.Invoke(trade, 0, sell.Price, sell.Amount);
                return false;
            }

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

            OnFifoSell?.Invoke(trade, buy.Price, sell.Price, amount);

            return true;
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
