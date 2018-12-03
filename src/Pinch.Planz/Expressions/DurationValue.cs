using System;
using NodaTime;

namespace Pinch.Planz.Expressions
{
    public class DurationValue : Expression
    {
        public int Value { get; set; }
        public Period Magnitude { get; set; }

        public DurationValue(int value, Period magnitude)
        {
            Value = value;
            Magnitude = magnitude;
        }
    }
}

