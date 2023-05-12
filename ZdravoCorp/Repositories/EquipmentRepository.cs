using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.ZdravoCorpData.Equipments;
using ZdravoCorp.ZdravoCorpData.Links;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Repositories
{
    public class EquipmentRepository
    {
        Link link;
        public EquipmentRepository(Link link)
        {
            this.link = link;
        }
        public List<Equipment> getAll()
        {
            return JsonConvert.DeserializeObject<List<Equipment>>(File.ReadAllText(link.equipment));
        }

    }
}
