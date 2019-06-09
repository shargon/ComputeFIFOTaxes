using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace ComputeFIFOTaxes.Types
{
    public class Quantity : IEquatable<Quantity>
    {
        /// <summary>
        /// Coin
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ECoin Coin { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public decimal Value { get; set; } = 0M;

        /// <summary>
        /// Return if is equal
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Return true if is equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is Quantity q) return Equals(q);
            return false;
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode() => (int)Value + (int)Coin;

        /// <summary>
        /// Check if is Equal
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if is equal</returns>
        public bool Equals(Quantity other)
        {
            return other != null && other.Coin == Coin && other.Value == Value;
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}