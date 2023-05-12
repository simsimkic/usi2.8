using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.ZdravoCorpData.Users
{
    public enum GENDER { FEMALE, MALE };
    public enum ROLE { MANAGER, DOCTOR, NURSE, PATIENT };
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string phoneNumber { get; set; }
        public GENDER gender { get; set; }
        public ROLE role { get; set; }

        public User(int id, string name, string lastname, string username, string password, string phoneNumber, GENDER gender, ROLE role)
        {
            this.id = id;
            this.name = name;
            this.lastname = lastname;
            this.username = username;
            this.password = password;
            this.phoneNumber = phoneNumber;
            this.gender = gender;
            this.role = role;
        }
    }
}
