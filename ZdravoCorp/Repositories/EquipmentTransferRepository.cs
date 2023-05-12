using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZdravoCorp.ZdravoCorpData.Equipments;
using ZdravoCorp.ZdravoCorpData.Links;

namespace ZdravoCorp.Repositories
{
    public class EquipmentTransferRepository
    {
        public static List<Equipment> equipment = MainRepository.equipment;

        Link link;

        public static List<EquipmentTransferRequest> requests = MainRepository.equipmentTransferRequests;

        public EquipmentTransferRepository(Link link)
        {
            this.link = link;
        }
        public List<EquipmentTransferRequest> getAll()
        {
            return JsonConvert.DeserializeObject<List<EquipmentTransferRequest>>(File.ReadAllText(link.equipmentTransferRequests));
        }

        public static void scheduleTransfer(EquipmentTransferRequest request)
        {
            //scheduling transfer for static equipment
            MainRepository.equipmentTransferRequests.Add(request);
            MainRepository.Save();
            MessageBox.Show("Request sucessfully sent!");
        }

        public static  void transfer(string equipmentName,int fromRoomId,int toRoomId,int quantity)
        {
            var equipmentToTransfer=equipment.FirstOrDefault(e=>e.name==equipmentName && e.roomId==fromRoomId);
            if (equipmentToTransfer == null) { return; }
            equipmentToTransfer.quantity -= quantity;
            AddEquipmentToRoom(equipmentToTransfer, toRoomId, quantity);
            MainRepository.Save();
        }

        public static void AddEquipmentToRoom(Equipment selectedEquipment,int roomId,int selectedQuantity)
         { 
            var equipmentInRoom = equipment.FirstOrDefault(e => isAlreadyInRoom(e, roomId) && e.name == selectedEquipment.name && roomId == e.roomId);
            if (equipmentInRoom != null)
            {
                //if there is already a certain amount of equipment in the room
                equipmentInRoom.quantity += selectedQuantity;
                return;
            }
            var newEquipmentForRoom= new Equipment(
                selectedEquipment.name,
                selectedQuantity,
                roomId,
                selectedEquipment.isDynamic,
                EquipmentProcurementRequestRepository.getEquipmentType(selectedEquipment.name)
            );
            equipment.Add(newEquipmentForRoom);
        }
        public static  bool isAlreadyInRoom(Equipment selectedEquipment,int roomId)
        {
            return equipment.Any(e => e.name == selectedEquipment.name && e.roomId == roomId);
        }

        public static void handleRequests()
        {
            DateTime today = DateTime.Now;
            foreach(var request in MainRepository.equipmentTransferRequests)
            {
                if (today <= request.startTime) continue;
                request.started = true;
                if (today >= request.endTime && !request.done)
                {
                    transfer(request.name, request.fromRoomId, request.toRoomId, request.quantity);
                    request.done = true;
                }
            }
            MainRepository.Save();
        }

        public static List<Equipment> getLowQuantityEquipment()
        {
            List<Equipment> lowQuantityEquipment = new List<Equipment>();
            foreach (var item in equipment)
            {
                if (item.quantity <= 5)
                {
                    lowQuantityEquipment.Add(item);
                }
            }
            return lowQuantityEquipment;
        }
    }
}
