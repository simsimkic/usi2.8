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
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Doctors.DoctorsGUI
{
    /// <summary>
    /// Interaction logic for EquipmentStateUpdateWindow.xaml
    /// </summary>
    public partial class EquipmentStateUpdateWindow : Window
    {
        public int completedExaminationId;
        public int roomId;
        public EquipmentStateUpdateWindow(int examinationId)
        {
            InitializeComponent();

            this.completedExaminationId = examinationId;

            this.roomId = ExamRoom.get(examinationId);

            List<Equipment> availableDynamicEquipment = Equipment.getDynamicFromRoom(roomId);
            EquipmentComboBox.ItemsSource = availableDynamicEquipment;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Equipment? selectedEquipment = EquipmentComboBox.SelectedItem as Equipment;

            if (selectedEquipment == null)
            {
                MessageBox.Show("You didn't choose any equipment!");
                return;
            }

            if(!quantityValidation(selectedEquipment)) { return; }
         

            Equipment.updateQuantity(selectedEquipment.name,roomId ,int.Parse(newQuantity.Text));

            MessageBox.Show("Successfully changed!");
        }

        private bool quantityValidation(Equipment choosenEquipment)
        {

            if (!int.TryParse(newQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("The quantity is not valid!");
                return false;
            }

            if (choosenEquipment.quantity < quantity)
            {
                MessageBox.Show("The quantity is not valid!");
                return false;
            }
            return true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)  //show quantity button
        {
            Equipment? selectedEquipment = EquipmentComboBox.SelectedItem as Equipment;

            if (selectedEquipment != null)
            {
                CurrentQuantityTextBlock.Text = $" {selectedEquipment.quantity}";
            }
            else
            {
                MessageBox.Show("You didn`t choose equipment!");
                return;
            }
        }
    }
}
