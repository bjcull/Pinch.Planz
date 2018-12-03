using Superpower.Display;

namespace Pinch.Planz.Parsing
{
    public enum ExpressionToken
    {
        Number,

        Magnitude,

        Duration,

        MonetaryAmount,

        [Token(Example = "+")]
        Plus,

        [Token(Example = "-")]
        Minus,

        [Token(Example = "*")]
        Asterisk,

        [Token(Example = "/")]
        Slash,

        [Token(Example = "(")]
        LParen,

        [Token(Example = ")")]
        RParen
    }
}