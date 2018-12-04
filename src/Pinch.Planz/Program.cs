using System;
using System.Text;
using Pinch.Planz.Evaluation;
using Pinch.Planz.Parsing;
using Superpower.Model;

namespace Pinch.Planz
{
    class Program
    {
        const string Prompt = "planz> ";

        static void Main()
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;            
            Console.Write(Prompt);
            var line = Console.ReadLine();
            while (line != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    try
                    {
                        var tokens = ExpressionTokenizer.TryTokenize(line);

                        //foreach (var token in tokens.Value)
                        //{
                        //    Console.WriteLine(token);
                        //}                        

                        if (!tokens.HasValue)
                        {
                            WriteSyntaxError(tokens.ToString(), tokens.ErrorPosition);
                        }
                        else if (!ExpressionParser.TryParse(tokens.Value, out var expr, out var error, out var errorPosition))
                        {
                            WriteSyntaxError(error, errorPosition);
                        }
                        else
                        {
                            var result = ExpressionEvaluator.Evaluate(expr);
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                    }

                    Console.ResetColor();
                    Console.WriteLine();
                }

                Console.Write(Prompt);
                line = Console.ReadLine();
            }
        }

        static void WriteSyntaxError(string message, Position errorPosition)
        {
            if (errorPosition.HasValue && errorPosition.Line == 1)
                Console.WriteLine(new string(' ', Prompt.Length + errorPosition.Column - 1) + '^');
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
