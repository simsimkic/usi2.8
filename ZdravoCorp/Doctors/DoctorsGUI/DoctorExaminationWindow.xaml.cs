using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using ZdravoCorp.Nurses;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Users;
using static ZdravoCorp.ZdravoCorpData.Examinations.Examination;

namespace ZdravoCorp.Doctors.DoctorsGUI
{
    /// <summary>
    /// Interaction logic for DoctorsExaminationsWindow.xaml
    /// </summary>
    public partial class DoctorExaminationWindow : Window
    {
        public int id;
        public DoctorExaminationWindow(int doctorId)
        {
            InitializeComponent();
            this.id = doctorId;
            Fill();
        }

        public void Fill()
        {
            DoctorsExaminations.Items.Clear();
            List<Examination> doctorsExaminations = MainRepository.examinations.Where(e => e.doctorId == id).ToList();
            foreach (Examination examination in doctorsExaminations)
            {
                DoctorsExaminations.Items.Add(examination);
            }
        }

        public List<Examination> getExaminationsForDate()
        {
            List<Examination> examinations = new List<Examination>();
            string format = "yyyy-MM-dd";
            if (GetDate(format) != null)
            {
                string dateString = date.Text;

                DateOnly dateOnly = DateOnly.ParseExact(dateString, format, CultureInfo.InvariantCulture);

                DateTime choosenDate = dateOnly.ToDateTime(TimeOnly.MinValue);  //Time = 00:00:00

                List<Examination> allDoctorsExaminations = DoctorsService.getMyExaminations(this.id);

                for (int i = 0; i < allDoctorsExaminations.Count; i++)
                {
                    DateTime scheduledDate = allDoctorsExaminations[i].timeSlot.start;

                    if (scheduledDate.Equals(date))
                    {
                        examinations.Add(allDoctorsExaminations[i]);
                    }
                    else
                    {
                        TimeSpan timeSpan = scheduledDate - choosenDate; // calculate difference
                        int totalDays = timeSpan.Days;
                        if (scheduledDate>choosenDate && totalDays <= 3)
                        {
                            examinations.Add(allDoctorsExaminations[i]);
                        }
                    }
                }
          
            }
            return examinations;
        }


        public  DateOnly? GetDate(string format)
        {
            if (date.Text == "")
            {
                MessageBox.Show("Date field is empty!");
                Fill();
                return null;
            }

            if (!DoctorsService.IsDateOnlyValid(date.Text))
            {
                MessageBox.Show("Invalid date format!");
                date.Clear();
                return null;
            }
            string dateString =date.Text;
            
            return DateOnly.ParseExact(dateString, format, CultureInfo.InvariantCulture);

        }
 
        public void btnExitProgram_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoctorsExaminations.Items.Clear();
            
            List<Examination> examinations = getExaminationsForDate();
            for (int i = 0; i < examinations.Count; i++)
            {
                DoctorsExaminations.Items.Add(examinations[i]);
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Examination selectedItem = (Examination)DoctorsExaminations.SelectedItem;
            new MedicalRecordWindow(selectedItem.patientId).Show();
            
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) //cancelling examination
        {
            Examination selectedItem = (Examination)DoctorsExaminations.SelectedItem;

            if(selectedItem == null) { MessageBox.Show("You didn`t` choose examination for cancelling!");return; }

            if (selectedItem.timeSlot.start<DateTime.Now) { MessageBox.Show("You could`t cancel examination in the past!"); return; }

            if (selectedItem.status==Examination.Status.CANCELLED) { MessageBox.Show("This examination is already cancelled!"); return; }

            bool result = MessageBox.Show("Are you sure you want to cancel the examination?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

            if (result)
            {
                DoctorsService.cancelExamination(selectedItem);
                DoctorsExaminations.Items.Clear();
                Fill();
                MessageBox.Show("Examination is successfully cancelled!");
                MainRepository.Save();
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e) //updating examination
        {
            Examination selectedItem = (Examination)DoctorsExaminations.SelectedItem;

            if (selectedItem == null) { MessageBox.Show("You didn`t` choose examination for updating!"); return; }

            if (selectedItem.timeSlot.start < DateTime.Now) { MessageBox.Show("You can`t update examination in the past!"); return; }

            new UpdatingExaminationWindow(selectedItem).Show();
            DoctorsExaminations.Items.Clear();
            this.Close();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)   //Start examination button
        {
            Examination selectedItem = (Examination)DoctorsExaminations.SelectedItem;

            bool correct = examinationValidation(selectedItem);

            if (!correct) { return; }

            this.Close();

            new ActiveExaminationWindow(selectedItem,id).Show();
        }

        private bool statusValidation(Examination choosenExamination)
        {
            switch (choosenExamination.status)
            {
                case Examination.Status.SCHEDULED:
                case Examination.Status.MODIFIED:
                    break;
                default:
                    MessageBox.Show("Examination must be SCHEDULED or MODIFIED");
                    return false;
            }

            return true;
        }

        private bool timeSlotValidation(Examination choosenExamination)
        {
            if (choosenExamination.timeSlot.start.Date != DateTime.Today) { MessageBox.Show("You can't start this examination NOW!"); return false;}

            TimeSpan timeDiff = (DateTime.Now < choosenExamination.timeSlot.start) ? choosenExamination.timeSlot.start - DateTime.Now : DateTime.Now - choosenExamination.timeSlot.start;

            if (timeDiff.TotalMinutes < 5) {return true;}

            MessageBox.Show("You can't start this examination NOW!");

            return false; 
        }

        private bool examinationValidation(Examination choosenExamination)
        {
            if (choosenExamination == null)
            {
                MessageBox.Show("You didn't choose an examination!");
                return false;
            }

            if (!statusValidation(choosenExamination)){ return false; }

            if(!timeSlotValidation(choosenExamination)) {return false; }

            return true;
        }
    }
}