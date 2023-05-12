using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using ZdravoCorp.Patients;
using ZdravoCorp.ZdravoCorpData.Links;

namespace ZdravoCorp.Repositories
{
    public class PatientRecordRepository
    {
        Link link;

        public PatientRecordRepository(Link link)
        {
            this.link = link;
        }

        public List<PatientRecord> getAll()
        {
            return JsonConvert.DeserializeObject<List<PatientRecord>>(File.ReadAllText(link.patientRecords));
        }
    }
}
