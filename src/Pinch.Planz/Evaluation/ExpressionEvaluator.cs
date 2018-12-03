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
                    return new MonetaryResult(monetaryValue.Value, monetaryValue.CurrencySymbol);
                case MagnitudeValue magnitude:
                    return new MagnitudeResult(magnitude.Magnitude);
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
            //if (left is DurationResult dl && right is DurationResult dr)
            //    return new DurationResult(dl.Value + dr.Value);

            if (left is NumericResult ln && right is NumericResult rn)
                return new NumericResult(ln.Value + rn.Value);

            if (left is DurationResult d2 && right is PaymentResult p2)
            {
                for (var i = 0; i < d2.Value; i++)
                {
                    p2.PaymentDate = p2.PaymentDate.Plus(d2.Magnitude);
                }

                return p2;
            }

            throw new EvaluationException($"Values {left} and {right} cannot be added.");
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

            if (left is MonetaryResult ml && right is NumericResult mr)
                return new MonetaryResult(ml.Value * Convert.ToDecimal(mr.Value), ml.CurrencySymbol);

            if (left is NumericResult n2 && right is MonetaryResult m2)
                return new MonetaryResult(m2.Value * Convert.ToDecimal(n2.Value), m2.CurrencySymbol);

            if (left is MonetaryResult m3 && right is MagnitudeResult mv3)
            {
                return new PaymentResult()
                {
                    Amount = m3.Value,
                    CurrencySymbol = m3.CurrencySymbol,
                    IsRepeating = true,
                    PaymentDate = CurrentDate(),
                    Period = mv3.Magnitude
                };
            }

            if (left is MonetaryResult m4 && right is DurationResult d4)
            {
                var plan = new PlanResult();
                for (var i = 0; i < d4.Value; i++)
                {
                    var date = CurrentDate();
                    for (var y = 0; y < i; y++)
                    {
                        date = date.Plus(d4.Magnitude);
                    }

                    plan.Payments.Add(new PaymentResult()
                    {
                        Amount = m4.Value,
                        CurrencySymbol = m4.CurrencySymbol,
                        Period = d4.Magnitude,
                        PaymentDate = date
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

            if (left is MonetaryResult m4 && right is DurationResult d4)
            {
                var plan = new PlanResult();
                for (var i = 0; i < d4.Value; i++)
                {
                    var date = CurrentDate();
                    for (var y = 0; y < i; y++)
                    {
                        date = date.Plus(d4.Magnitude);
                    }

                    plan.Payments.Add(new PaymentResult()
                    {
                        Amount = (m4.Value / d4.Value),
                        CurrencySymbol = m4.CurrencySymbol,
                        Period = d4.Magnitude,
                        PaymentDate = date
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
