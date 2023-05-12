using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.ZdravoCorpData.Equipments
{
    public class EquipmentProcurementRequest
    {
        
        public string name { get; set; }
        public int quantity { get; set; }
        public DateTime dateSent { get; set; }
        public bool done { get; set; }

       public EquipmentProcurementRequest(string name, int quantity, DateTime dateSent,bool done)
        {
            this.name = name;
            this.quantity = quantity;
            this.dateSent = dateSent;
            this.done = done;
        }
    }
}
