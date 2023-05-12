using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Links;

namespace ZdravoCorp.Nurses
{
    public partial class WritePatientJson
    {
        public static Link link = new Link();

        public WritePatientJson() { }

        public static void WriteMedicalRecord()
        {
            string fileName = link.medicalRecords;
            string jsonString = JsonSerializer.Serialize<List<MedicalRecord>>(MainRepository.medicalRecords);
            File.WriteAllText(fileName, jsonString);
        }
    }
}
