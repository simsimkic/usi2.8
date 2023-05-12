using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Equipments;
using ZdravoCorp.ZdravoCorpData.Rooms;

namespace ZdravoCorp.Managers.MANAGERGUI
{
    /// <summary>
    /// Interaction logic for EquipmentSchedulingView.xaml
    /// </summary>
    public partial class EquipmentTransferView : Window
    {
       
        public List<Equipment> equipment;
        public List<Room> rooms;
        public string equipmentName;
        public string fromRoom;
        public string toRoom;
        public DateTime startDate;
        public DateTime endDate;
        public string startTime;
        public string endTime;
        public string selectedQuantity;
        public bool isDynamic = true;


        public EquipmentTransferView()
        {
            InitializeComponent();
            EquipmentTransferRepository.handleRequests();
            equipment = MainRepository.equipment;
            rooms=MainRepository.rooms;
            fillDataGrid();
            fillEquipmentComboBox();
            fillRoomsComboBoxes();
            hideElements();
        }

        public void hideElements()
        {
            EndTime.Visibility = Visibility.Hidden;
            EndTimeLabel.Visibility = Visibility.Hidden;
            EndDateLabel.Visibility = Visibility.Hidden;
            EndDate.Visibility = Visibility.Hidden;
            startDateLabel.Visibility = Visibility.Hidden;
            StartTime.Visibility=Visibility.Hidden;
            startTimeLabel.Visibility = Visibility.Hidden;
            StartDate.Visibility= Visibility.Hidden;
        }
        public void fillDataGrid()
        {
            List<Equipment> equipmentToDisplay = EquipmentTransferRepository.getLowQuantityEquipment();
            foreach(var equipment in equipmentToDisplay)
            {
                equipmentDataGrid.Items.Add(equipment);
            }
            markUpTableRows();
        }

        public void fillEquipmentComboBox()
        {
            foreach (var item in equipment)
            {
                if (!equipmentComboBox.Items.Contains(item.name))
                        equipmentComboBox.Items.Add(item.name);
            }
        }

        public void fillRoomsComboBoxes() { 
               foreach (var room in rooms) { 
                    string item = room.id + "    " + room.type.ToString();
                    fromRoomComboBox.Items.Add(item);
                    toRoomComboBox.Items.Add(item);
               }
        }

        private void markUpTableRows()
        {
            equipmentDataGrid.LoadingRow += (sender, e) =>
            {
                if (e.Row.DataContext is Equipment equipment && equipment.quantity == 0)
                {
                    e.Row.Background = new SolidColorBrush(Colors.Cyan);
                }
            };
        }

        private void setSelectedEquipment(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count > 0)
            {
                equipmentName = e.AddedItems[0].ToString();
            }
            else
            {
                equipmentName = "";
            }
            changeElementsVisibillity(equipmentName);
        }


        private void changeElementsVisibillity(string equipmentName)
        {
            Equipment equipment = EquipmentProcurementRequestRepository.getEquipmentByName(equipmentName);
            if (equipment != null && !equipment.isDynamic)
            {
                isDynamic = false;
                SetVisibility(false, Visibility.Visible);
            }
            else
            {
                SetVisibility(true, Visibility.Hidden);
            }
        }

        private void SetVisibility(bool IsDynamic, Visibility visibility)
        {
            EndTime.Visibility = visibility;
            EndTimeLabel.Visibility = visibility;
            EndDateLabel.Visibility = visibility;
            EndDate.Visibility = visibility;
            startDateLabel.Visibility = visibility;
            StartTime.Visibility = visibility;
            startTimeLabel.Visibility = visibility;
            StartDate.Visibility = visibility;
            isDynamic = IsDynamic;
        }

        private void setFromRoomId(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = e.AddedItems.Count > 0 ? e.AddedItems[0].ToString() : "";
            fromRoom = selectedItem.Split("    ")[0];
        }

        private void setToRoomId(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem;
            if (e.AddedItems.Count > 0)
            {
                selectedItem = e.AddedItems[0].ToString();
            }
            else
            {
                selectedItem = "";
            }
            toRoom = selectedItem.Split("    ")[0];
        }
     
        private void SendRequestButtonClick(object sender, RoutedEventArgs e)
        {
            setRequest();
            if (isRequestValid())
            {
                sendTransferRequest();
            }
            else
            {
                MessageBox.Show("Invalid request!");
            }
        }

        private void setRequest()
        {
            setSelectedDatesAndTimes();
            setSelectedQuantity();
        }
        private void sendTransferRequest()
        {
            if (isDynamic)
            {
                EquipmentTransferRepository.transfer
                    (equipmentName, Int32.Parse(fromRoom), Int32.Parse(toRoom), Int32.Parse(selectedQuantity));
                MessageBox.Show("Request successfully sent!");
            }
            else
            {
                //if equipment is static
                sendRequestForStaticEquipment();
            }
        }

        private void sendRequestForStaticEquipment()
        {
            string transferStartDate = startDate.ToString("yyyy-MM-dd");
            string transferEndDate = endDate.ToString("yyyy - MM - dd");
            DateTime transferStartTime = DateTime.Parse(transferStartDate + "T" + startTime);
            DateTime transferEndTime = DateTime.Parse(transferEndDate + "T" + endTime);
            EquipmentTransferRequest request = new EquipmentTransferRequest(equipmentName, Int32.Parse(selectedQuantity), transferStartTime, transferEndTime,
                                                                                                                            Int32.Parse(fromRoom), Int32.Parse(toRoom), false, false);
            EquipmentTransferRepository.scheduleTransfer(request);
            isDynamic = true;
        }

        private void setSelectedQuantity()
        {
            selectedQuantity = quantity.Text;
        }
        private void setSelectedDatesAndTimes()
        {
           
            if (!isDynamic)
            {
                if(StartDate.SelectedDate==null || EndDate.SelectedDate == null)
                {
                    MessageBox.Show("Dates not selected!");
                    return;
                }
                startDate = StartDate.SelectedDate.Value;
                endDate = EndDate.SelectedDate.Value;
                startTime = StartTime.Text;
                endTime = EndTime.Text;
            }
        }

       
        private bool isRequestValid()
        {
            if (!areInputsValid()) { return false; }
            Equipment equipment = EquipmentProcurementRequestRepository.getEquipmentByName(equipmentName);
            if (!roomContainsEquipment(equipmentName,Int32.Parse(fromRoom))) { return false; }
            if (Int32.Parse(selectedQuantity) > equipment.quantity) { return false; }
            if (equipment.isDynamic) { return true; }
            //if equipment isn't dynamic validate dates and times
             bool validTerms = validateDatesAndTimes();
             if (validTerms) { return true; }
             return false;
        }
        private bool areInputsValid()
        {
            if (equipmentName == "") { return false; }
            else if (fromRoom == "" || toRoom == "") { return false; }
            else if (Regex.IsMatch(selectedQuantity, @"^\d+$") == false) { return false; }
            else { return true; }
        }

        private bool roomContainsEquipment(string equipmentName, int roomId)
        {
            return equipment.Any(item => item.name == equipmentName && item.roomId == roomId && item.quantity >= 0);
        }

        private bool validateDatesAndTimes()
        {
           if(startDate==null ||  endDate==null || startDate>endDate ) {
                return false;
           }
            bool validTimes = validateTimes();
            return (validTimes);
        }

        private bool validateTimes()
        {
            if (startTime == "" || endTime=="")
            { 
               return false;
            }
            string pattern = @"^(?:[01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$";
            return (Regex.IsMatch(startTime, pattern) || !Regex.IsMatch(endTime, pattern));
        }

        private void btnExitProgram_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

 }
       


