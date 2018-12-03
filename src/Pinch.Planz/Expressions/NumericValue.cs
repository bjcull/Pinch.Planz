﻿namespace Pinch.Planz.Expressions
{
    public class NumericValue : Expression
    {
        public double Value { get; }

        public NumericValue(double value)
        {
            Value = value;
        }
    }
}