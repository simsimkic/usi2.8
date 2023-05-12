using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.ZdravoCorpData.Links;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Repositories
{
    public class UserRepository
    {
        Link link;
        public UserRepository(Link link)
        {
            this.link = link;
        }
        public List<User> getAll()
        {
            return JsonConvert.DeserializeObject<List<User>>(File.ReadAllText(link.users));
        }

    }
}
