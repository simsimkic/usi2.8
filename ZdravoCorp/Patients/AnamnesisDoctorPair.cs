using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Patients
{
    public class AnamnesisDoctorPair
    {
        public Anamnesis anamnesis { get; set; }
        public Doctor doctor { get; set; }

        public AnamnesisDoctorPair(Anamnesis anamnesis, Doctor doctor)
        {
            this.anamnesis = anamnesis;
            this.doctor = doctor;
        }
    }
}
