using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Doctors;

namespace ZdravoCorp.ZdravoCorpData.Users
{
    public enum Specialty
    {
        CARDIOLOGIST,
        DERMATOLOGIST,
        SURGEON,
        OTORHINOLARYNGOLOGIST
    }
    public class Doctor : User
    {   
       
        public List<FreeDayRequest> freeDays;
        public Specialty specialty { get; set; }
        public double averageGrade;   //treba izracunati prosecnu ocenu 
        public Doctor(int id, string name, string lastname, string username, string password, string phoneNumber, GENDER gender, ROLE role, List<FreeDayRequest> freeDays,Specialty specialty) : base(id, name, lastname, username, password, phoneNumber, gender, role)
        {
            this.freeDays = freeDays;
            this.specialty = specialty;
            this.averageGrade = 0;  //treba je izracunati
        }
    }
}
