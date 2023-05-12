using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZdravoCorp.ZdravoCorpData.Examinations
{
    public class TimeSlot
    {
        public DateTime start { get; set; }
        public Duration duration { get; set; }

        public TimeSlot(DateTime start, Duration duration)
        {
            this.start = start;
            this.duration = duration;
        }

        public bool overlapsWithTimeSlot(TimeSlot timeSlot)
        {
            if(timeSlot == null) return true;

            return timeSlot.start == this.start;
        }
    }
}
