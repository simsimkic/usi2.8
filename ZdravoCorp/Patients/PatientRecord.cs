using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Patients
{
    public class PatientRecord
    {
        public int patientId { get; set; }
        public DateTime modifiedDate { get; set; }
        public string changeDescription { get; set; }

        public PatientRecord(int patientId, DateTime modifiedDate, string changeDescription)
        {
            this.patientId = patientId;
            this.modifiedDate = modifiedDate;
            this.changeDescription = changeDescription;
        }
    }
}
