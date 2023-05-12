using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.ZdravoCorpData.Examinations
{
    public class Examination
    {
        public enum Status {
            SCHEDULED,
            MODIFIED,
            CANCELLED,
            FINISHED
        }

        public int id { get; set; }
        public bool isOperation { get; set; }
        public bool isEmergency { get; set; }
        public TimeSlot timeSlot { get; set; }

        public int doctorId{ get; set; }
        public int patientId { get; set; }

        public Status status { get; set; } 
        public Examination(int id, bool isOperation, bool isEmergency, TimeSlot timeSlot, int doctorId, int patientId, Status status)
        {   
            this.id = id;
            this.isOperation = isOperation;
            this.timeSlot = timeSlot;
            this.doctorId = doctorId;
            this.patientId = patientId;
            this.status = status;
            this.isEmergency = isEmergency;
        }


        public static void Finish(Examination examToFinish)
        {
            Examination ? examination = MainRepository.examinations.FirstOrDefault(e => e.id == examToFinish.id);
            if (examination == null) { return; }

            modifyStatus(examination, Status.FINISHED);

        }

        public static void modifyStatus(Examination examination, Examination.Status newStatus)
        {
            examination.status = newStatus;
            MainRepository.Save();
        }

        public static void Update(Examination updatedExamination)
        {
            Examination? examinationToUpdate = MainRepository.examinations.FirstOrDefault(e => e.id == updatedExamination.id);

            if (examinationToUpdate == null)
            {
                throw new ArgumentException("Invalid examination ID");
            }

            examinationToUpdate.status = updatedExamination.status;
            examinationToUpdate.timeSlot.start = updatedExamination.timeSlot.start;
            examinationToUpdate.timeSlot.duration = updatedExamination.timeSlot.duration;

            MessageBox.Show("Examination is successfully updated!");
           
        }
    }
}
