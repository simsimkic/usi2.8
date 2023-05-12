using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZdravoCorp.ZdravoCorpData.Drugs
{
    public class Drug
    {
        public enum MedicationTiming
        {
            BEFORE_MEAL,
            AFTER_MEAL,
            DURING_MEAL,
            UNSPECIFIED
        }

        public int id { get; set; }
        public string name { get; set; }
        public int dailyDose { get; set; }
        public MedicationTiming medicationTiming { get; set; }


        public List<TimeOnly> dosingSchedule { get; set; }
        public int therapyDuration { get; set; }
        public List<string> ingridients { get; set; }


        public Drug(string name, int dailyDose, MedicationTiming medicationTiming,List<TimeOnly> dosingSchedule, int therapyDuration,List<string> ingridients)
        {
            this.name = name;
            this.dailyDose = dailyDose;
            this.medicationTiming = medicationTiming;
            this.dosingSchedule = dosingSchedule;
            this.therapyDuration = therapyDuration;
            this.ingridients = ingridients;
        }
    }
}
