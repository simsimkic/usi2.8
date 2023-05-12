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
    public class PatientRepository
    {
        Link link;
        public PatientRepository(Link link)
        {
            this.link = link;
        }
        public List<Patient> getAll()
        {
            return JsonConvert.DeserializeObject<List<Patient>>(File.ReadAllText(link.patients));
        }

    }
}
