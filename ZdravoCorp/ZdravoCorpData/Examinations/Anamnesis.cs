using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Nurses;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.ZdravoCorpData.Examinations
{
    public class Anamnesis
    {
        public string observation { get; set; }
        public DateOnly date { get; set; }
        public int doctorId { get; set; }
        public Anamnesis(string observation,DateOnly dateOnly,int doctorId)
        {
            this.observation = observation;
            this.date = dateOnly;
            this.doctorId = doctorId;
        }

        public static void Create(DateOnly dateOfMHE, string observation,int patientId,int doctorId) {

            Anamnesis newAnamnesis = new Anamnesis(observation, dateOfMHE, doctorId);
            UpdateMedicalHistory(newAnamnesis,patientId);
        
        }

        public static void UpdateMedicalHistory(Anamnesis newAnamnesis,int patientId) {
            MedicalRecord.addAnamnesis(newAnamnesis, patientId);
        }

    }
}

