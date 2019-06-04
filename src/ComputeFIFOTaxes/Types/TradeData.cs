using System.Collections.Generic;

namespace ComputeFIFOTaxes.Types
{
    public class TradeData
    {
        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        public List<object[]> Data { get; set; }
    }
}