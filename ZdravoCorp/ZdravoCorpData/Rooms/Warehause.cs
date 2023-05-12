using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.ZdravoCorpData.Equipments;
using ZdravoCorp.ZdravoCorpData.Rooms;

namespace ZdravoCorp.ZdravoCorpData.Rooms
{
    public class Warehause
    {
        public  static List<Equipment> equipment { get; set; }
        public Dictionary<int,int> drugs { get; set; }  

        public Warehause(List<Equipment>equipment,Dictionary<int,int> drugs) {
            equipment = WarehouseFactory.GetEquipment();
            this.drugs = drugs;
        }
    }
}
