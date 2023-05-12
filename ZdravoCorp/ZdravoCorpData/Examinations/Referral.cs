using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.ZdravoCorpData.Examinations
{
    public class Referral
    {

        public bool isInternist { get; set; }

        public Referral(bool isInternist)
        {
            this.isInternist = isInternist;
        }   
    }
}
