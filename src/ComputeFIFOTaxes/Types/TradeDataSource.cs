using System.Collections.Generic;

namespace ComputeFIFOTaxes.Types
{
    public class TradeDataSource
    {
        /// <summary>
        /// Variables for parsing
        /// </summary>
        internal Dictionary<string, object> Variables { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Data
        /// </summary>
        public TradeData[] Data { get; set; }
    }
}