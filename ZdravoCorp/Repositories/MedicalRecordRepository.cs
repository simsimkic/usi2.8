using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Nurses;
using ZdravoCorp.ZdravoCorpData.Links;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Repositories
{
    public class MedicalRecordRepository
    {
        Link link;
        public MedicalRecordRepository(Link link)
        {
            this.link = link;
        }
        public List<MedicalRecord> getAll()
        {
            return JsonConvert.DeserializeObject<List<MedicalRecord>>(File.ReadAllText(link.medicalRecords));
        }

    }
}
