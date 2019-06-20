using ComputeFIFOTaxes.Types;
using System;
using System.Collections.Generic;

namespace ComputeFIFOTaxes.Helpers
{
    public static class FiatProviderHelper
    {
        class FiatResult
        {
            public string Base { get; set; }
            public string Date { get; set; }
            public Dictionary<string, decimal> Rates { get; set; }
        }

        /// <summary>
        /// USD per Coin
        /// </summary>
        /// <param name="fiatCoin">Fiat coin</param>
        /// <param name="date">Date</param>
        public static decimal UsdPerCoin(ECoin fiatCoin, DateTime date)
        {
            var down = DownloadHelper.Download<FiatResult>($"https://api.exchangeratesapi.io/{date.ToString("yyyy-MM-dd")}?base=USD", true);

            if (down != null && down.Rates.TryGetValue(fiatCoin.ToString(), out var value))
            {
                return value;
            }

            throw new ArgumentException();
        }
    }
}