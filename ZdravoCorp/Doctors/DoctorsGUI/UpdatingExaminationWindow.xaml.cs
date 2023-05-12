using System;
using System.Collections.Generic;
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
using ZdravoCorp.ZdravoCorpData.Users;
using static ZdravoCorp.ZdravoCorpData.Examinations.Examination;

namespace ZdravoCorp.Doctors.DoctorsGUI
{
    /// <summary>
    /// Interaction logic for UpdatingExaminationWindow.xaml
    /// </summary>
    public partial class UpdatingExaminationWindow : Window
    {
        Examination examination;
        public UpdatingExaminationWindow(Examination examination)
        {
            InitializeComponent();
            this.examination = examination;

            DataContext = examination;

            bool isOperationChecked = examination.isOperation;
            bool isEmergencyChecked = examination.isEmergency;

            statusComboBox.ItemsSource = Enum.GetValues(typeof(Examination.Status));

        }


        public DateOnly? GetNewDate()
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

        

        public Examination GetUpdatedData(Examination examination)
        {

            if (GetNewDate() == null) { return null; }
            DateOnly newDate = (DateOnly)GetNewDate();


            if (!DoctorsService.GetTime(time.Text))
            {
                return null;
            }

            Duration newDuration;
            if (examination.isOperation)
            {
                if (!DoctorsService.GetDuration(durationOfOperation.Text).Item1) { return null; }
                else
                {
                    if (DoctorsService.GetDuration(durationOfOperation.Text).Item2 == 0) { newDuration = examination.timeSlot.duration; }
                    else
                    {
                        TimeSpan timeSpan = TimeSpan.FromMinutes(DoctorsService.GetDuration(durationOfOperation.Text).Item2);
                        newDuration = timeSpan.Duration();
                    }
                }
            }
            else  { newDuration=examination.timeSlot.duration; }



            string startString = date.Text + "T" + time.Text;
            DateTime startDate = DateTime.Parse(startString);
            TimeSlot newTimeSlot = new TimeSlot(startDate, newDuration);


            bool available = DoctorsService.isTimeSlotAvailableForPatientAndDoctor(newTimeSlot, examination.patientId, examination.doctorId,examination.id);


            if (!available)
            {
                MessageBox.Show("You can`t schedule examination in this TimeSlot,because it`s NOT FREE.");
                date.Clear();
                time.Clear();
                return null;
            }


            int durationInMinutes = (int)newDuration.TimeSpan.TotalMinutes;
            DateTime intervalEnd = startDate.AddMinutes(durationInMinutes);

            //MessageBox.Show("END : " + intervalEnd.ToString());

            Examination.Status selectedValue = (Status)statusComboBox.SelectedItem;

            examination.timeSlot = newTimeSlot;
            examination.status = selectedValue;

            return examination;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Examination updatedExamination=GetUpdatedData(examination);
            Examination.Update(updatedExamination);
            MainRepository.Save();
        }
    }
}
