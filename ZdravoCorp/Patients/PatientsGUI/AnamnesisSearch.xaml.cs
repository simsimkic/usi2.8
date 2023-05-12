using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
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
using ZdravoCorp.Managers;
using ZdravoCorp.Nurses;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Equipments;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Patients.PatientsGUI
{
    public partial class AnamnesisSearch : Window
    { 
        public AnamnesisSearch()
        {
            InitializeComponent();
            MedicalRecord patientMedicalRecord = GetMedicalRecord();
            FillPatientMedicalRecordData(patientMedicalRecord);
            List<AnamnesisDoctorPair> anamnesisDoctorPairs = getAnamnesisDoctorPairs(patientMedicalRecord);
            FillAnamnesisTable(anamnesisDoctorPairs);
            FillComboBoxForSort();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new PatientWindow(ActivePatient.acitveUser).Show();
            this.Close();
        }

        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxForSort.SelectedIndex == -1)
            {
                MessageBox.Show("Select what to sort by!", "Failed!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var view = CollectionViewSource.GetDefaultView(AnamnesisTable.ItemsSource);

            string selectedValue = comboBoxForSort.SelectedItem.ToString();

            switch (selectedValue)
            {
                case "DATE":
                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new SortDescription("anamnesis.date", ListSortDirection.Ascending));
                    break;
                case "DR NAME":
                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new SortDescription("doctor.name", ListSortDirection.Ascending));
                    break;
                case "DR LASTNAME":
                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new SortDescription("doctor.lastname", ListSortDirection.Ascending));
                    break;
                case "SPECIALTY":
                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new SortDescription("doctor.specialty", ListSortDirection.Ascending));
                    break;
                default:
                    break;
            }
        }

        private void FillComboBoxForSort()
        {
            comboBoxForSort.Items.Add("DATE");
            comboBoxForSort.Items.Add("DR NAME");
            comboBoxForSort.Items.Add("DR LASTNAME");
            comboBoxForSort.Items.Add("SPECIALTY");
        }

        private void FillAnamnesisTable(List<AnamnesisDoctorPair> anamnesisDoctorPairs)
        {
            AnamnesisTable.ItemsSource = anamnesisDoctorPairs;
        }

        private void Textbox_textChanged(object sender, TextChangedEventArgs e)
        {
            MedicalRecord patientMedicalRecord = GetMedicalRecord();
            List<AnamnesisDoctorPair> anamnesisDoctorPairs = getAnamnesisDoctorPairs(patientMedicalRecord);
            
            var tbx = sender as TextBox;
            if (tbx.Text != "")
            {
                var searchMatches = FindSearchMatches(tbx, anamnesisDoctorPairs);
                AnamnesisTable.ItemsSource = null;
                AnamnesisTable.ItemsSource = searchMatches;
            }
            else
            {
                FillAnamnesisTable(anamnesisDoctorPairs);
            }
        }

        public static IEnumerable<AnamnesisDoctorPair> FindSearchMatches(TextBox? tbx, List<AnamnesisDoctorPair> anamnesisDoctorPairs)
        {
            var searchTerm = tbx.Text.ToLower();
            var searchMatches = anamnesisDoctorPairs.Where(anamnesisItem =>
                anamnesisItem.anamnesis.observation.ToLower().Contains(searchTerm) 
            );
            return searchMatches;
        }

        private List<AnamnesisDoctorPair> getAnamnesisDoctorPairs(MedicalRecord medicalRecord)
        {
            List<AnamnesisDoctorPair> anamnesisDoctorPairs = new List<AnamnesisDoctorPair>();
            foreach (var anamnesis in medicalRecord.medicalHistory)
            {
                var doctor = GetDoctorById(anamnesis.doctorId);
                var anamnesisDoctorPair = new AnamnesisDoctorPair(anamnesis, doctor);
                anamnesisDoctorPairs.Add(anamnesisDoctorPair);
            }
            return anamnesisDoctorPairs;
        }

        private Doctor GetDoctorById(int doctorId)
        {
            return MainRepository.doctors.FirstOrDefault(doctor => doctor.id == doctorId);
        }

        private void FillPatientMedicalRecordData(MedicalRecord medicalRecord)
        {
            id_txt.Text = medicalRecord.id.ToString();
            height_txt.Text = medicalRecord.height.ToString();
            weight_txt.Text = medicalRecord.weight.ToString();
            allergens_txt.Text = medicalRecord.allergens;
        }

        public MedicalRecord GetMedicalRecord()
        {
            int medicalRecordId = GetActivePatientMedicalRecordId();

            if (medicalRecordId == 0)
            {
                return null;
            }

            return GetMedicalRecordById(medicalRecordId);
        }

        private int GetActivePatientMedicalRecordId()
        {
            Patient activePatient = MainRepository.patients.FirstOrDefault(p => p.id == ActivePatient.acitveUser.id);

            return activePatient?.medicalRecordId ?? 0;
        }

        private MedicalRecord GetMedicalRecordById(int id)
        {
            return MainRepository.medicalRecords.FirstOrDefault(m => m.id == id);
        }
    }
}
