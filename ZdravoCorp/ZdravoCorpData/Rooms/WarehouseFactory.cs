using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Equipments;

namespace ZdravoCorp.ZdravoCorpData.Rooms
{
   public  class WarehouseFactory
    {
       
        public static List<Equipment> GetEquipment()
        {
            List<Equipment> allEquipment = MainRepository.equipment;
            List<Equipment>results = new List<Equipment>();
            for (int i = 0; i < allEquipment.Count; i++)
            {
                if (allEquipment[i].roomId== 0)
                {
                    results.Add(allEquipment[i]);
                }
            }
            return results;
        }
    }
}
