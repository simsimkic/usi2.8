using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.ZdravoCorpData.Examinations
{
    public class Admission
    {
        public int examinationId { get; set; }
        public bool admission { get; set; }

        public Admission(int examinationId, bool admission)
        {
            this.examinationId = examinationId;
            this.admission = admission;
        }   
    }
}
