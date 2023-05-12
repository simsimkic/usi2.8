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
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Doctors.DoctorsGUI
{
    /// <summary>
    /// Interaction logic for ActiveExaminationWindow.xaml
    /// </summary>
    public partial class ActiveExaminationWindow : Window
    {
        public Examination currentExamination;
        public int doctorId;
        public ActiveExaminationWindow(Examination examination,int doctorId)
        {
            this.currentExamination = examination;
            this.doctorId = doctorId;

            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e) //finish examination button
        {

            Examination.Finish(currentExamination);
            MessageBox.Show("Examination is successfully finished!");

            this.Close();

            new EquipmentStateUpdateWindow(currentExamination.id).Show();  //dynamic equipment review

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            new UpdateMedicalRecordWindow(currentExamination.patientId,doctorId ).Show();
            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            new MedicalRecordWindow(currentExamination.patientId).Show();
        }
    }
}
