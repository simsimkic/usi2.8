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
    public class ExamRoomsRepository
    {
        Link link;
        public ExamRoomsRepository(Link link)
        {
            this.link = link;
        }
        public List<ExamRoom> getAll()
        {
            return JsonConvert.DeserializeObject<List<ExamRoom>>(File.ReadAllText(link.examrooms));
        }
    }
}
