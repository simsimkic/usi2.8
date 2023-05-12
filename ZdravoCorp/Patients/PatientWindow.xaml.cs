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
using ZdravoCorp.Patients;
using ZdravoCorp.Patients.PatientsGUI;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp
{
    /// <summary>
    /// Interaction logic for Patient.xaml
    /// </summary>
    public partial class PatientWindow : Window
    {
        public PatientWindow(User user)
        {
            InitializeComponent();
            patientWindow.Title = user.username;
            ActivePatient.acitveUser = user;
        }
        public void btnExitProgram_Click(object sender, EventArgs e)
        {
            this.Close();
            MainRepository.Save();
        }

        private void Button_CRUD(object sender, RoutedEventArgs e)
        {
            if (isPatientBlocked())
            {
                MessageBox.Show("You are blocked!", "Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            new CRUDWindow().Show();
            this.Close();
        }

        private void RecommendedExaminations_Click(object sender, RoutedEventArgs e)
        {
            if (isPatientBlocked())
            {
                MessageBox.Show("You are blocked!", "Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            new RecommendedFreeExaminationsWindow().Show();
            this.Close();
        }

        private void Anamnesis_Click(object sender, RoutedEventArgs e)
        {
            if (isPatientBlocked())
            {
                MessageBox.Show("You are blocked!", "Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            new AnamnesisSearch().Show();
            this.Close();
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
            MainRepository.Save();
        }

        private bool isPatientBlocked()
        {
            Patient patient = GetPatientById(ActivePatient.acitveUser.id);
            return patient.isBlocked;
        }

        private Patient GetPatientById(int patientId)
        {
            return MainRepository.patients.FirstOrDefault(p => p.id == patientId);
        }

    }
}
