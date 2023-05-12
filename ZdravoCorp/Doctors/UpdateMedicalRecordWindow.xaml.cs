using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using ZdravoCorp.Nurses;
using ZdravoCorp.ZdravoCorpData.Examinations;

namespace ZdravoCorp.Doctors
{
    /// <summary>
    /// Interaction logic for UpdateMedicalRecordWindow.xaml
    /// </summary>
    public partial class UpdateMedicalRecordWindow : Window
    {
        public int patientId;
        public int doctorId;
        public UpdateMedicalRecordWindow(int patientId,int doctorId)
        {
            InitializeComponent();

            this.patientId = patientId;
            this.doctorId = doctorId;

            MedicalRecord medicalRecord = MedicalRecord.get(patientId);
            DataContext = medicalRecord;

            List<Anamnesis> anamnesisList = medicalRecord.medicalHistory;
            anamnesis.ItemsSource = anamnesisList;
        }

        public int PatientId
        {
            get { return patientId; }
        }


        private void Button_Click(object sender, RoutedEventArgs e)  //adding anamnesis in medical history
        {
            DateTime? selectedDate = newDate.SelectedDate;
            string observation = newObservation.Text;

            if (!anamnesisValidation(selectedDate, observation)) { return; }

            DateOnly newDateOfMHE = DateOnly.FromDateTime(selectedDate.Value);
            Anamnesis.Create(newDateOfMHE, observation, patientId, doctorId);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)  //updating data in medical record
        {
           
            if (!validateIntInput(newHeight.Text, "Height") || !validateIntInput(newWeight.Text, "Weight")){ return;}

            string allergens = newAllergens.Text;

            MedicalRecord.Update(int.Parse(newHeight.Text), int.Parse(newWeight.Text), allergens, patientId);

            MessageBox.Show("Patients medical record is successfully updated !");
            this.Close();
        }

        public bool anamnesisValidation(DateTime? date, string observation)
        {
            if (!isValidObservation(observation))
            {
                MessageBox.Show("Observation field can't be empty!");
                return false;
            }

            if (!isValidDate(date))
            {
                MessageBox.Show("You didn't choose a valid date!");
                return false;
            }
            return true;
        }

        private bool validateIntInput(string input, string fieldName)
        {
            if (!int.TryParse(input, out int result) || result <= 0)
            {
                MessageBox.Show(fieldName + " is not valid!");
                return false;
            }

            return true;
        }

        private bool isValidObservation(string observation)
        {
            return !string.IsNullOrWhiteSpace(observation);
        }

        private bool isValidDate(DateTime? date)
        {
            if (date == null)
            {
                MessageBox.Show("Date field is empty!");
                return false;
            }
            return date.HasValue && date <= DateTime.Now;
        }
    }
}
