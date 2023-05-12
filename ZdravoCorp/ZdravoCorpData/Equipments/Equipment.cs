using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Repositories;

namespace ZdravoCorp.ZdravoCorpData.Equipments
{
    public class Equipment
    {
        public enum Type { EXAMINATION, OPERATION, HALLWAY, FURNITURE };
        public string name { get; set; }
        public int quantity { get; set; }
        public int roomId { get; set; }
        public bool isDynamic { get; set; }
        public Type type { get; set; }

        public Equipment(string name, int quantity, int roomId, bool isDynamic, Type type)
        {
            this.name = name;
            this.quantity = quantity;
            this.roomId = roomId;
            this.isDynamic = isDynamic;
            this.type = type;
        }

        public static List<Equipment> getDynamicFromRoom(int roomId)
        {
            return MainRepository.equipment.Where(e => e.isDynamic && e.roomId == roomId).ToList();
        }

        public static void updateQuantity(string equipmentName,int roomId ,int newQuantity)
        {
            Equipment? equipment = MainRepository.equipment.Find(e => e.name.Equals(equipmentName) && e.roomId==roomId);

            if (equipment == null) { return; }

            modifyQuantity(equipment, newQuantity);

        }

        public static void modifyQuantity(Equipment equipment,int newQuantity)
        {
            equipment.quantity = newQuantity;
            MainRepository.Save();
        }
    }
}
