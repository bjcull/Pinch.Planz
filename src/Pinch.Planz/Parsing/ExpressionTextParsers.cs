using System;
using System.Globalization;
using NodaTime;
using Pinch.Planz.Evaluation;
using Pinch.Planz.Expressions;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Result = Pinch.Planz.Evaluation.Result;

namespace Pinch.Planz.Parsing
{
    public static class ExpressionTextParsers
    {
        public static TextParser<Period> Magnitude { get; } =
            Character.EqualTo('d').Value(Period.FromDays(1))
                .Or(Character.EqualTo('w').Value(Period.FromWeeks(1)))
                .Or(Character.EqualTo('m').Value(Period.FromMonths(1)))
                .Or(Character.EqualTo('y').Value(Period.FromYears(1)));

        public static TextParser<DurationValue> Duration { get; } =
            Numerics.IntegerInt32
                .Then(d => Magnitude.Select(m => new DurationValue(d, m)));

        public static TextParser<TrialValue> Trial { get; } =
            Duration
                .Then(d => Character.EqualTo('T').Value(new TrialValue(d)));

        public static TextParser<MonetaryValue> MonetaryValue { get; } =
            ExpressionTokenizer.MonetaryValue()
                .Select((input) =>
                {
                    var text = input.ToStringValue();
                    foreach (var cultureInfo in CultureInfo.GetCultures(CultureTypes.AllCultures))
                    {
                        if (decimal.TryParse(text, NumberStyles.Currency, cultureInfo, out var dec))
                        {

                            return new MonetaryValue(dec, cultureInfo.NumberFormat.CurrencySymbol);
                        }
                    }

                    return null;
                });
    }
}
