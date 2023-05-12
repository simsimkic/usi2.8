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
using ZdravoCorp.Nurses;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Doctors
{
    /// <summary>
    /// Interaction logic for MedicalRecordWindow.xaml
    /// </summary>
    public partial class MedicalRecordWindow : Window
    {
        public int patientId;
        public MedicalRecordWindow(int patientId)
        {
            InitializeComponent();

            this.patientId = patientId;

            MedicalRecord medicalRecord = MedicalRecord.get(patientId);
            DataContext = medicalRecord;

            List<Anamnesis> anamnesisList = medicalRecord.medicalHistory;
            anamnesis.ItemsSource = anamnesisList;
        }

        public int PatientId
        {
            get { return patientId; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
