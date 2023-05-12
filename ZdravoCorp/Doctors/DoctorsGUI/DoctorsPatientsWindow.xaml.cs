using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
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
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Equipments;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Doctors.DoctorsGUI
{
    /// <summary>
    /// Interaction logic for DoctorsPatientsWindow.xaml
    /// </summary>
    public partial class DoctorsPatientsWindow : Window
    {
        public int id;
        public ObservableCollection<Patient> doctorsPatients { get; set; }
        public ObservableCollection<Patient> allPatients { get; set; } = new ObservableCollection<Patient>(MainRepository.patients);

        public DoctorsPatientsWindow(int doctorId)
        {
            InitializeComponent();

            this.id = doctorId;
            setPatientsCollections();

            initializeTableElements();
           
        }

        private void setPatientsCollections()
        {
            this.allPatients = new ObservableCollection<Patient>(MainRepository.patients);
            this.doctorsPatients = DoctorsService.getPatients(this.id);
        }

        private void initializeTableElements()
        {
            AllPatients.ItemsSource = allPatients;
        }

        public  IEnumerable<Patient> findSearchMatches(TextBox? tbx) { 
           
            var searchTerm = tbx?.Text?.ToLower() ?? "";

            var searchMatches=allPatients.Where(patientItem=>patientItem.name.ToLower().Contains(searchTerm)||
                                                patientItem.lastname.ToLower().Contains(searchTerm)||
                                                patientItem.id.ToString()==searchTerm);
            return searchMatches; 
        }

        public void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)  //filter table
        {
            var tbx = sender as TextBox;
            if (tbx != null && !string.IsNullOrWhiteSpace(tbx.Text))
            {
                var searchMatches = findSearchMatches(tbx);
                AllPatients.ItemsSource = searchMatches;
            }
            else
            {
                initializeTableElements();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)  //show patients medical record button
        {
            Patient selectedItem = (Patient)AllPatients.SelectedItem;
            new MedicalRecordWindow(selectedItem.id).Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) //update patients medical record button
        {

            if (AllPatients.SelectedItem is not Patient selectedItem){ return; }

            if (doctorsPatients.Contains(selectedItem))
            {
                new UpdateMedicalRecordWindow(selectedItem.id, id).Show();
            }
            else
            {
                MessageBox.Show("This is not your patient!You can't update medical record");
            }
        }
    }
}
