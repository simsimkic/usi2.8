using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Equipments;

namespace ZdravoCorp.Managers
{
    public class EquipmentQuantityCalculations
    {
        
        public static List<Equipment> equipment=MainRepository.equipment;
       
        public static List<EquipmentTotalQuantity> LoadEquipmentTotalQuantities()
        {
            List<EquipmentTotalQuantity> equipmentTotalQuantities = new List<EquipmentTotalQuantity>();
            foreach(var item in equipment)
            {
                if (!isAlreadyIncluded(equipmentTotalQuantities,item.name)&& item.isDynamic)
                {
                    equipmentTotalQuantities.Add(new EquipmentTotalQuantity
                   (item.name, calculateEquipmentTotalQuantity(item.name)));
                }
            }
            return equipmentTotalQuantities;
        }

        public static bool isAlreadyIncluded(List<EquipmentTotalQuantity> equipmentTotalQuantities, string equipmentName)
        {
            return equipmentTotalQuantities.Any(e => e.name == equipmentName);
        }
        public static  int calculateEquipmentTotalQuantity(string equipmentName)
        {
            int totalquantity = 0;
            foreach (var item in equipment)
            {
                if (item.name == equipmentName)
                {
                    totalquantity += item.quantity;
                }
            }
            return totalquantity;
        }
       
    }
}
