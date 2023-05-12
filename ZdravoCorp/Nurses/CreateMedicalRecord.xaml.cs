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
using ZdravoCorp.ZdravoCorpData.Examinations;

namespace ZdravoCorp.Nurses
{
    /// <summary>
    /// Interaction logic for CreateMedicalRecord.xaml
    /// </summary>
    public partial class CreateMedicalRecord : Window
    {
        private MedicalRecord _medicalRecord;
        private AdmissionPatients _admissionPatients;
        private Examination examination;
        public CreateMedicalRecord(MedicalRecord medicalRecord, AdmissionPatients _admissionPatientsWindow, Examination examination)
        {
            InitializeComponent();
            _medicalRecord = medicalRecord;
            _admissionPatients = _admissionPatientsWindow;
            LoadData();
            this.examination = examination;
        }

        private void LoadData()
        {
            TextBoxHeight.Text = _medicalRecord.height.ToString();
            TextBoxWeight.Text = _medicalRecord.weight.ToString();
            TextBoxAllergens.Text = _medicalRecord.allergens;
            for (int i = 0; i < _medicalRecord.medicalHistory.Count; i++)
            {
                ListViewAnamnesis.Items.Add(_medicalRecord.medicalHistory[i]);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            AnamnesisWindow anamnesisWindow = new AnamnesisWindow(this);
            anamnesisWindow.Show();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            ListViewAnamnesis.Items.RemoveAt(ListViewAnamnesis.SelectedIndex);
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            if(CheckData())
            {
                List<MedicalRecord> medicalRecords = MainRepository.medicalRecords;
                medicalRecords.Remove(_medicalRecord);
                _medicalRecord.height = Int32.Parse(TextBoxHeight.Text);
                _medicalRecord.weight = Int32.Parse(TextBoxWeight.Text);
                _medicalRecord.allergens = TextBoxAllergens.Text;
                _medicalRecord.medicalHistory = readMedicalHistory();
                medicalRecords.Add(_medicalRecord);
                MainRepository.Save();
                MessageBox.Show("Admission is over!");
                _admissionPatients.OverAdmission(examination);
                this.Close();
            }
        }

        private List<Anamnesis> readMedicalHistory()
        {
            List<Anamnesis> medicalHistoryList = new List<Anamnesis>();
            for (int i = 0; i < ListViewAnamnesis.Items.Count; i++)
            {
                Anamnesis medicalHistory = (Anamnesis)ListViewAnamnesis.Items.GetItemAt(i);
                medicalHistoryList.Add(medicalHistory);
            }
            return medicalHistoryList;
        }

        private bool CheckData()
        {
            if (!CheckDataNumber(TextBoxHeight.Text))
            {
                MessageBox.Show("You must enter a number for the height!", "Error");
                return false;
            }
            if (!CheckDataNumber(TextBoxWeight.Text))
            {
                MessageBox.Show("You must enter a number for the weight!", "Error");
                return false;
            }
            return true;
        }

        private bool CheckDataNumber(string numberString)
        {
            int numberInt;
            return int.TryParse(numberString, out numberInt);
        }
    }
}
