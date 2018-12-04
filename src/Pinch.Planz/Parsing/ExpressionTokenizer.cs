using System;
using System.Globalization;
using System.Linq;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Pinch.Planz.Parsing
{
    public static class ExpressionTokenizer
    {
        public static TextParser<TextSpan> Magnitude { get; } =
            Span.Length(1)
                .And(Span.WithAll(x => char.IsLetter(x) && char.IsLower(x)));

        public static TextParser<TextSpan> Duration { get; } =
            Numerics.Integer
                .Then(_ => Span.WithAll(x => char.IsLetter(x) && x != 'T'));

        public static TextParser<char> Trial { get; } =
            Duration.Then(_ => Character.EqualTo('T'));

        public static TextParser<TextSpan> MonetaryValue()
        {
            return (TextParser<TextSpan>) (input =>
            {
                Result<char> result = input.ConsumeChar();
                while (result.HasValue && !char.IsWhiteSpace(result.Value) && !"+-*/()".Contains(result.Value))
                    result = result.Remainder.ConsumeChar();
                if (!(result.Location == input))
                {
                    var text = input.Until(result.Location).ToStringValue();
                    if (text.All(char.IsDigit))
                    {
                        return Result.Empty<TextSpan>(input);
                    }

                    foreach (var cultureInfo in CultureInfo.GetCultures(CultureTypes.AllCultures))
                    {
                        if (decimal.TryParse(text, NumberStyles.Currency, cultureInfo, out var dec))
                        {
                            return Result.Value<TextSpan>(input.Until(result.Location), input, result.Location);
                        }
                    }
                }

                return Result.Empty<TextSpan>(input);
            });
        }

        private static Tokenizer<ExpressionToken> Tokenizer { get; } = new TokenizerBuilder<ExpressionToken>()            
            .Match(Character.EqualTo('+'), ExpressionToken.Plus)
            .Match(Character.EqualTo('-'), ExpressionToken.Minus)
            .Match(Character.EqualTo('*'), ExpressionToken.Asterisk)
            .Match(Character.EqualTo('/'), ExpressionToken.Slash)            
            .Match(MonetaryValue(), ExpressionToken.MonetaryAmount, requireDelimiters: true)            
            .Match(Trial, ExpressionToken.Trial, requireDelimiters: false)
            .Match(Duration, ExpressionToken.Duration, requireDelimiters: true)
            .Match(Magnitude, ExpressionToken.Magnitude, requireDelimiters: true)
            .Match(Numerics.Decimal, ExpressionToken.Number, requireDelimiters: true)
            .Match(Character.EqualTo('('), ExpressionToken.LParen)
            .Match(Character.EqualTo(')'), ExpressionToken.RParen)
            .Ignore(Span.WhiteSpace)
            .Build();

        public static Result<TokenList<ExpressionToken>> TryTokenize(string source)
        {
            return Tokenizer.TryTokenize(source);
        }
    }
}
