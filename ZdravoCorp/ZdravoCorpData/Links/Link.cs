using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.ZdravoCorpData.Links;

public class Link
{
    public string users { get; set; }
    public string examinations { get; set; }

    public string rooms { get; set; }

    public string equipment { get; set; }

    public string doctors { get; set; }

    public string patients { get; set; }

    public string medicalRecords { get; set; }

    public string patientRecords { get; set; }

    public string admissions { get; set; }
    public string equipmentProcurementRequests { get; set; }

    public string equipmentTransferRequests { get; set; }

    public string notification { get; set; }
    public string examrooms { get; set; }

    public Link() { 
        this.users = "../../../Data/users.json";
        this.examinations = "../../../Data/examinations.json";
        this.rooms = "../../../Data/rooms.json";
        this.equipment = "../../../Data/equipment.json";
        this.doctors = "../../../Data/doctors.json";
        this.patients = "../../../Data/patients.json";
        this.medicalRecords = "../../../Data/medicalRecords.json";
        this.patientRecords = "../../../Data/patientRecords.json";
        this.admissions = "../../../Data/admission.json";
        this.equipmentProcurementRequests = "../../../Data/equipmentProcurementRequest.json";
        this.equipmentTransferRequests = "../../../Data/equipmentTransferRequests.json";
        this.notification = "../../../Data/notification.json";
        this.examrooms= "../../../Data/exam_room.json";
    }

}
