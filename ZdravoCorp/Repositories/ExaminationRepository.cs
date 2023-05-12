using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Links;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Repositories
{
    public class ExaminationRepository
    {
        Link link;
        public ExaminationRepository(Link link)
        {
            this.link = link;
        }
        public List<Examination> getAll()
        {
            return JsonConvert.DeserializeObject<List<Examination>>(File.ReadAllText(link.examinations));
        }

    }
}
