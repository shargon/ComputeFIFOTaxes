using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputeFIFOTaxes.Parsers
{
    public class KrakenParser : IParser
    {
        public IEnumerable<Trade> GetTrades(TradeData data)
        {
            throw new NotImplementedException();
        }

        public bool IsThis(TradeData data)
        {
            var first = data.Data.FirstOrDefault();

            return first != null &&
                string.Concat(",", first) == "txid,refid,time,type,aclass,asset,amount,fee,balance";
        }
    }
}