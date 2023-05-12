using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.ZdravoCorpData.Drugs
{
    public class Prescription
    {
        public List<Drug> drugs;

        public Prescription(List<Drug> drugs)
        {
            this.drugs = drugs;
        }
    }
}
