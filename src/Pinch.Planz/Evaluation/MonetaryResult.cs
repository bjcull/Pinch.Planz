using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Pinch.Planz.Evaluation
{
    public class MonetaryResult : Result
    {
        public string CurrencySymbol { get; }
        public decimal Value { get; }

        private readonly CultureInfo _inferredCulture;

        public MonetaryResult(decimal value, string currencySymbol)
        {
            Value = value;
            CurrencySymbol = currencySymbol;
            _inferredCulture = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .FirstOrDefault(x => x.NumberFormat.CurrencySymbol == currencySymbol);
        }

        public override string ToString()
        {
            return Value.ToString("C", _inferredCulture);
        }
    }
}
