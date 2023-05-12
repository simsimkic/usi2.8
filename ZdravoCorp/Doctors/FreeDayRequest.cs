using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Doctors
{
    public class FreeDayRequest
    {

        public DateOnly start { get; set; }
        public DateOnly end { get; set; }

        public string reason { get; set; }

        public FreeDayRequest(DateOnly start, DateOnly end, string reason)
        {
            this.start = start;
            this.end = end;
            this.reason = reason;
        }
    }
}
