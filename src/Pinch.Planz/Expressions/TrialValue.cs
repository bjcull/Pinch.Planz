using System;
using System.Collections.Generic;
using System.Text;

namespace Pinch.Planz.Expressions
{
    public class TrialValue : Expression
    {
        public DurationValue Duration { get; set; }

        public TrialValue(DurationValue duration)
        {
            Duration = duration;
        }
    }
}
