using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Nurses;
using ZdravoCorp.Repositories;

namespace ZdravoCorp.ZdravoCorpData.Users
  
{
    public class Patient : User
    {   
        public int medicalRecordId { get; set; }
        public bool isBlocked { get; set; }

        public int examinationsScheduledLastMonthCount;
        public int examinationsCancelledLastMonthCount;
        public int examinationsModifiedLastMonthCount;
        public Patient(int id, string name, string lastname, string username, string password, string phoneNumber, GENDER gender, ROLE role, int medicalRecordId) : base(id, name, lastname, username, password, phoneNumber, gender, role)
        {
            this.medicalRecordId = medicalRecordId;
            isBlocked = false;
            examinationsScheduledLastMonthCount = 0;
            examinationsCancelledLastMonthCount = 0;
            examinationsModifiedLastMonthCount = 0;
        }

    }
}
