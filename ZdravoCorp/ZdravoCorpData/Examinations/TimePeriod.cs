using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.ZdravoCorpData.Examinations
{
    public class TimePeriod
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }

        public TimePeriod(DateTime start, DateTime end)
        {
            this.start = start;
            this.end = end;
        } 
    }
}
