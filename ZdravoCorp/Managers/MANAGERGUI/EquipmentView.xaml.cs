using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
using ZdravoCorp.Managers;
using System.CodeDom.Compiler;

namespace ZdravoCorp.Managers.MANAGERGUI

{
    
    public partial class EquipmentView : Window
    {
       public  List<Equipment> equipment;
      
        public EquipmentView()
        {
            InitializeComponent();
            equipment = MainRepository.equipment;
            Fill();
        }

        public void Fill()
        { 
                EquipmentGrid.ItemsSource=equipment;
        }

        private void SearchTextBoxInput(object sender, TextChangedEventArgs e)
        {
            var searchtext = sender as TextBox;
            if (searchtext.Text != "")
            {
                var searchMatches = Filter.FindSearchMatches(searchtext,equipment);
                EquipmentGrid.ItemsSource = null;
                EquipmentGrid.ItemsSource = searchMatches;
            }
            else
            {
                Fill();
            }

        }

    

        //filter by equipment type
        private void EquipmentTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
    
            if (e.AddedItems.Count > 0)
            {
                FilterByEquipmentType(e);
            }
            else
            {
                Fill();
            }
        
        }
        private void FilterByEquipmentType(SelectionChangedEventArgs e)
        {
            Fill();
            var selectedValue = e.AddedItems[0].ToString().ToUpper();

            
            if (notInWarehouse.IsChecked == true)
            {
               var  results = Filter.GetResultsFilteringByEquipmentType(e, EquipmentGrid, true);
                UpdateDataGrid(results);
            }
            else
            {
                //filter the equipment that isn't in warehouse
                var results = Filter.GetResultsFilteringByEquipmentType(e, EquipmentGrid, false);
                UpdateDataGrid(results);
            }
        }

        private void QuantitySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Fill();
            if (e.AddedItems.Count > 0)
            {
                FilterByQuantity(e);
            }
            else
            {
                Fill();
            }

        }

        private void FilterByQuantity(SelectionChangedEventArgs e)
        {
            Fill();

            string? selectedValue = e.AddedItems[0].ToString();

            var bounds = Filter.GetQuantityMatchBounds(selectedValue);

            int lowerBound = bounds.Item1;
            int upperBound = bounds.Item2;
           if (notInWarehouse.IsChecked == true)
            {
                var filteredData =Filter.GetResultsFilteringByQuantity(lowerBound, upperBound,true,EquipmentGrid);
                UpdateDataGrid(filteredData);

            }
            else
            {
                //filter the equipment that isnt in warehouse
                var filteredData = Filter.GetResultsFilteringByQuantity(lowerBound, upperBound, false,EquipmentGrid);
                UpdateDataGrid(filteredData);
            }
            
        }
      
        private void UpdateDataGrid(IEnumerable<Equipment>? filteredData)
        {
            EquipmentGrid.ItemsSource = filteredData; // update the datagrid's data source with the filtered data
        }

        private void ViewAllButtonClick(object sender, RoutedEventArgs e)
        {
            Fill();
        }

        private void RoomTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Fill();
            if (e.AddedItems.Count > 0)
            {
               FilterByRoomType(e);
            }
            else
            {
                Fill();
            }
        }

     private void FilterByRoomType(SelectionChangedEventArgs e)
        {
            Fill();
            var selectedValue = e.AddedItems[0].ToString().ToUpper();

            if (notInWarehouse.IsChecked == true)
            {
                var filteredData = Filter.GetResultsOfRoomTypeFilter(selectedValue,true,EquipmentGrid);
                UpdateDataGrid(filteredData);
            }
            else
            {
                //filter the equipment that isnt in warehouse
                var filteredData = Filter.GetResultsOfRoomTypeFilter(selectedValue, false,EquipmentGrid);
                UpdateDataGrid(filteredData);
            }
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
