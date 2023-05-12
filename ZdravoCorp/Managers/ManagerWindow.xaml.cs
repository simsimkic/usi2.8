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
using ZdravoCorp.Managers.MANAGERGUI;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp
{
    /// <summary>
    /// Interaction logic for Manager.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        public ManagerWindow(User user)
        {
            InitializeComponent();
            managerWindow.Title = user.username;
        }

        public void btnExitProgram_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new EquipmentView().Show();

        }


        private void LogOut_Button_Click_(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Succesffully logged out. ", "Log out");
            new MainWindow().Show();
            this.Close();
        }

        private void ViewRooms_Button_Click(object sender, RoutedEventArgs e)
        {
            new RoomsView().Show();
        }

        private void ViewWarehouseEquipmentState_Button_Click(object sender, RoutedEventArgs e)
        {
            new WarehouseState().Show();
        }

        private void Equipment_Procurement_Button_Click(object sender, RoutedEventArgs e)
        {
            new EquipmentProcurementView().Show();
        }

        private void equipmentTransferButton_Click(object sender, RoutedEventArgs e)
        {
            new EquipmentTransferView().Show();
        }
    }
}
