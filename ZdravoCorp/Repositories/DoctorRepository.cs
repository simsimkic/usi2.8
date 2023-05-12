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
    public class DoctorRepository
    {
        Link link;
        public DoctorRepository(Link link) {
            this.link = link;
        }
        public List<Doctor> getAll()
        {
            return JsonConvert.DeserializeObject<List<Doctor>>(File.ReadAllText(link.doctors));
        }

    }
}
