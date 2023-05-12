using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Nurses
{
    public class MedicalRecord
    {
        public int id { get; set; }
        public int height { get; set; }
        public int weight { get; set; }
        public List<Anamnesis> medicalHistory { get; set; }   //history of previous illnesses
        public List<Referral> referrals { get; set; }
        public string allergens { get; set; }

        public MedicalRecord(int id, int height, int weight, List<Anamnesis> medicalHistory,List<Referral>referrals, string allergens)
        {
            this.id = id;
            this.height = height;
            this.weight = weight;
            this.medicalHistory = medicalHistory;
            this.referrals = referrals;
            this.allergens = allergens;
        }

        public static void addAnamnesis(Anamnesis anamnesis,int patientId)
        {
            var medicalRecord = get(patientId);
            if (medicalRecord == null) { throw new ArgumentException("Invalid patient ID"); }
            medicalRecord.medicalHistory.Add(anamnesis);
            MainRepository.Save();

        }

        public static void Update(int newHeight, int newWeight, string newAllergens,int patientId) {

            MedicalRecord? medicalRecord = get(patientId);

            if (medicalRecord == null) { throw new ArgumentException("Invalid patient ID"); }

            medicalRecord.height = newHeight;
            medicalRecord.weight = newWeight;
            medicalRecord.allergens = newAllergens;

            MainRepository.Save();
        }

        public static MedicalRecord? get(int patientId)
        {
            
            Patient? patient = MainRepository.patients.FirstOrDefault(p => p.id == patientId);
            if (patient == null) return null;

            int medicalRecordId = patient.medicalRecordId;

            return MainRepository.medicalRecords.FirstOrDefault(mr => mr.id == medicalRecordId);
        }
    }
}
