using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinch.Planz.Evaluation
{
    public class PlanResult : Result
    {
        public List<PaymentResult> Payments { get; set; } = new List<PaymentResult>();

        public PlanResult()
        {            
        }

        public PlanResult(params PaymentResult[] payments)
        {
            Payments = payments.ToList();
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Payments.OrderBy(x => x.PaymentDate).Select(x => x.ToString()));
        }
    }
}
