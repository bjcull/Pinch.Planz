using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinch.Planz.Evaluation
{
    public class PlanResult : Result
    {
        public List<PaymentResult> Payments { get; set; } = new List<PaymentResult>();

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Payments.Select(x => x.ToString()));
        }
    }
}
