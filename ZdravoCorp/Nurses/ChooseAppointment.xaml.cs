using System;
using System.Collections.Generic;
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
using ZdravoCorp.ZdravoCorpData.Activities;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Rooms;

namespace ZdravoCorp.Nurses
{
    /// <summary>
    /// Interaction logic for ChooseAppointment.xaml
    /// </summary>
    public partial class ChooseAppointment : Window
    {
        private List<Examination> _examinations;
        private bool _isEmergency;
        private int _patientId;
        private int _durationExamination;
        public ChooseAppointment(List<Examination> examinations, bool isEmergency, int patientId, int durationExamination)
        {
            InitializeComponent();
            _examinations = examinations;
            _isEmergency = isEmergency;
            _patientId = patientId;
            _durationExamination = durationExamination;
            _examinations = SortExaminationsByDateTime(_examinations);
            FillComboBox();
        }

        private List<Examination> SortExaminationsByDateTime(List<Examination> examinations)
        {
            for(int i = 0; i < examinations.Count - 1; i++)
            {
                for(int j = i + 1; j < examinations.Count; j++)
                {
                    if (examinations[i].timeSlot.start > examinations[j].timeSlot.start)
                    {
                        Examination temp = examinations[i];
                        examinations[i] = examinations[j];
                        examinations[j] = temp;
                    }
                }
            }
            return examinations;
        }

        private void FillComboBox()
        {
            for(int i = 0; i < _examinations.Count && i < 5; i++)
            {
                ComboBoxAppontments.Items.Add(_examinations[i].timeSlot.start.ToString());
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonChoose_Click(object sender, RoutedEventArgs e)
        {
            Examination choosenExamination = _examinations[ComboBoxAppontments.SelectedIndex];
            int doctorId = choosenExamination.doctorId;
            DateTime endChoosenExamination = choosenExamination.timeSlot.start.AddMinutes(choosenExamination.timeSlot.duration.TimeSpan.TotalMinutes);
            List<Examination> examinationsFromDoctor = GetExaminationsFromDoctor(doctorId, endChoosenExamination);
            examinationsFromDoctor = SortExaminationsByDateTime(examinationsFromDoctor);
            DateTime newDateTime = FindAppointment(examinationsFromDoctor, endChoosenExamination, (int)choosenExamination.timeSlot.duration.TimeSpan.TotalMinutes);
            DateTime oldDateTime = choosenExamination.timeSlot.start;
            ChangeDateTimeExamination(choosenExamination, newDateTime);
            SendNotifications(choosenExamination.doctorId, "Examination for patient with ID " + choosenExamination.patientId.ToString() + " is delayed from " + oldDateTime.ToString() + " to " + newDateTime.ToString());
            SendNotifications(choosenExamination.patientId, "Your examination is delayed from " + oldDateTime.ToString() + " to " + newDateTime.ToString());
            choosenExamination.timeSlot.start = oldDateTime;
            choosenExamination = ChangeDataExamination(choosenExamination, oldDateTime);
            WriteNewExamination(choosenExamination);
            MessageBox.Show("You have successfully selected a date!");
            this.Close();
        }

        private Examination ChangeDataExamination(Examination examination, DateTime dateTime)
        {
            examination.id = EmergencyWindow.GenerateNextExaminationId();
            examination.patientId = _patientId;
            if(_isEmergency)
            {
                examination.isEmergency = true;
                examination.isOperation = false;
            }
            else
            {
                examination.isEmergency = false;
                examination.isOperation = true;
            }
            examination.timeSlot.start = dateTime;
            examination.timeSlot.duration = TimeSpan.FromMinutes(_durationExamination);
            return examination;
        }

        private List<Examination> GetExaminationsFromDoctor(int doctorId, DateTime endChoosenExamination)
        {
            List<Examination> examinations = MainRepository.examinations;
            List<Examination> examinationsFromDoctor = new List<Examination>();
            for(int i = 0; i < examinations.Count; i++)
            {
                if(examinations[i].doctorId == doctorId && examinations[i].timeSlot.start > endChoosenExamination)
                {
                    examinationsFromDoctor.Add(examinations[i]);
                }
            }
            return examinationsFromDoctor;
        }

        private DateTime FindAppointment(List<Examination> examinations, DateTime start, int duration)
        {
            bool check = false;
            bool checkEnd = false;
            DateTime end = start;
            do
            {
                start = end;
                end = start.AddMinutes(duration);
                for(int i = 0; i < examinations.Count; i++)
                {
                    if (examinations[i].timeSlot.start > start && examinations[i].timeSlot.start < end)
                    {
                        checkEnd = true;
                        break;
                    }
                }
                if(checkEnd)
                    check = true;
            }
            while (!check);
            return start;
        }

        private void WriteNewExamination(Examination newExamination)
        {
            Room.Type type = Room.Type.EXAMINATION;
            if (newExamination.isOperation)
            {
                type = Room.Type.OPERATION;
            }
            MainRepository.examrooms.Add(new ExamRoom(newExamination.id, Room.getAvailable(newExamination.timeSlot.start, newExamination.timeSlot.start.AddMinutes(newExamination.timeSlot.duration.TimeSpan.TotalMinutes), type)));
            MainRepository.admissions.Add(new Admission(newExamination.id, false));
            MainRepository.examinations.Add(newExamination);
            MainRepository.Save();
            MainRepository.Load();
        }

        private void ChangeDateTimeExamination(Examination examination, DateTime newDateTime)
        {
            List<Examination> examinations = MainRepository.examinations;
            for(int i = 0; i < examinations.Count; i++)
            {
                if (examinations[i].id == examination.id)
                {
                    examinations[i].timeSlot.start = newDateTime;
                }
            }
            MainRepository.examinations = examinations;
            MainRepository.Save();
            MainRepository.Load();
        }

        private void SendNotifications(int userId, string message)
        {
            Notification notification = new Notification(userId, message);
            MainRepository.notifications.Add(notification);
            MainRepository.Save();
            MainRepository.Load();
        }
    }
}
