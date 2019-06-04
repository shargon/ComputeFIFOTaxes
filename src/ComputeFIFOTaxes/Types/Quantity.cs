using System;

namespace ComputeFIFOTaxes.Types
{
    public class Quantity
    {
        /// <summary>
        /// Coin
        /// </summary>
        public ECoin Coin { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public Decimal Value { get; set; }

        /// <summary>
        /// Fiat price
        /// </summary>
        public Decimal? FiatPrice { get; set; }
    }
}