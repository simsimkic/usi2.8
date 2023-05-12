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
using ZdravoCorp.Doctors.DoctorsGUI;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp
{
    /// <summary>
    /// Interaction logic for DoctorWundow.xaml
    /// </summary>
    public partial class DoctorWindow : Window
    {
        public int id;
        public DoctorWindow(User user)
        {
            InitializeComponent();
            doctorWindow.Title = user.username;
            this.id= user.id;
        }

        public void btnExitProgram_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new SchedulingExaminationsWindow(id).Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            new DoctorExaminationWindow(id).Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            new MainWindow().Show();
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            new DoctorsPatientsWindow(id).Show();
        }
    }
}
