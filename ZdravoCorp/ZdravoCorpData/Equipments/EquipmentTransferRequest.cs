using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.ZdravoCorpData.Equipments
{
    public  class EquipmentTransferRequest
    {
        public string name { get; set; }
        public int quantity { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }

    
        public int fromRoomId { get; set; }
        public int toRoomId { get; set; }
        public bool done { get; set; }
        public bool started { get; set; }

        public EquipmentTransferRequest(string name, int quantity, DateTime startTime, DateTime endTime, int fromRoomId, int toRoomId,bool started, bool done)
        {
            this.name = name;
            this.quantity = quantity;
            this.startTime= startTime;
            this.endTime = endTime;
            this.fromRoomId = fromRoomId;
            this.toRoomId = toRoomId;
            this.started = started;
            this.done = done;
        }
    }
}
