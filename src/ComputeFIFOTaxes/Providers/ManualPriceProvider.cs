using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using System;

namespace ComputeFIFOTaxes.Providers
{
    public class ManualPriceProvider : FiatProviderBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Config</param>
        public ManualPriceProvider(Config.FiatProviderConfig config) : base(config) { }

        protected override decimal InternalGetFiatPrice(ITradeParser parser, ECoin coin, DateTime date) => 0M;
    }
}
