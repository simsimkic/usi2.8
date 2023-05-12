using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.ZdravoCorpData.Links;
using ZdravoCorp.ZdravoCorpData.Rooms;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Repositories
{
    public class RoomRepository
    {
        Link link;
        public RoomRepository(Link link)
        {
            this.link = link;
        }
        public List<Room> getAll()
        {
            return JsonConvert.DeserializeObject<List<Room>>(File.ReadAllText(link.rooms));
        }

        public static Room FindbyId(int id)
        {
            List<Room> rooms = MainRepository.rooms;
            for(int i=0;i<rooms.Count; i++)
            {
                if (rooms[i].id== id)
                {
                    return rooms[i];

                }
            }
            return null;
        }


    }
}
