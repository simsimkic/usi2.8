using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Equipments;

namespace ZdravoCorp.Managers
{
    public class Filter
    {
       
       
        public static IEnumerable<Equipment> GetResultsOfRoomTypeFilter(string? selectedValue, bool notInWarehouse,DataGrid EquipmentGrid)
        {
            IEnumerable<Equipment> results = new List<Equipment>();
            List<string> selectedOptions = new List<string> { "EXAMINATION", "OPERATION", "INPATIENT", "WAITING" };
            foreach (var item in selectedOptions)
            {
                if (selectedValue.Contains(item))
                {
                    if (notInWarehouse)
                    {
                        //filtering equipment that isnt in warehouse
                        results = ((IEnumerable<Equipment>)EquipmentGrid.ItemsSource)
                    .Where(x => RoomRepository.FindbyId(x.roomId).type.ToString() == item && x.roomId != 0);
                    }
                    else
                    {
                        results = ((IEnumerable<Equipment>)EquipmentGrid.ItemsSource)
                 .Where(x => (RoomRepository.FindbyId(x.roomId)).type.ToString() == item);
                    }
                }

            }
            return results;
        }

        public static IEnumerable<Equipment>? GetResultsFilteringByEquipmentType(SelectionChangedEventArgs e,DataGrid EquipmentGrid,bool notInWarehouse)
        {
            var selectedValue = e.AddedItems[0].ToString().ToUpper();

            IEnumerable<Equipment>filteredData=new List<Equipment>();
            if (notInWarehouse)
            {
                filteredData = ((IEnumerable<Equipment>)EquipmentGrid.ItemsSource)
                 .Where(x => selectedValue.Contains(x.type.ToString()) && x.roomId != 0); // apply the filter using LINQ
             
            }
            else
            {
                //filter the equipment that isnt in warehouse
                filteredData = ((IEnumerable<Equipment>)EquipmentGrid.ItemsSource)
              .Where(x => selectedValue.Contains(x.type.ToString())); // apply the filter using LINQ
               
            }
            return filteredData;
        }

     public static IEnumerable<Equipment> FindSearchMatches(TextBox? tbx,List<Equipment>equipment)
        {
            var searchTerm = tbx.Text.ToLower();


            var searchMatches = equipment.Where(equipmentItem =>
                equipmentItem.name.ToLower().Contains(searchTerm) ||
                equipmentItem.roomId.ToString() == searchTerm ||
                equipmentItem.type.ToString().ToLower().Contains(searchTerm) ||
                equipmentItem.quantity.ToString() == searchTerm
            );
            return searchMatches;
        }


       public static IEnumerable<Equipment>? GetResultsFilteringByQuantity(int lowerBound, int upperBound,
            bool notInWarehouse,DataGrid EquipmentGrid)
        {
            IEnumerable<Equipment> filteredData = null;
            if (notInWarehouse)
            {

                filteredData = ((IEnumerable<Equipment>)EquipmentGrid.ItemsSource)
                .Where(x => x.roomId != 0 && (x.quantity >= lowerBound && x.quantity <= upperBound)); // apply the filter using LINQ

            }
            else
            {
                //filter the equipment that isnt in warehouse
                filteredData = ((IEnumerable<Equipment>)EquipmentGrid.ItemsSource)
               .Where(x => (x.quantity >= lowerBound && x.quantity <= upperBound)); // apply the filter using LINQ

            }
            return filteredData;
        }

        public static (int, int) GetQuantityMatchBounds(string? selectedValue)
        {

            if (selectedValue.Contains("out of stock"))
            {
                return (0, 0);
            }
            else if (selectedValue.Contains("0-10"))
            {
                return (0, 10);
            }
            else
            {
                return (10, int.MaxValue);
            }
        }
    }
}
