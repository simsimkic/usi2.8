using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using ZdravoCorp.Doctors.DoctorsGUI;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Users;


namespace ZdravoCorp.Doctors
{
    public static class DoctorsService
    {
        public static IEnumerable<int> getPatientsIds(int doctorId)  //getting patients ids from doctors examinations
        {
            var patientsIds = MainRepository.examinations
            .Where(examination => examination.doctorId == doctorId)
            .Select(examination => examination.patientId)
            .Distinct();

            return patientsIds;
        }
         public static ObservableCollection<Patient> getPatients(int doctorId)
         {
            var patientIds = getPatientsIds(doctorId);
            var allPatients = MainRepository.patients;
            var patients = new ObservableCollection<Patient>(allPatients.Where(patient => patientIds.Contains(patient.id)));
            return patients;
        }

        public static List<Examination> getMyExaminations(int doctorId)
        {

            return MainRepository.examinations.Where(e => e.doctorId == doctorId).ToList();

        }

        public static (bool, int) IsPositiveInteger(string input)
        {
            if (int.TryParse(input, out int number))
            {
                return (number > 0, number);
            }
            else
            {
                return (false, 0);
            }
        }

        public static (bool, int) GetDuration(string operationDuration)
        {
            string durationString = operationDuration;
            if (durationString == "")
            {
                return (true, 0);
            }

            if (!IsPositiveInteger(durationString).Item1)
            {
                MessageBox.Show("Invalid format for Duration.It must be int number!");
                return (false, 0);
            }
            return (true, IsPositiveInteger(durationString).Item2);

        }

        public static bool GetTime(string time)
        {
            string timeString = time;
            if (timeString == "")
            {
                MessageBox.Show("You didn`t choose a time!");
                return false;
            }

            string pattern = @"^(?:[01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$";
            if (!Regex.IsMatch(time, pattern))
            {
                MessageBox.Show("Invalid format of time!This is the correct format : HH:MM:SS");
                return false;
            }
            return true;

        }
        public static bool IsDateOnlyValid(string dateString)
        {
            DateOnly dateValue;
            string format = "yyyy-MM-dd";

            return DateOnly.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue);
        }

        public static List<Examination> getPatientsExaminations(int patientId)
        {
            List<Examination>patientsExaminations = new List<Examination>();
            List<Examination> allExaminations = MainRepository.examinations;

            for(int i = 0;i < allExaminations.Count; i++)
            {
                if (allExaminations[i].patientId== patientId) { patientsExaminations.Add(allExaminations[i]);}
            }
            return patientsExaminations;
        }

        public static bool intervalContainsTimeSlot(DateTime intervalStart, DateTime intervalEnd,DateTime timeSlot,Duration duration)
        {
            if(timeSlot>intervalStart && timeSlot < intervalEnd)
            {
                return true;
            }

            int durationInMinutes = (int)duration.TimeSpan.TotalMinutes;
            DateTime intervalEnd_ = timeSlot.AddMinutes(durationInMinutes);

            if(timeSlot<intervalStart && intervalEnd_ < intervalEnd && intervalEnd_>intervalStart) { return true; }

            return false;
        }

        public static void addNewExamination(Examination newExamination,int roomId)
        {
            MainRepository.examinations.Add(newExamination);
            Admission admission = new Admission(newExamination.id, false);
            ExamRoom examroom = new ExamRoom(newExamination.id, roomId);
            MainRepository.admissions.Add(admission);
            MainRepository.examrooms.Add(examroom);
        }


        public static List<TimeSlot> getScheduledTimeSlots(int doctorId,int patientId)
        {
            List<TimeSlot>timeSlots = new List<TimeSlot>();
            List<Examination>examinations=MainRepository.examinations;
            for(int i = 0; i<examinations.Count; i++)
            {
                if (examinations[i].doctorId==doctorId || examinations[i].patientId == patientId) { 
                    timeSlots.Add(examinations[i].timeSlot); 
                }
               
            }
            return timeSlots;
        }

        public static bool isTimeSlotAvailableForPatientAndDoctor(TimeSlot timeSlot, int patientId, int doctorId,int examinationId)
        {

            List<Examination> doctorsScheduledExaminations = DoctorsService.getMyExaminations(doctorId);
            TimeSlot scheduledTimeSlot;

            for (int i = 0; i < doctorsScheduledExaminations.Count; i++)
            {
                scheduledTimeSlot = doctorsScheduledExaminations[i].timeSlot;
                if (scheduledTimeSlot.overlapsWithTimeSlot(timeSlot) && doctorsScheduledExaminations[i].id!=examinationId) { return false; }

                DateTime intervalStart = scheduledTimeSlot.start;
                if (intervalStart.Date.Equals(timeSlot.start.Date) && doctorsScheduledExaminations[i].id != examinationId)  //same dates
                {
                    //MessageBox.Show(intervalStart.Date.ToString());
                    // DateTime intervalEnd = scheduledTimeSlot.start + TimeSpan.FromTicks(scheduledTimeSlot.duration.TimeSpan.Ticks);
                    int durationInMinutes = (int)scheduledTimeSlot.duration.TimeSpan.TotalMinutes;
                    DateTime intervalEnd = intervalStart.AddMinutes(durationInMinutes);
                    //MessageBox.Show("End: " +intervalEnd.ToString());
                    bool inInterval = DoctorsService.intervalContainsTimeSlot(intervalStart, intervalEnd, timeSlot.start, timeSlot.duration);
                    if (inInterval) { return false; }
                }
            }

            List<Examination> patientsScheduledExaminations = DoctorsService.getPatientsExaminations(patientId);

            for (int i = 0; i < patientsScheduledExaminations.Count; i++)
            {
                scheduledTimeSlot = patientsScheduledExaminations[i].timeSlot;
                if (scheduledTimeSlot.overlapsWithTimeSlot(timeSlot) && patientsScheduledExaminations[i].id != examinationId) { return false; }

                DateTime intervalStart = scheduledTimeSlot.start;
                if (scheduledTimeSlot.start.Date == timeSlot.start.Date && patientsScheduledExaminations[i].id != examinationId)  //same dates
                {

                    DateTime intervalEnd = scheduledTimeSlot.start + TimeSpan.FromTicks(scheduledTimeSlot.duration.TimeSpan.Ticks);
                    bool inInterval = DoctorsService.intervalContainsTimeSlot(intervalStart, intervalEnd, timeSlot.start, timeSlot.duration);
                    if (inInterval) { return false; }
                }
            }

            return true;
        }


        public static  int findExaminationsId()
        {
            List<Examination> examinations = MainRepository.examinations;
            int maxExaminationsId = -1;
            for (int i = 0; i < examinations.Count; i++)
            {
                int examinationId = examinations[i].id;
                if (examinationId > maxExaminationsId)
                {
                    maxExaminationsId = examinationId;
                }
            }

            return maxExaminationsId;
        }

        public static void cancelExamination(Examination cancelledExamination)
        {
            for (int i = 0; i < MainRepository.examinations.Count; i++)
            {
                if (MainRepository.examinations[i].id == cancelledExamination.id) { MainRepository.examinations[i].status = Examination.Status.CANCELLED; }
            }
        }



    }
}