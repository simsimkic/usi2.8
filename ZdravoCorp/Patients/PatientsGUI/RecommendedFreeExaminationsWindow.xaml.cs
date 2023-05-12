using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Rooms;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Patients.PatientsGUI
{
    
    public partial class RecommendedFreeExaminationsWindow : Window
    {
        public RecommendedFreeExaminationsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillPriorityComboBox();
            CRUDWindow.fillDoctorComboBox(doctor_cbx);
        }

        private void FillPriorityComboBox()
        {
            priority_cbx.Items.Add("DOCTOR");
            priority_cbx.Items.Add("TIME RANGE");
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            if (!Validations.isValidInputForFind(date_txt, start_txt, end_txt, doctor_cbx, priority_cbx))
            {
                MessageBox.Show("Empty input!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int doctorId = CRUDWindow.getDoctorIdFromString(doctor_cbx.Text);

            List<DateTime> dateTimes = Validations.getValidFutureDateTimes(start_txt, end_txt);
            DateTime startTime = dateTimes[0];
            DateTime endTime = dateTimes[1];

            DateTime latestDate = Validations.getValidFutureDate(date_txt);

            string priority = priority_cbx.SelectedItem.ToString();

            RecommendedExaminationsTable.Items.Clear();

            bool isSlotFound;
            FindAvailableExamination(doctorId, startTime, endTime, latestDate, priority, out isSlotFound);
            
            if (!isSlotFound)
            {
                FindThreeSimilarRecommendedExaminations(startTime);
            }
        }

        private void FindAvailableExamination(int doctorId, DateTime desiredStartTime, DateTime desiredEndTime, DateTime latestPossibleDate, string priority, out bool isSlotFound)
        {
            isSlotFound = false;
            DateTime currentDate = DateTime.Now.AddDays(1).Date;
            DateTime currentDateTime = CombineDateAndTime(currentDate, desiredStartTime.TimeOfDay);

            if (Validations.isDoctorAvailableAtTime(currentDateTime, doctorId))
            {
                isSlotFound = true;
            }
            else
            {
                currentDateTime = FindAvailableSlot(doctorId, currentDateTime, desiredStartTime, desiredEndTime, latestPossibleDate);
                isSlotFound = (currentDateTime != DateTime.MinValue);
            }

            if (isSlotFound)
            {
                GetRecommendedExamination(currentDateTime, doctorId);
            }
            else
            {
                bool isSimilarSlotFound;
                ExecuteBasedOnPriority(priority, doctorId, desiredStartTime, desiredEndTime, latestPossibleDate, out isSimilarSlotFound);
                isSlotFound = isSimilarSlotFound;
            }
        }

        private DateTime FindAvailableSlot(int doctorId, DateTime currentDateTime, DateTime desiredStartTime, DateTime desiredEndTime, DateTime latestPossibleDate)
        {
            TimeSpan timeIncrement = TimeSpan.FromMinutes(16);

            while (currentDateTime.Date <= latestPossibleDate.Date)
            {
                while (currentDateTime.TimeOfDay <= desiredEndTime.TimeOfDay)
                {
                    if (Validations.isDoctorAvailableAtTime(currentDateTime, doctorId))
                    {
                        return currentDateTime;
                    }
                    currentDateTime = currentDateTime.Add(timeIncrement);
                }
                currentDateTime = CombineDateAndTime(currentDateTime.AddDays(1).Date, desiredStartTime.TimeOfDay);
            }

            return DateTime.MinValue;
        }

        private void ExecuteBasedOnPriority(string priority, int doctorId, DateTime desiredStartTime, DateTime desiredEndTime, DateTime latestPossibleDate, out bool isSimilarSlotFound)
        {
            isSimilarSlotFound = false;
            if (priority == "DOCTOR")
            {
                FindSimilarExaminationForSelectedDoctor(doctorId, desiredStartTime, desiredEndTime, latestPossibleDate, out isSimilarSlotFound);
            }
            else
            {
                FindSimilarExaminationForDesiretTimeRange(doctorId, desiredStartTime, desiredEndTime, latestPossibleDate, out isSimilarSlotFound);
            }
        }

        private void GetRecommendedExamination(DateTime currentDateTime, int doctorId)
        {
            Examination examination = createRecommendedExamination(doctorId, currentDateTime);
            RecommendedExaminationsTable.Items.Add(examination);
        }

        private bool IsDoctorAlreadyChecked(int doctorId, List<int> checkedDoctors)
        {
            return checkedDoctors.Contains(doctorId);
        }

        private bool FindAvailableSlotByPriorityTimeRange(List<Doctor> doctors, DateTime desiredStartTime, DateTime desiredEndTime, DateTime latestPossibleDate, List<int> checkedDoctors, out bool isSlotFound)
        {
            bool isSlotFoundByPriority;
            foreach (var doctor in doctors)
            {
                if (!IsDoctorAlreadyChecked(doctor.id, checkedDoctors))
                {
                    FindAvailableExamination(doctor.id, desiredStartTime, desiredEndTime, latestPossibleDate, "TIME RANGE", out isSlotFoundByPriority);
                    if (isSlotFoundByPriority)
                    {
                        isSlotFound = true;
                        return true;
                    }
                    else
                    {
                        checkedDoctors.Add(doctor.id);
                    }
                }
            }
            isSlotFound = false;
            return false;
        }

        private void FindSimilarExaminationForDesiretTimeRange(int doctorId, DateTime desiredStartTime, DateTime desiredEndTime, DateTime latestPossibleDate, out bool isSlotFound)
        {
            isSlotFound = false;
            List<Doctor> doctors = MainRepository.doctors;
            List<int> checkedDoctors = new List<int>();
            checkedDoctors.Add(doctorId);

            if (FindAvailableSlotByPriorityTimeRange(doctors, desiredStartTime, desiredEndTime, latestPossibleDate, checkedDoctors, out isSlotFound))
            {
                return;
            }

            isSlotFound = false;
        }

        private void FindSimilarExaminationForSelectedDoctor(int doctorId, DateTime desiredStartTime, DateTime desiredEndTime, DateTime latestPossibleDate, out bool isSlotFound)
        {
            isSlotFound = false;
            var currentDate = DateTime.Now.AddDays(1).Date;
            var timeIncrement = TimeSpan.FromMinutes(16);
            var currentDateTimeFront = CombineDateAndTime(currentDate, desiredStartTime.TimeOfDay.Subtract(timeIncrement));
            var currentDateTimeEnd = CombineDateAndTime(currentDate, desiredEndTime.TimeOfDay.Add(timeIncrement));
            int counter = 0;

            if (!Validations.isDoctorAvailableAtTime(currentDateTimeFront, doctorId))
            {
                while (currentDate <= latestPossibleDate.Date)
                {
                    if (TryBookExaminationAtTime(doctorId, currentDateTimeFront, timeIncrement, out isSlotFound))
                    {
                        break;
                    }

                    if (TryBookExaminationAtTime(doctorId, currentDateTimeEnd, -timeIncrement, out isSlotFound))
                    {
                        break;
                    }
                    
                    currentDateTimeFront = CombineDateAndTime(currentDate, currentDateTimeFront.TimeOfDay.Subtract(timeIncrement));
                    currentDateTimeEnd = CombineDateAndTime(currentDate, currentDateTimeEnd.TimeOfDay.Add(timeIncrement));
                    counter++;
                    currentDate = IncrementDateIfConditionMet(currentDateTimeFront, counter).Date;
                }
            }
            else
            {
                isSlotFound = true;
                var examination = createRecommendedExamination(doctorId, currentDateTimeFront);
                FillDataIntoTable(examination);
            }
        }

        private DateTime IncrementDateIfConditionMet(DateTime currentdateTime, int brojac)
        {
            if (brojac == 2) currentdateTime = currentdateTime.Date.AddDays(1);
            return currentdateTime;
        }

        private void FillDataIntoTable(Examination recommednedExamination)
        {
            RecommendedExaminationsTable.Items.Add(recommednedExamination);
        }

        private bool TryBookExaminationAtTime(int doctorId, DateTime dateTime, TimeSpan timeIncrement, out bool isSlotFound)
        {
            isSlotFound = Validations.isDoctorAvailableAtTime(dateTime, doctorId);

            if (isSlotFound)
            {
                var examination = createRecommendedExamination(doctorId, dateTime);
                FillDataIntoTable(examination);
            }
            else
            {
                dateTime = dateTime.Add(timeIncrement);
            }

            return isSlotFound;
        }

        private void FindThreeSimilarRecommendedExaminations(DateTime desiredStartTime) 
        {
            List<Examination> similarRecommendedExaminations = new List<Examination>();
            TimeSpan timeIncrement = TimeSpan.FromMinutes(16);
            DateTime currentDate = DateTime.Now.AddDays(1).Date;
            DateTime currentDateTime = CombineDateAndTime(currentDate, desiredStartTime.TimeOfDay);

            int counter = 0;

            while (similarRecommendedExaminations.Count < 3)
            {
                IsDoctorAvailableInSimilarSlot(currentDateTime, similarRecommendedExaminations);
                currentDateTime = CombineDateAndTime(currentDate, currentDateTime.TimeOfDay.Add(timeIncrement));
                counter++;
                currentDate = IncrementDateIfConditionMet(currentDateTime, counter).Date;
                if (counter == 2)
                {
                    currentDateTime = CombineDateAndTime(currentDate, desiredStartTime.TimeOfDay);
                    counter = 0;
                }
            }

            InsertListMembersIntoTbale(similarRecommendedExaminations);
        }

        private void IsDoctorAvailableInSimilarSlot(DateTime currentDateTime, List<Examination> similarRecommendedExaminations)
        {
            List<Doctor> doctors = MainRepository.doctors;
            foreach (Doctor doctor in doctors)
            {
                if (Validations.isDoctorAvailableAtTime(currentDateTime, doctor.id))
                {
                    Examination examination = createRecommendedExamination(doctor.id, currentDateTime);
                    similarRecommendedExaminations.Add(examination);
                }
            }

        }

        private void InsertListMembersIntoTbale(List<Examination> examinations)
        {
            foreach (var exam in examinations.Take(3))
            {
                RecommendedExaminationsTable.Items.Add(exam);
            }
        }

        private TimeSlot CreateTimeSlot(DateTime dateTime)
        {
            TimeSpan timeSpan = TimeSpan.FromMinutes(15);
            Duration duration = timeSpan.Duration();
            TimeSlot newExaminationTimeSlot = new TimeSlot(dateTime, duration);

            return newExaminationTimeSlot;
        }

        private Examination createRecommendedExamination(int doctorId, DateTime currentDateTime)
        {
            TimeSlot timeSlot = CreateTimeSlot(currentDateTime);
            int patientId = ActivePatient.acitveUser.id;

            Examination recommednedExamination = new Examination(CRUDWindow.findMaxExaminationsId() + 1, false, false, timeSlot,
                doctorId, patientId, Examination.Status.SCHEDULED);

            return recommednedExamination;
        }

        public static ExamRoom CreateExamRoom(int examinationId, TimeSlot timeSlot)
        {
            int durationInMinutes = (int)timeSlot.duration.TimeSpan.TotalMinutes;
            DateTime intervalEnd = timeSlot.start.AddMinutes(durationInMinutes);

            Room.Type roomType = Room.Type.EXAMINATION;

            int roomId = Room.getAvailable(timeSlot.start, intervalEnd, roomType);
            ExamRoom examRoom = new ExamRoom(examinationId, roomId);
            return examRoom;
        }

        public static Admission CreateAdmission(int examinationId)
        {
            Admission admission = new Admission(examinationId, false);
            return admission;
        }

        public DateTime CombineDateAndTime(DateTime date, TimeSpan time)
        {
            return date.Date + time;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecommendedExaminationsTable.SelectedItem == null)
                return;

            Examination row = (Examination)RecommendedExaminationsTable.SelectedItem;

            ShowExaminationDetails(row);
        }

        private void ShowExaminationDetails(Examination row)
        {
            selectedDate_txt.Text = row.timeSlot.start.Date.ToString("yyyy-MM-dd");
            selectedStartTime_txt.Text = row.timeSlot.start.TimeOfDay.ToString();
            selectedDoctorId_txt.Text = row.doctorId.ToString();
        }

        public bool IsValidInputForCreate()
        {
            if(selectedDoctorId_txt.Text==String.Empty || selectedDate_txt.Text==String.Empty || selectedStartTime_txt.Text == String.Empty)
            {
                MessageBox.Show("Empty input!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (!(IsValidInputForCreate())) return;
            DateTime dateTime = CreateDateTime();
            int doctorId = int.Parse(selectedDoctorId_txt.Text);
            Examination newExamination = createRecommendedExamination(doctorId, dateTime);
            Admission admission = CreateAdmission(newExamination.id);
            ExamRoom examRoom = CreateExamRoom(newExamination.id, newExamination.timeSlot);
            MessageBox.Show("Examination successfully scheduled!");
            SaveNewExamination(newExamination, admission, examRoom);
        }

        private DateTime CreateDateTime()
        {
            DateTime startTime = Validations.ParseStartTimeFromTextBox(selectedStartTime_txt.Text);
            DateTime date = DateTime.ParseExact(selectedDate_txt.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dateTime = date.Date + startTime.TimeOfDay;

            return dateTime;
        }

        private void SaveNewExamination(Examination newExamination, Admission admission, ExamRoom examRoom)
        {
            MainRepository.examinations.Add(newExamination);
            MainRepository.admissions.Add(admission);
            MainRepository.examrooms.Add(examRoom);
            MainRepository.Save();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new PatientWindow(ActivePatient.acitveUser).Show();
            this.Close();
        }
    }
}
