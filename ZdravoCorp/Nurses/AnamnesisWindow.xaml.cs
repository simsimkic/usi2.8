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
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Nurses
{
    /// <summary>
    /// Interaction logic for AnamnesisWindow.xaml
    /// </summary>
    public partial class AnamnesisWindow : Window
    {
        private List<Doctor> _doctors;
        public Anamnesis anamnesis;
        private PatientDataWindow _patientDataWindow;
        private CreateMedicalRecord _createMedicalRecordWindow;

        public AnamnesisWindow(PatientDataWindow patientDataWindow)
        {
            InitializeComponent();
            _doctors = MainRepository.doctors;
            anamnesis = null;
            FillComboBox();
            this._patientDataWindow = patientDataWindow;
            this._createMedicalRecordWindow = null;
        }

        public AnamnesisWindow(CreateMedicalRecord createMedicalRecordWindow)
        {
            InitializeComponent();
            _doctors = MainRepository.doctors;
            anamnesis = null;
            FillComboBox();
            this._patientDataWindow = null;
            this._createMedicalRecordWindow = createMedicalRecordWindow;
        }

        private void FillComboBox() {
            
            for(int i = 0; i < _doctors.Count; i++)
                ComboBoxDoctor.Items.Add(_doctors[i].name + " " + _doctors[i].lastname);
            ComboBoxDoctor.SelectedIndex = 0;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            string dateString = DatePickerAnamnesis.Text;
            string[] dateData = dateString.Split('/');
            DateOnly date = new DateOnly(int.Parse(dateData[2]), int.Parse(dateData[1]), int.Parse(dateData[0]));
            string observation = TextBoxObservation.Text;
            int doctorId = _doctors[ComboBoxDoctor.SelectedIndex].id;
            anamnesis = new Anamnesis(observation, date, doctorId);
            MessageBox.Show("Successful add anamnesis!", "Add in medical history");
            AddInListView();
            this.Close();
        }

        private void AddInListView()
        {
            if (_createMedicalRecordWindow == null)
                _patientDataWindow.ListViewAnamnesis.Items.Add(anamnesis);
            else
                _createMedicalRecordWindow.ListViewAnamnesis.Items.Add(anamnesis);
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
