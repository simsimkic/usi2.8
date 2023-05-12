using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Managers;
using ZdravoCorp.ZdravoCorpData.Equipments;
using ZdravoCorp.ZdravoCorpData.Links;
using ZdravoCorp.ZdravoCorpData.Equipments;
using System.Windows;

namespace ZdravoCorp.Repositories
{
    public class EquipmentProcurementRequestRepository
    {
        Link link;
        public static List<Equipment> equipment = MainRepository.equipment;
        public static List<EquipmentProcurementRequest> requests = MainRepository.equipmentProcurementRequests;

        public EquipmentProcurementRequestRepository(Link link)
        {
            this.link = link;
        }
        public List<EquipmentProcurementRequest> getAll()
        {
            return JsonConvert.DeserializeObject<List<EquipmentProcurementRequest>>(File.ReadAllText(link.equipmentProcurementRequests));
        }

      
        public static void SendRequest(string equipmentName, string equipmentQuantity,DateTime dateSent)
        {
            MainRepository.equipmentProcurementRequests.Add(new EquipmentProcurementRequest(equipmentName, int.Parse(equipmentQuantity), dateSent,false));
            MainRepository.Save();
            MessageBox.Show("Request successfully sent!");
        }

        public static void handleRequests()
        {
            DateTime today = DateTime.Now;
            foreach(var request in requests) { 
                int daysPassed = (today - request.dateSent).Duration().Days;
                if (daysPassed>=1 && request.done==false)
                {
                    AddEquipmentToWarehouse(request);
                    request.done = true;
                }
            }
            MainRepository.Save();
        }

        public static bool IsAlreadyInWareHouse(Equipment equipmentCheck)
        {
            return equipment.Any(item=> item.name == equipmentCheck.name && item.roomId == 0);
        }

        public static void AddEquipmentToWarehouse(EquipmentProcurementRequest request)
        {
            foreach(var item in MainRepository.equipment)
            {
                if (IsAlreadyInWareHouse(item) && request.name == item.name && item.roomId == 0) {
                    item.quantity += request.quantity;
                    return;
                }
            }
                MainRepository.equipment.Add
                (new Equipment(request.name, request.quantity, 0, true, getEquipmentType(request.name)));
        }
        public static Equipment.Type getEquipmentType(string equipmentName)
        {
            Equipment equipment = getEquipmentByName(equipmentName);
            return equipment.type;
        }
        public static Equipment getEquipmentByName(string equipmentName)
        {
            foreach (var item in equipment)
            {
                if (item.name == equipmentName)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
