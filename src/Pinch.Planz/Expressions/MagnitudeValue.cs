using System;
using System.Collections.Generic;
using System.Text;
using NodaTime;

namespace Pinch.Planz.Expressions
{
    public class MagnitudeValue : Expression
    {
        public Period Magnitude { get; set; }

        public MagnitudeValue(Period magnitude)
        {
            Magnitude = magnitude;
        }
    }
}
