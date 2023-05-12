using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
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

namespace ZdravoCorp
{
    /// <summary>
    /// Interaction logic for Nurse.xaml
    /// </summary>
    public partial class NurseWindow : Window
    {
        public NurseWindow(User user)
        {
            InitializeComponent();
            nurseWindow.Title = user.username;
            LabelWelcome.Content = "Welcome, " + user.name + " " + user.lastname + "!";
        }
        public void btnExitProgram_Click(object sender, EventArgs e)
        {
            this.Close();
            MainRepository.Save();
        }

        private void ButtonLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MessageBox.Show("Successfully logged out!", "Log out");
            MainRepository.Save();
        }

        private void ButtonPatients_Click(object sender, RoutedEventArgs e)
        {
            CRUDPatients crudPatients = new CRUDPatients();
            crudPatients.Show();
        }

        private void ButtonReception_Click(object sender, RoutedEventArgs e)
        {
            AdmissionPatients admissionPatients = new AdmissionPatients();
            admissionPatients.Show();
        }

        private void ButtonEmergency_Click(object sender, RoutedEventArgs e)
        {
            EmergencyWindow emergencyWindow = new EmergencyWindow();
            emergencyWindow.Show();
        }
    }
}
