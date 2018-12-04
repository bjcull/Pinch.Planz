using System;
using System.Collections.Generic;
using System.Text;
using NodaTime;

namespace Pinch.Planz.Evaluation
{
    public static class EvaluationHelpers
    {
        public static LocalDate AddDuration(this LocalDate date, int multiplier, Period period)
        {
            for (var i = 0; i < multiplier; i++)
            {
                date = date.Plus(period);
            }

            return date;
        }
    }
}
