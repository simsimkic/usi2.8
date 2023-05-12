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
using ZdravoCorp.ZdravoCorpData.Equipments;
using ZdravoCorp.ZdravoCorpData.Rooms;

namespace ZdravoCorp.Managers.MANAGERGUI
{
    /// <summary>
    /// Interaction logic for WarehouseState.xaml
    /// </summary>
    public partial class WarehouseState : Window
    {
        public List<Equipment> wareHouseEquipment;
       
        public WarehouseState()
        {
            InitializeComponent();
            wareHouseEquipment = WarehouseFactory.GetEquipment();
            Fill();
        }
        public void Fill()
        {
            WarehouseEquipmentGrid.ItemsSource = wareHouseEquipment;
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
