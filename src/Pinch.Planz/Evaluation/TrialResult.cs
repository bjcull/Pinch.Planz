using System;
using System.Collections.Generic;
using System.Text;
using NodaTime;

namespace Pinch.Planz.Evaluation
{
    public class TrialResult : Result
    {
        public DurationResult DurationResult { get; set; }
        public LocalDate Date { get; set; }

        public TrialResult(DurationResult duration, LocalDate date)
        {
            DurationResult = duration;
            Date = date;
        }

        public override string ToString()
        {
            return $"Trial starting {Date:yyyy-MM-dd} for {DurationResult.Value} {DurationResult.Magnitude}";
        }
    }
}
