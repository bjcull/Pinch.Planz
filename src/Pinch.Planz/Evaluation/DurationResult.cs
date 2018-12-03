using System;
using NodaTime;

namespace Pinch.Planz.Evaluation
{
    public class DurationResult : Result
    {
        public int Value { get; set; }
        public Period Magnitude { get; set; }        

        public DurationResult(int value, Period magnitude)
        {
            Value = value < 0 ? 0 : value;
            Magnitude = magnitude;
        }

        public override string ToString()
        {
            return $"{Value}{Magnitude}";
        }
    }
}