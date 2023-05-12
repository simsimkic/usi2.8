using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
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
using ZdravoCorp.Doctors;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Equipments;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Users;


namespace ZdravoCorp.Patients.PatientsGUI
{
    /// <summary>
    /// Interaction logic for CRUDWindow.xaml
    /// </summary>
    public partial class CRUDWindow : Window
    {
        
        public CRUDWindow()
        {
            InitializeComponent();
            loadDataIntoTable();
            fillDoctorComboBox(doctors_comboBox);
            Patient patient = GetPatientById(ActivePatient.acitveUser.id);
            updatePatientWithRecentRecordCounts(patient);
            Update_Button.IsEnabled = false;
            Delete_Button.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new PatientWindow(ActivePatient.acitveUser).Show();
            this.Close();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CRUD_table.SelectedItem == null)
                return;

            Examination row = (Examination)CRUD_table.SelectedItem;

            showExaminationDetails(row);
            enableUpdateAndDeleteButtons();
            disableInsertButton();
        }

        private void showExaminationDetails(Examination row)
        {
            id_txt.Text = row.id.ToString();
            operation_txt.Text = row.isOperation ? "Yes" : "No";
            emergency_txt.Text = row.isEmergency ? "Yes" : "No";
            date_txt.Text = row.timeSlot.start.Date.ToString("yyyy-MM-dd");
            start_txt.Text = row.timeSlot.start.TimeOfDay.ToString();
            duration_txt.Text = row.timeSlot.duration.ToString();
            doctorId_txt.Text = row.doctorId.ToString();
            patient_txt.Text = row.patientId.ToString();
            status_txt.Text = row.status.ToString();
        }

        private void enableUpdateAndDeleteButtons()
        {
            Update_Button.IsEnabled = true;
            Delete_Button.IsEnabled = true;
        }

        private void disableInsertButton()
        {
            Insert_Button.IsEnabled = false;
        }

        public void loadDataIntoTable()
        {
            List<Examination> examinations = MainRepository.examinations;
            for (int i = 0; i < examinations.Count; i++)
            {
                if (examinations[i].patientId == ActivePatient.acitveUser.id)
                {
                    CRUD_table.Items.Add(examinations[i]);
                }    
            }
        }

        public static void fillDoctorComboBox(ComboBox doctor_cbx)
        {
            List<Doctor> doctors = MainRepository.doctors;
            foreach (Doctor doctor in doctors)
            {
                string doctorFullName = $"{doctor.id}: {doctor.name} {doctor.lastname} - {doctor.specialty}";
                doctor_cbx.Items.Add(doctorFullName);
            }
        }

        private void updatePatientWithRecentRecordCounts(Patient patient)
        {
            List<PatientRecord> patientRecords = MainRepository.patientRecords;

            patient.examinationsCancelledLastMonthCount = 0;
            patient.examinationsScheduledLastMonthCount = 0;
            patient.examinationsModifiedLastMonthCount = 0;
            foreach (PatientRecord patientRecord in patientRecords)
            {
                if(patientRecord.patientId == patient.id)
                {
                    if (isDateWithinLast30Days(patientRecord.modifiedDate))
                    {
                        switch (patientRecord.changeDescription)
                        {
                            case "create":
                                patient.examinationsScheduledLastMonthCount++;
                                break;
                            case "modified":
                                patient.examinationsModifiedLastMonthCount++;
                                break;
                            case "cancelled":
                                patient.examinationsCancelledLastMonthCount++;
                                break;
                        }
                    }
                }
            }
        }

        public bool isDateWithinLast30Days(DateTime dateToCheck)
        {
            DateTime today = DateTime.Today;
            DateTime thirtyDaysAgo = today.AddDays(-30);
            return (dateToCheck >= thirtyDaysAgo && dateToCheck <= today);
        }

        public void clearData()
        {
            id_txt.Clear();
            operation_txt.Clear();
            emergency_txt.Clear();
            date_txt.Clear();
            start_txt.Clear();
            duration_txt.Clear();
            doctors_comboBox.SelectedIndex = -1;
            doctorId_txt.Clear();
            patient_txt.Clear();
            status_txt.Clear();

            Update_Button.IsEnabled = false;
            Delete_Button.IsEnabled = false;
            Insert_Button.IsEnabled = true;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            clearData();
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            createNewExamination();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            chooseExaminationToCancell();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            updateExamination();
        }

        public void createNewExamination()
        {
            if (!isValidInput()) return;

            int newExaminationId = findMaxExaminationsId() + 1;

            string selectedDoctor = doctors_comboBox.SelectedItem.ToString();

            int doctorId = getDoctorIdFromString(selectedDoctor);

            int patientId = ActivePatient.acitveUser.id;

            if (!TryParseAndCheckDateTime(out DateTime newExaminationStart)) return;

            if (isDoctorAvailable(newExaminationStart, doctorId))
            {
                if (!isDoctorDayOff(doctorId, newExaminationStart))
                {
                    TimeSpan timeSpan = TimeSpan.FromMinutes(15);
                    Duration duration = timeSpan.Duration();
                    TimeSlot newExaminationTimeSlot = new TimeSlot(newExaminationStart, duration);
                    Examination newExamination = 
                        new Examination(newExaminationId, 
                        false, 
                        false, 
                        newExaminationTimeSlot, 
                        doctorId, 
                        patientId, 
                        Examination.Status.SCHEDULED);
                    Admission admission = RecommendedFreeExaminationsWindow.CreateAdmission(newExamination.id);
                    ExamRoom examRoom = RecommendedFreeExaminationsWindow.CreateExamRoom(newExamination.id, newExamination.timeSlot);
                    MainRepository.examinations.Add(newExamination);
                    MainRepository.admissions.Add(admission);
                    MainRepository.examrooms.Add(examRoom);
                    MainRepository.Save();

                    MessageBox.Show("Examination successfully scheduled!");
                    CRUD_table.Items.Clear();
                    loadDataIntoTable();
                    clearData();

                    Patient patient = GetPatientById(ActivePatient.acitveUser.id);
                    patient.examinationsScheduledLastMonthCount++;
                    createPatientRecord("create");
                    MainRepository.Save();

                    warningBeforeBlocking(patient);
                    blockPatientIfNecessary(patient);
                    isPatientBlocked(patient);
                }
            }
        }

        public void updateExamination()
        {
            Examination selectedExamination = (Examination)CRUD_table.SelectedItem;

            if (selectedExamination == null)
            {
                MessageBox.Show("You didn't choose examination for update!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!isValidInput()) return;

            string selectedDoctor = doctors_comboBox.SelectedItem.ToString();
            int doctorId = getDoctorIdFromString(selectedDoctor);

            selectedExamination.doctorId = doctorId;

            if (!TryParseAndCheckDateTime(out DateTime newExaminationStart)) return;

            if (isTimeDifferenceLessThan24Hours(DateTime.Now, newExaminationStart))
            {
                MessageBox.Show("The examination can be updated at least 24 hours before!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (selectedExamination.status == Examination.Status.CANCELLED)
            {
                MessageBox.Show("Examination has already been cancelled.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (isDoctorAvailable(newExaminationStart, doctorId))
            {
                if (!isDoctorDayOff(doctorId, newExaminationStart))
                {

                    TimeSpan timeSpan = TimeSpan.FromMinutes(15);
                    Duration duration = timeSpan.Duration();
                    TimeSlot newExaminationTimeSlot = new TimeSlot(newExaminationStart, duration);

                    selectedExamination.timeSlot = newExaminationTimeSlot;

                    updateSelectedExamination(selectedExamination);
                    MessageBox.Show("Examination successfully updated!");
                    CRUD_table.Items.Clear();
                    loadDataIntoTable();
                    clearData();

                    Patient patient = GetPatientById(ActivePatient.acitveUser.id);
                    patient.examinationsModifiedLastMonthCount++;
                    createPatientRecord("modified");
                    MainRepository.Save();

                    warningBeforeBlocking(patient);
                    blockPatientIfNecessary(patient);
                    isPatientBlocked(patient);
                }
            }

        }

        private void updateSelectedExamination(Examination selectedExamination)
        {
            List<Examination> allExaminations = MainRepository.examinations;

            foreach(Examination examination in allExaminations)
            {
                if(examination.id == selectedExamination.id)
                {
                    examination.doctorId = selectedExamination.doctorId;
                    examination.timeSlot = selectedExamination.timeSlot;
                    examination.status = Examination.Status.MODIFIED;
                }
            }

            MainRepository.Save();
        }

        public void chooseExaminationToCancell()
        {
            Examination selectedExamination = (Examination)CRUD_table.SelectedItem;

            if (selectedExamination == null)
            {
                MessageBox.Show("You didn't choose examination for cancell!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (isTimeDifferenceLessThan24Hours(DateTime.Now, selectedExamination.timeSlot.start))
            {
                MessageBox.Show("The examination can be canceled at least 24 hours before!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (selectedExamination.status == Examination.Status.CANCELLED)
            {
                MessageBox.Show("Examination has already been cancelled.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            cancellExamination(selectedExamination);
            CRUD_table.Items.Clear();
            loadDataIntoTable();
            MessageBox.Show("Examination successfully cancelled!");
            clearData();

            Patient patient = GetPatientById(ActivePatient.acitveUser.id);
            patient.examinationsCancelledLastMonthCount++;
            createPatientRecord("cancelled");
            MainRepository.Save();

            warningBeforeBlocking(patient);
            blockPatientIfNecessary(patient);
            isPatientBlocked(patient);
        }

        public void cancellExamination(Examination examinationForCancell)
        {
            List<Examination> allExaminations = MainRepository.examinations;
            foreach (Examination examination in allExaminations)
            {
                if (examinationForCancell.id == examination.id)
                {
                    examination.status = Examination.Status.CANCELLED;
                }
            }

            MainRepository.Save();
        }

        private bool TryParseAndCheckDateTime(out DateTime newExaminationStart)
        {
            try
            {
                DateTime newExaminationDate = ParseAndCheckDate(date_txt.Text);
                DateTime newExaminationTime = ParseAndCheckTime(start_txt.Text, newExaminationDate);
                newExaminationStart = newExaminationDate.Add(newExaminationTime.TimeOfDay);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                newExaminationStart = default;
                return false;
            }
        }

        private void createPatientRecord(string change)
        {
            string changeDescription = "";
            switch (change)
            {
                case "create":
                    changeDescription = "create";
                    break;
                case "modified":
                    changeDescription = "modified";
                    break;
                case "cancelled":
                   changeDescription = "cancelled";
                    break;
            }
            PatientRecord patientRecord = new PatientRecord(ActivePatient.acitveUser.id, 
                DateTime.Now, changeDescription);

            MainRepository.patientRecords.Add(patientRecord);
        }

        public void blockPatientIfNecessary(Patient patient)
        {
            int scheduledExamsLastMonth = patient.examinationsScheduledLastMonthCount;
            int cancelledExamsLastMonth = patient.examinationsCancelledLastMonthCount;
            int modifiedExamsLastMonth = patient.examinationsModifiedLastMonthCount;

            if (scheduledExamsLastMonth > 8)
            {
                patient.isBlocked = true;
                updatePatientsList(patient.id);
            }
            else if (cancelledExamsLastMonth >= 5)
            {
                patient.isBlocked = true;
                updatePatientsList(patient.id);
            }
            else if (modifiedExamsLastMonth >= 5)
            {
                patient.isBlocked = true;
                updatePatientsList(patient.id);
            }
        }



        private void warningBeforeBlocking(Patient patient)
        {
            int scheduledExamsLastMonth = patient.examinationsScheduledLastMonthCount;
            int cancelledExamsLastMonth = patient.examinationsCancelledLastMonthCount;
            int modifiedExamsLastMonth = patient.examinationsModifiedLastMonthCount;

            if (scheduledExamsLastMonth == 8)
            {
                MessageBox.Show("You have scheduled an examination 8 times in the last month, be careful!");
            }
            if (cancelledExamsLastMonth == 4)
            {
                MessageBox.Show("You have cancelled an examination 4 times in the last month, be careful!");
            }
            if (modifiedExamsLastMonth == 4)
            {
                MessageBox.Show("You have modified an examination 4 times in the last month, be careful!");
            }
        }

        private void isPatientBlocked(Patient patient)
        {
            if (patient.isBlocked)
            {
                MessageBox.Show("You are blocked!");
                new PatientWindow(ActivePatient.acitveUser).Show();
                this.Close();
            }
        }

        private Patient GetPatientById(int patientId)
        {
            return MainRepository.patients.FirstOrDefault(p => p.id == patientId);
        }

        public bool isDoctorAvailable(DateTime start, int doctorId)
        {
            List<Examination> examinations = MainRepository.examinations;

            foreach (Examination examination in examinations)
            {
                if (doctorId != examination.doctorId)
                {
                    continue;
                }

                DateTime examintionStart = examination.timeSlot.start;
                int durationInMinutes = (int)examination.timeSlot.duration.TimeSpan.TotalMinutes;
                DateTime examinationEnd = examintionStart.AddMinutes(durationInMinutes);

                if (start >= examintionStart && start <= examinationEnd)
                {
                    MessageBox.Show("The doctor is not available at the desired time!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }

        public bool isDoctorDayOff(int doctorId, DateTime dateInput)
        {
            DateOnly dateOnlyInput = DateOnly.FromDateTime(dateInput);

            List<Doctor> doctors = MainRepository.doctors;
            foreach (Doctor doctor in doctors)
            {
                if(doctor.id != doctorId)
                {
                    continue;
                }
                else
                {
                    List<FreeDayRequest> freeDays = doctor.freeDays;

                    foreach(FreeDayRequest freeDay in freeDays)
                    {
                        DateOnly start = freeDay.start;
                        DateOnly end = freeDay.end;

                        if(dateOnlyInput >= start && dateOnlyInput <= end)
                        {
                            MessageBox.Show("The doctor has the day off!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                            return true;
                        }
                    } 
                }
            }

            return false;
        }

        public static int getDoctorIdFromString(string selectedDoctor)
        {
            string[] parts = selectedDoctor.Split(':');
            int doctorId = Int32.Parse(parts[0]);
            return doctorId;
        }

        public static DateTime ParseAndCheckDate(string dateString)
        {
            DateTime date;
            try
            {
                date = DateTime.ParseExact(dateString, "yyyy-MM-dd", null);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while converting the date: " + ex.Message);
            }

            if (date < DateTime.Today)
            {
                throw new Exception("You have entered a date which is in past.");
            }
            else
            {
                return date;
            }
        }

        public DateTime ParseAndCheckTime(string timeInput, DateTime date)
        {
            DateTime time;
            try
            {
                time = DateTime.ParseExact(timeInput, "HH:mm:ss", null);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while converting the date: " + ex.Message);
            }

            if (date == DateTime.Today && time < DateTime.Now)
            {
                throw new Exception("You have entered a time which is in past.");
            }
            else
            {
                return time;
            }
        }

        public bool isTimeDifferenceLessThan24Hours(DateTime firstDate, DateTime secondDate)
        {
            TimeSpan difference = secondDate - firstDate;
            if (difference.TotalHours < 24)
            {
                return true;
            }
            return false;
        }

        public bool isValidInput()
        {
            if(date_txt.Text == String.Empty)
            {
                MessageBox.Show("Empty input!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (start_txt.Text == String.Empty)
            {
                MessageBox.Show("Empty input!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (doctors_comboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Empty input!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        
        private void updatePatientsList(int patientId)
        {
            List<Patient> allPatinets = MainRepository.patients;
            foreach (Patient patient in allPatinets)
            {
                if(patient.id == patientId)
                {
                    patient.isBlocked = true;
                }
            }
            MainRepository.Save();
        }

        public static int findMaxExaminationsId()
        {
            List<Examination> examinations = MainRepository.examinations;
            int maxExaminationsId = -1;
            for(int i = 0; i < examinations.Count; i++)
            {
                int examinationId = examinations[i].id;
                if (examinationId > maxExaminationsId){
                    maxExaminationsId = examinationId;
                }
            }

            return maxExaminationsId;
        }
    }

}
