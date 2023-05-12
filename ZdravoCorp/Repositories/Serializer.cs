using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Repositories
{
    public class Serializer
    {
        public static void saveJsonToFile<T>(T data, string filePath)
        {
            string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, jsonString);
        }
    }
}
