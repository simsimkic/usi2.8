using System;
using System.Collections.Generic;
using System.Configuration;
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
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Nurses
{
    /// <summary>
    /// Interaction logic for CRUDPatients.xaml
    /// </summary>
    public partial class CRUDPatients : Window
    {
        List<Patient> patients;
        public CRUDPatients()
        {
            patients = MainRepository.patients;
            InitializeComponent();
            SetTitle("Patients");
            LoadData();
        }

        private void SetTitle(string title)
        {
            CRUDPatientsWindow.Title = title;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonMedicalRecord_Click(object sender, RoutedEventArgs e)
        {
            int patientId = FindPatiendId(DataGridPatients.SelectedIndex);
            if (patientId == -1)
            {
                MessageBox.Show("ERROR!", "Error");
            }
            else
            {
                MedicalRecordWindow medicalRecordWindow = new MedicalRecordWindow(patientId);
                medicalRecordWindow.Show();
            }
            
        }

        private int FindPatiendId(int index)
        {
            for(int i = 0; i < patients.Count; i++)
            {
                if (i == index)
                {
                    return patients[i].id;
                }
            }
            return -1;
        }

        public void LoadData()
        {
            DataGridPatients.Items.Clear();
            // check if there is a patient
            for (int i = 0; i < patients.Count; i++)
            {
                DataGridPatients.Items.Add(patients[i]);
            }
        }

        private void ButtonDeletePatient_Click(object sender, RoutedEventArgs e)
        {
            Delete(DataGridPatients.SelectedIndex);

            LoadData();
        }

        private void DeleteUser()
        {

        }

        private void Delete(int index)
        {
            if (index == -1)
            {
                MessageBox.Show("You must select patient from the table!", "Error");
            }
            else
            {
                Patient patient = null;
                for (int i = 0; i < patients.Count; i++)
                {
                    if (i == index)
                    {
                        patient = patients[i];
                        patients.RemoveAt(i);
                        break;
                    }
                }
                MainRepository.patients = patients;
                for(int i = 0; i < MainRepository.users.Count; i++)
                {
                    if (MainRepository.users[i].id == patient.id)
                    {
                        MainRepository.users.RemoveAt(i);
                        break;
                    }
                }
                for(int i = 0; i < MainRepository.medicalRecords.Count; i++)
                {
                    if (MainRepository.medicalRecords[i].id == patient.medicalRecordId)
                    {
                        MainRepository.medicalRecords.RemoveAt(i);
                        break;
                    }
                }
                MessageBox.Show("You have successfully deleted the patient account!", "Delete account");
            }
        }

        private void ButtonAddPattint_Click(object sender, RoutedEventArgs e)
        {
            PatientDataWindow patientDataWindow = new PatientDataWindow(this);
            patientDataWindow.Show();
            LoadData();
        }

        private void ButtonUpdatePatient_Click(object sender, RoutedEventArgs e)
        {
            int index = DataGridPatients.SelectedIndex;
            if (index == -1)
            {
                MessageBox.Show("You must select patient from the table!", "Error");
            }
            else
            {
                PatientDataWindow patientDataWindow = new PatientDataWindow(patients[index], this);
                patientDataWindow.Show();
                
            }
        }
    }
}
