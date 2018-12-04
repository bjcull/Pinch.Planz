using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime;
using Pinch.Planz.Expressions;

namespace Pinch.Planz.Evaluation
{
    public static class ExpressionEvaluator
    {
        public static Result Evaluate(Expression expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            switch (expression)
            {
                case DurationValue duration:
                    return new DurationResult(duration.Value, duration.Magnitude);
                case NumericValue numeric:
                    return new NumericResult(numeric.Value);
                case MonetaryValue monetaryValue:
                    return new PaymentResult(monetaryValue.Value, monetaryValue.CurrencySymbol, CurrentDate());
                case MagnitudeValue magnitude:
                    return new MagnitudeResult(magnitude.Magnitude);
                case TrialValue trial:
                    return new TrialResult(new DurationResult(trial.Duration.Value, trial.Duration.Magnitude), CurrentDate());
                    //return new PaymentResult(0, "", CurrentDate())
                    //{
                    //    IsTrial = true,
                    //    Period = trial.Duration.Magnitude                        
                    //};
                case BinaryExpression binary:
                    return DispatchOperator(Evaluate(binary.Left), Evaluate(binary.Right), binary.Operator);
                default:
                    throw new ArgumentException($"Unsupported expression {expression}.");
            }
        }

        static Result DispatchOperator(Result left, Result right, Operator @operator)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            switch (@operator)
            {
                case Operator.Add:
                    return DispatchAdd(left, right);
                case Operator.Subtract:
                    return DispatchSubtract(left, right);
                case Operator.Multiply:
                    return DispatchMultiply(left, right);
                case Operator.Divide:
                    return DispatchDivide(left, right);
                default:
                    throw new ArgumentException($"Unsupported operator {@operator}.");
            }
        }

        static Result DispatchAdd(Result left, Result right)
        {
            //if (left is DurationResult dl1 && right is DurationResult dr1)
            //    return new DurationResult(dl.Value + dr.Value);

            if (left is NumericResult ln && right is NumericResult rn)
                return new NumericResult(ln.Value + rn.Value);

            if (left is DurationResult d2 && right is PaymentResult p2)
                return AddPaymentAndDuration(d2, p2);

            if (left is PaymentResult p3 && right is DurationResult d3)
                return AddPaymentAndDuration(d3, p3);

            if (left is PaymentResult pl4 && right is PaymentResult pr4)
                return new PlanResult(pl4, pr4);

            if (left is PaymentResult pl5 && right is PlanResult pr5)
                return new PlanResult(pr5.Payments.Concat(new List<PaymentResult>() {pl5}).ToArray());

            if (left is PlanResult pl6 && right is PaymentResult pr6)
                return new PlanResult(pl6.Payments.Concat(new List<PaymentResult>() {pr6}).ToArray());

            if (left is PlanResult pl7 && right is DurationResult d7)
                return AddPlanAndDuration(pl7, d7);

            if (left is DurationResult d8 && right is PlanResult pr8)
                return AddPlanAndDuration(pr8, d8);

            if (left is TrialResult t9 && right is PaymentResult pr9)
            {
                var trialPayment = new PaymentResult(t9);
                pr9.PaymentDate = pr9.PaymentDate.AddDuration(t9.DurationResult.Value, t9.DurationResult.Magnitude);
                return new PlanResult(trialPayment, pr9);
            }
            
            if (left is PaymentResult pl10 && right is TrialResult t10)
            {
                var trialPayment = new PaymentResult(t10);
                pl10 = AddPaymentAndDuration(t10.DurationResult, pl10);
                return new PlanResult(trialPayment, pl10);
            }
            
            if (left is TrialResult t11 && right is PlanResult pr11)
            {
                var trialPayment = new PaymentResult(t11);
                pr11 = AddPlanAndDuration(pr11, t11.DurationResult);
                pr11.Payments.Add(trialPayment);
                return pr11;
            }

            if (left is PlanResult pr12 && right is TrialResult t12)
            {
                var trialPayment = new PaymentResult(t12);
                pr12 = AddPlanAndDuration(pr12, t12.DurationResult);
                pr12.Payments.Add(trialPayment);
                return pr12;
            }

            throw new EvaluationException($"Values {left} and {right} cannot be added.");
        }

        private static PlanResult AddPlanAndDuration(PlanResult planResult, DurationResult durationResult)
        {
            planResult.Payments.ForEach(x =>
                x.PaymentDate = x.PaymentDate.AddDuration(durationResult.Value, durationResult.Magnitude));
            return planResult;
        }

        private static PaymentResult AddPaymentAndDuration(DurationResult durationResult, PaymentResult paymentResult)
        {
            paymentResult.PaymentDate = paymentResult.PaymentDate.AddDuration(durationResult.Value, durationResult.Magnitude);
            return paymentResult;
        }

        static Result DispatchSubtract(Result left, Result right)
        {
            //if (left is DurationResult dl && right is DurationResult dr)
            //    return new DurationResult(dl.Value - dr.Value);

            if (left is NumericResult ln && right is NumericResult rn)
                return new NumericResult(ln.Value - rn.Value);

            throw new EvaluationException($"Value {right} cannot be subtracted from {left}.");
        }

        static Result DispatchMultiply(Result left, Result right)
        {
            if (left is NumericResult ln && right is NumericResult rn)
                return new NumericResult(ln.Value * rn.Value);

            //if (left is DurationResult dl && right is NumericResult nr)
            //    return new DurationResult(dl.Value * nr.Value);

            //if (left is NumericResult nl && right is DurationResult dr)
            //    return new DurationResult(nl.Value * dr.Value);

            if (left is PaymentResult ml && right is NumericResult mr)
                return new PaymentResult(ml.Amount * Convert.ToDecimal(mr.Value), ml.CurrencySymbol, CurrentDate());

            if (left is NumericResult n2 && right is PaymentResult m2)
                return new PaymentResult(m2.Amount * Convert.ToDecimal(n2.Value), m2.CurrencySymbol, CurrentDate());

            if (left is PaymentResult m3 && right is MagnitudeResult mv3)
            {
                return new PaymentResult(m3.Amount, m3.CurrencySymbol, CurrentDate())
                {
                    IsRepeating = true,
                    Period = mv3.Magnitude
                };
            }

            if (left is PaymentResult m4 && right is DurationResult d4)
            {
                var plan = new PlanResult();
                for (var i = 0; i < d4.Value; i++)
                {
                    var date = CurrentDate();
                    for (var y = 0; y < i; y++)
                    {
                        date = date.Plus(d4.Magnitude);
                    }

                    plan.Payments.Add(new PaymentResult(m4.Amount, m4.CurrencySymbol, date)
                    {
                        Period = d4.Magnitude
                    });
                }

                return plan;
            }


            throw new EvaluationException($"Values {left} and {right} cannot be multiplied.");
        }

        static Result DispatchDivide(Result left, Result right)
        {
            if (left is NumericResult ln && right is NumericResult rn)
                return new NumericResult(ln.Value / rn.Value);

            //if (left is DurationResult dl && right is NumericResult nr)
            //    return new DurationResult(dl.Value / nr.Value);

            //if (left is DurationResult dl2 && right is DurationResult dr)
            //    return new NumericResult(dl2.Value / dr.Value);

            if (left is PaymentResult m4 && right is DurationResult d4)
            {
                var plan = new PlanResult();
                for (var i = 0; i < d4.Value; i++)
                {
                    var date = CurrentDate();
                    for (var y = 0; y < i; y++)
                    {
                        date = date.Plus(d4.Magnitude);
                    }

                    plan.Payments.Add(new PaymentResult(m4.Amount / d4.Value, m4.CurrencySymbol, date)
                    {
                        Period = d4.Magnitude
                    });
                }

                return plan;
            }

            throw new EvaluationException($"Value {left} cannot be divided by {right}.");
        }

        static LocalDate CurrentDate()
        {
            return SystemClock.Instance.GetCurrentInstant().InUtc().Date;
        }
    }
}
