using System;
using System.Collections.Generic;
using System.Text;

namespace Pinch.Planz.Expressions
{
    public class MonetaryValue : Expression    
    {
        public string CurrencySymbol { get; set; }
        public decimal Value { get; }

        public MonetaryValue(decimal value, string currencySymbol)
        {
            Value = value;
            CurrencySymbol = currencySymbol;
        }
    }
}
