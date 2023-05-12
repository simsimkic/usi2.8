using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Equipments;


namespace ZdravoCorp.Managers.MANAGERGUI
{
    /// <summary>
    /// Interaction logic for EquipmentProcurementView.xaml
    /// </summary>
    public partial class EquipmentProcurementView : Window
    {
        public  List<EquipmentTotalQuantity> equipmentWithQuantity;
        public List<Equipment> equipment;
        public string selectedEquipment;
        public string selectedQuantity;
        public EquipmentProcurementView()
        {
            InitializeComponent();
            equipment = MainRepository.equipment;
            FillDataGridAndComboBox();
        }

        public void FillDataGridAndComboBox()
        {
            List<EquipmentTotalQuantity> equipmentToDisplay =GetEquipmentForView();
            EquipmentStateGrid.ItemsSource= equipmentToDisplay;
            foreach(var item in equipment)
            {   
                if(item.isDynamic && !selectEquipment.Items.Contains(item.name)){ selectEquipment.Items.Add(item.name); }
            } 
        }
        public static List<EquipmentTotalQuantity> GetEquipmentForView()
        {
            List<EquipmentTotalQuantity> results = new List<EquipmentTotalQuantity>();
            List<EquipmentTotalQuantity>equipmentWithQuantity = EquipmentQuantityCalculations.LoadEquipmentTotalQuantities();
            foreach (var item in equipmentWithQuantity)
            {
                if (item.totalQuantity < 5)
                {
                    results.Add(item);
                }
            }
            return results;
        }
        private void setSelectedEquipment(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                selectedEquipment = e.AddedItems[0].ToString();
            }
            else {
                selectedEquipment = "";
            }
       }

        private void setSelectedQuantity(object sender, TextChangedEventArgs e)
        {
            selectedQuantity=quantityTextBox.Text;
        }
        private bool isRequestValid()
        {
            if(selectedEquipment ==null || selectedQuantity==null ) 
                return false;
            if(Regex.IsMatch(selectedQuantity, @"^\d+$") == false) 
                return false; 
            
                return true;
        }

        private void requestButtonClick(object sender, RoutedEventArgs e)
        {
            if (isRequestValid()==false)
            {
                MessageBox.Show("Request is not valid, try again!");
                return;
            }
                DateTime dateSent = DateTime.Now;
                EquipmentProcurementRequestRepository.SendRequest(selectedEquipment, selectedQuantity,dateSent);
        }

        private void btnExitProgram_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
