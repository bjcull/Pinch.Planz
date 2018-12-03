using System;
using NodaTime;

namespace Pinch.Planz.Evaluation
{
    public class MagnitudeResult : Result
    {
        public Period Magnitude { get; set; }

        public MagnitudeResult(Period magnitude)
        {
            Magnitude = magnitude;
        }

        public override string ToString()
        {
            return Magnitude.ToString();
        }
    }
}
