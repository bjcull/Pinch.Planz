using System;
using System.Threading;
using Superpower;
using Superpower.Model;

namespace Pinch.Planz.Parsing
{
    static class ParserExtensions
    {
        static int _instance;

        public static TextParser<T> Log<T>(this TextParser<T> parser, string name)
        {
            var id = Interlocked.Increment(ref _instance);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{id}] Constructing instance of {name}");
            Console.ResetColor();
            return i =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{id}] Invoking with input: {i}");
                Console.ResetColor();
                var result = parser(i);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{id}] Result: {result}");
                Console.ResetColor();
                return result;
            };
        }

        public static TokenListParser<TKind, T> Log<TKind, T>(this TokenListParser<TKind, T> parser, string name)
        {
            var id = Interlocked.Increment(ref _instance);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{id}] Constructing instance of {name}");
            Console.ResetColor();
            return i =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{id}] Invoking with input: {i}");
                Console.ResetColor();
                var result = parser(i);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{id}] Result: {result}");
                Console.ResetColor();
                return result;
            };
        }


        public static TextParser<U> And<T, U>(this TextParser<T> first, TextParser<U> second)
        {
            if (first == null)
                throw new ArgumentNullException(nameof (first));
            if (second == null)
                throw new ArgumentNullException(nameof (second));
            return (TextParser<U>) (input =>
            {
                Result<T> result1 = first(input);
                if (!result1.HasValue)
                    return Result.CastEmpty<T, U>(result1);
                Result<U> result2 = second(input);
                if (!result2.HasValue)
                    return result2;
                return Result.Value<U>(result2.Value, input, result2.Remainder);
            });
        }
    }
}
