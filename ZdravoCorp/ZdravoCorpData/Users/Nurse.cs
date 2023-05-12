using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.ZdravoCorpData.Users
{
    public class Nurse : User
    {
        public Nurse(int id, string name, string lastname, string username, string password, string phoneNumber, GENDER gender, ROLE role) : base(id, name, lastname, username, password, phoneNumber, gender, role)
        {
        }
    }
}
