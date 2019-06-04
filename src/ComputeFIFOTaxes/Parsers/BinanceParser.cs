using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputeFIFOTaxes.Parsers
{
    public class BinanceParser : IParser
    {
        public IEnumerable<Trade> GetTrades(TradeData data)
        {
            throw new NotImplementedException();
        }

        public bool IsThis(TradeData data)
        {
            var first = data.Data.FirstOrDefault();

            return first != null && 
                string.Concat(",", first) == "Date(UTC),Market,Type,Price,Amount,Total,Fee,Fee Coin";
        }
    }
}