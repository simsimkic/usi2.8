using System;
using System.Collections.Generic;
using System.Data;
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
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Rooms;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Nurses
{
    /// <summary>
    /// Interaction logic for EmergencyWindow.xaml
    /// </summary>
    public partial class EmergencyWindow : Window
    {
        private List<TimePeriod> _periods;
        private int _durationExamination;
        public EmergencyWindow()
        {
            InitializeComponent();
            FillComboBox();
            _periods = new List<TimePeriod>();
            _durationExamination = 15;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        private void FillComboBox()
        {
            List<Patient> patients = MainRepository.patients;
            ComboBoxPatient.Items.Add("Without account");
            for(int i = 0; i < patients.Count; i++)
            {
                ComboBoxPatient.Items.Add(patients[i].username);
            }
        }

        private void ButtonSchedule_Click(object sender, RoutedEventArgs e)
        {
            _durationExamination = (int)Convert.ToInt32(TextBoxDuration.Text);
            if(_durationExamination < 15)
            {
                MessageBox.Show("The operation can't last less than 15 minutes", "Error");
            }
            else
            {
                List<Doctor> doctorsSelected = FindDoctorsBySpecialty();
                if (doctorsSelected.Count > 0)
                {
                    FindAppointment(doctorsSelected);
                }
                else
                {
                    MessageBox.Show("There are no doctors with such a specialty!", "Error");
                }
            }
        }

        private List<Doctor> FindDoctorsBySpecialty()
        {
            List<Doctor> doctors = MainRepository.doctors;
            List<Doctor> doctorsSameSpecialty = new List<Doctor>();
            for(int i = 0; i < doctors.Count; i++)
            {
                if (((int)doctors[i].specialty) == ComboBoxSpecialty.SelectedIndex)
                {
                    doctorsSameSpecialty.Add(doctors[i]);
                }
            }
            return doctorsSameSpecialty;
        }

        private void FindAppointment(List<Doctor> doctors)
        {
            List<Examination> examinations = MainRepository.examinations;
            List<Examination> existringExaminations = new List<Examination>();
            for(int j =0; j < doctors.Count; j++)
            {
                DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0, 0);
                TimePeriod timePeriod = new TimePeriod(start.AddMinutes(5), start.AddHours(2).AddMinutes(5)); // 5 minutes for admission
                _periods.Clear();
                _periods.Add(timePeriod);
                DateTime now = _periods[0].start;
                DateTime after2hours = _periods[0].end;
                for (int i = 0; i < examinations.Count; i++)
                {
                    if (examinations[i].timeSlot.start > (now - TimeSpan.FromMinutes(5)) && examinations[i].timeSlot.start < (after2hours - TimeSpan.FromMinutes(5)) && examinations[i].doctorId == doctors[j].id)
                    {
                        CorrectTime(examinations[i]);
                        existringExaminations.Add(examinations[i]);
                    }
                }
                for(int i = 0; i < _periods.Count; i++)
                {
                    if (CheckTime(_periods[i]))
                    {
                        int id = GenerateNextExaminationId();
                        bool isEmergency = false;
                        bool isOperation = false;
                        if (ComboBoxType.SelectedIndex == 0)
                        {
                            isEmergency = true;
                        }
                        else
                        {
                            isOperation = true;
                        }
                        Duration duration = new Duration(TimeSpan.FromMinutes(_durationExamination));
                        TimeSlot timeSlot = new TimeSlot(_periods[i].start, duration);
                        int patientId = GetPatientId();
                        if (patientId == -1)
                        {
                            patientId++;
                        }

                        Examination newExamination = new Examination(id, isOperation, isEmergency, timeSlot, doctors[j].id, patientId, Examination.Status.SCHEDULED);
                        Room.Type type = Room.Type.EXAMINATION;
                        if (newExamination.isOperation)
                        {
                            type = Room.Type.OPERATION;
                        }
                        MainRepository.examrooms.Add(new ExamRoom(newExamination.id, Room.getAvailable(newExamination.timeSlot.start, newExamination.timeSlot.start.AddMinutes(newExamination.timeSlot.duration.TimeSpan.TotalMinutes), type)));

                        Admission newAdmission = new Admission(newExamination.id, false);
                        MainRepository.examinations.Add(newExamination);
                        MainRepository.admissions.Add(newAdmission);
                        MainRepository.Save();
                        MessageBox.Show("The examination is scheduled and ready for admission!");
                        return;
                    }
                }

                OpenNextWindow(existringExaminations);
            }

        }
        
        private void CorrectTime(Examination examination)
        {
            DateTime startExamination = examination.timeSlot.start;
            int durationMinutes = (int)examination.timeSlot.duration.TimeSpan.TotalMinutes;
            DateTime endExamination = startExamination.AddMinutes(durationMinutes);
            List<TimePeriod> periodsNewList = new List<TimePeriod>();
            for(int i = 0; i < _periods.Count; i++)
            {   
                if (_periods[i].start < startExamination)
                {
                    if(_periods[i].end > startExamination)
                    {
                        TimePeriod timePeriod = new TimePeriod(_periods[i].start, startExamination);
                        periodsNewList.Add(timePeriod);
                        if (_periods[i].end > endExamination)
                        {
                            TimePeriod newTimePeriod = new TimePeriod(endExamination, _periods[i].end);
                            periodsNewList.Add(newTimePeriod);
                        }
                            
                    }
                    else
                    {
                        periodsNewList.Add(_periods[i]);
                    }
                }
                else
                {
                    if (_periods[i].end > endExamination)
                    {
                        TimePeriod timePeriod = new TimePeriod(endExamination, _periods[i].end);
                        periodsNewList.Add(timePeriod);
                    }
                }
            }
            _periods.Clear();
            _periods = periodsNewList;
        }

        private bool CheckTime(TimePeriod timePeriod)
        {
            if (timePeriod.start.AddMinutes(_durationExamination) > timePeriod.end)
            {
                return false;
            }
            return true;
        }

        public static int GenerateNextExaminationId()
        {
            List<Examination> examinations = MainRepository.examinations;
            int nextId = 0;
            for(int i = 0; i < examinations.Count; i++)
            {
                nextId = examinations[i].id;
            }
            return nextId + 1;
        }

        private int GetPatientId()
        {
            string patientUsername = (string)ComboBoxPatient.SelectedValue.ToString();
            List<Patient> patients = MainRepository.patients;
            for(int i = 0; i < patients.Count; i++)
            {
                if(patientUsername == patients[i].username)
                {
                    return patients[i].id;
                }
            }
            return -1;
        }

        private void ComboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(TextBoxDuration == null) { 
                return;
            }
            if(ComboBoxType.SelectedIndex == 0)
            {
                TextBoxDuration.IsEnabled = false;
                TextBoxDuration.Text = "15";
            }
            else
            {
                TextBoxDuration.IsEnabled = true;
            }
        }

        private void OpenNextWindow(List<Examination> examinations)
        {
            bool isEmergency = false;
            if (ComboBoxType.SelectedIndex == 0)
            {
                isEmergency = true;
            }
            int patientId = GetPatientId();
            if (patientId == -1)
            {
                patientId++;
            }
            ChooseAppointment chooseAppointment = new ChooseAppointment(examinations, isEmergency, patientId, _durationExamination);
            chooseAppointment.ShowDialog();
        }
    }
}
