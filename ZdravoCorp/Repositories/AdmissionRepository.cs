using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Links;

namespace ZdravoCorp.Repositories
{
    public class AdmissionRepository
    {

        Link link;
        public AdmissionRepository(Link link)
        {
            this.link = link;
        }
        public List<Admission> getAll()
        {
            return JsonConvert.DeserializeObject<List<Admission>>(File.ReadAllText(link.admissions));
        }
    }
}
