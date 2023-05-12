
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.ZdravoCorpData.Equipments
{
    public class EquipmentTotalQuantity
    {
        public string name { get; set; }
        public int totalQuantity { get; set; }

        public EquipmentTotalQuantity(string name, int totalQuantity)
        {
            this.name = name;
            this.totalQuantity= totalQuantity;
        }
    }
}
