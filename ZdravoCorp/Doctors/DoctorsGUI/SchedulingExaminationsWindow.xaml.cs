using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static ZdravoCorp.ZdravoCorpData.Examinations.Examination;

namespace ZdravoCorp.Doctors.DoctorsGUI
{
    /// <summary>
    /// Interaction logic for SchedulingExaminationWindow.xaml
    /// </summary>
    public partial class SchedulingExaminationsWindow : Window
    {
        public int id;
        public ObservableCollection<Patient> doctorsPatients { get; set; }
        public SchedulingExaminationsWindow(int doctorId)
        {
            InitializeComponent();
            doctorsPatients = DoctorsService.getPatients(doctorId);
            this.id = doctorId;

            //filling the table
            for (int i = 0; i < doctorsPatients.Count; i++)
            {
                DoctorsPatients.Items.Add(doctorsPatients[i]);
            }

            availableRooms.ItemsSource = MainRepository.rooms
            .Where(r => r.type == Room.Type.EXAMINATION || r.type == Room.Type.OPERATION);
            //.Select(r => r.id);

        }

        public void Schedule(int doctorId)
        {
            Patient selectedPatient = (Patient)DoctorsPatients.SelectedItem;

            if (selectedPatient == null)
            {
                MessageBox.Show("You must choose a patient!");
                return;
            }


            /*Room? room = availableRooms.SelectedItem as Room;
            if(room == null) { MessageBox.Show("You didn`t choose room!"); return; }*/


            int patientId = selectedPatient.id;

            bool isOperationChecked = false;
            bool isEmergencyChecked = false;

            if (isOperation.IsChecked == true) { 
                isOperationChecked = true; 
                
            }
            if (isEmergency.IsChecked == true) { 
                isEmergencyChecked = true; 

            }


            /*if (isOperationChecked && room.type == Room.Type.EXAMINATION)
            {
                MessageBox.Show("You can't choose room for operations! You need examination room!");
                return;
            }

            if (!isOperationChecked && room.type == Room.Type.OPERATION)
            {
                MessageBox.Show("You can't choose room for operations! You need examination room!");

                return;
            }*/

            string dateString = date.Text;

            if (GetDate() == null)
            {
                return;
            }

            if (!DoctorsService.GetTime(time.Text))
            {
                return;
            }

            Duration duration;

            if (!isOperationChecked)
            {
                TimeSpan timeSpan = TimeSpan.FromMinutes(15);
                duration = timeSpan.Duration();
            }
            else
            {
                if (!DoctorsService.GetDuration(operationDuration.Text).Item1){return;}
                else {
                    TimeSpan timeSpan = TimeSpan.FromMinutes(DoctorsService.GetDuration(operationDuration.Text).Item2);
                    duration = timeSpan.Duration();
                }
            }

            string startString = date.Text +"T"+ time.Text;
       
            DateTime startDate = DateTime.Parse(startString);
            TimeSlot timeSlot=new TimeSlot(startDate,duration);

            if (timeSlot.start < DateTime.Now)
            {
                MessageBox.Show("Choosen time slot is in the past!");
                return;
            }

            int examinationId = 0;

            bool available = DoctorsService.isTimeSlotAvailableForPatientAndDoctor(timeSlot, patientId, doctorId,examinationId);

            if (!available)
            {
                MessageBox.Show("You can`t schedule examination in this TimeSlot,because it`s NOT FREE.");
                date.Clear();
                time.Clear();
                return;
            }

            int durationInMinutes = (int)duration.TimeSpan.TotalMinutes;
            DateTime intervalEnd = startDate.AddMinutes(durationInMinutes);

            Room.Type roomType = Room.Type.EXAMINATION;
            if (isOperationChecked) { roomType = Room.Type.OPERATION; }
            int roomId = Room.getAvailable(timeSlot.start,intervalEnd,roomType);

            MessageBox.Show("Room ID: "+roomId.ToString());

            Examination.Status status = Status.SCHEDULED;

            examinationId = DoctorsService.findExaminationsId()+1;

            Examination newExamination = new Examination(examinationId, isOperationChecked, isEmergencyChecked, timeSlot, doctorId, patientId, status);
            DoctorsService.addNewExamination(newExamination,roomId);
            MessageBox.Show("Examination is successfully scheduled!");

        }

       
        public bool IsDateOnlyValid(string dateString)
        {
            DateOnly dateValue;
            string format = "yyyy-MM-dd";

            return DateOnly.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue);
        }

        public DateOnly? GetDate()
        {
            if (date.Text == "")
            {
                MessageBox.Show("You didn`t choose a date!");
                return null;
            }

            if (!DoctorsService.IsDateOnlyValid(date.Text))
            {
                MessageBox.Show("Invalid date format!");
                date.Clear();
                return null;
            }
            string dateString = date.Text;
            string format = "yyyy-MM-dd";

            return DateOnly.ParseExact(dateString, format, CultureInfo.InvariantCulture);

        }
        public void btnExitProgram_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void operationDuration_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e) //create button
        {
            Schedule(id);
            MainRepository.Save();
        }
    }
}
