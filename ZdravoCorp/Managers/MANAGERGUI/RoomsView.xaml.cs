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
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Equipments;
using ZdravoCorp.ZdravoCorpData.Rooms;

namespace ZdravoCorp.Managers.MANAGERGUI
{
    /// <summary>
    /// Interaction logic for RoomsView.xaml
    /// </summary>
    public partial class RoomsView : Window
    {
        public List<Room> rooms;
       
        public RoomsView()
        {
            InitializeComponent();
            rooms = MainRepository.rooms;
            Fill();
        }
        public void Fill()
        {
            RoomsGrid.ItemsSource = rooms;
         }

        private void btnExitProgram_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
