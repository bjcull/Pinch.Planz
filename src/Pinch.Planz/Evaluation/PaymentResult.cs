using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NodaTime;

namespace Pinch.Planz.Evaluation
{
    public class PaymentResult : Result
    {
        public LocalDate PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsTrial { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool IsRepeating { get; set; }
        public Period Period { get; set; }

        public PaymentResult(decimal amount, string currencySymbol, LocalDate paymentDate)
        {
            Amount = amount;
            CurrencySymbol = currencySymbol;
            PaymentDate = paymentDate;
            Period = Period.Zero;
        }

        public override string ToString()
        {
            if (IsTrial)
            {
                return $"Trial starting {PaymentDate:yyyy-MM-dd}";
            }
            
            var inferredCulture = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .FirstOrDefault(x => x.NumberFormat.CurrencySymbol == CurrencySymbol);

                
            var text = $"Payment due on {PaymentDate:yyyy-MM-dd} for {Amount.ToString("C", inferredCulture)}";
            
            if (DiscountAmount > 0)
            {
                text += $" includes discount of {DiscountAmount.ToString("C", inferredCulture)}";
            }

            if (IsRepeating)
            {
                text += $" repeating indefinitely each {Period}";
            }

            return text;
        }
    }
}
