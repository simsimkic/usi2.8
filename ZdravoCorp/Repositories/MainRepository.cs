using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using ZdravoCorp.Nurses;
using ZdravoCorp.Patients;
using ZdravoCorp.ZdravoCorpData.Activities;
using ZdravoCorp.ZdravoCorpData.Equipments;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Links;
using ZdravoCorp.ZdravoCorpData.Rooms;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Repositories
{
    public static class MainRepository
    {

        public static Link link = new Link();
        public static List<User> users = new List<User>();
        public static List<Examination> examinations;
        public static List<Equipment> equipment;
        public static List<Room> rooms;
        public static List<Doctor> doctors;
        public static List<Patient> patients;
        public static List<MedicalRecord> medicalRecords;
        public static List<PatientRecord> patientRecords;
        public static List<Admission> admissions;
        public static List<EquipmentProcurementRequest> equipmentProcurementRequests;
        public static List<EquipmentTransferRequest> equipmentTransferRequests;

        public static List<Notification> notifications;

        public static List<ExamRoom> examrooms;

        public static DoctorRepository doctorRepository;
        public static PatientRepository patientRepository;
        public static ExaminationRepository examinationRepository;
        public static EquipmentRepository equipmentRepository;
        public static RoomRepository roomRepository;
        public static UserRepository userRepository;
        public static MedicalRecordRepository medicalRecordRepository;
        public static AdmissionRepository admissionRepository;
        public static PatientRecordRepository patientRecordRepository;
        public static EquipmentProcurementRequestRepository equipmentProcurementRequestRepository;
        public static EquipmentTransferRepository equipmentTransferRepository;
        public static NotificationRepository notificationRepository;
        public static ExamRoomsRepository examroomsRepository;


        public static void Load()
        {
            doctorRepository = new DoctorRepository(link);
            doctors = doctorRepository.getAll();

            patientRepository = new PatientRepository(link);
            patients = patientRepository.getAll();

            examinationRepository = new ExaminationRepository(link);
            examinations = examinationRepository.getAll();

            equipmentRepository = new EquipmentRepository(link);
            equipment = equipmentRepository.getAll();

            roomRepository = new RoomRepository(link);
            rooms = roomRepository.getAll();

            userRepository=new UserRepository(link);
            users=userRepository.getAll();


            patientRecordRepository = new PatientRecordRepository(link);
            patientRecords = patientRecordRepository.getAll();

            medicalRecordRepository = new MedicalRecordRepository(link);
            medicalRecords = medicalRecordRepository.getAll();

            admissionRepository = new AdmissionRepository(link);
            admissions = admissionRepository.getAll();

            examroomsRepository = new ExamRoomsRepository(link);
            examrooms = examroomsRepository.getAll();

            equipmentProcurementRequestRepository = new EquipmentProcurementRequestRepository(link);
            equipmentProcurementRequests= equipmentProcurementRequestRepository.getAll();

            equipmentTransferRepository=new EquipmentTransferRepository(link);
            equipmentTransferRequests= equipmentTransferRepository.getAll();

            notificationRepository = new NotificationRepository(link);
            notifications = notificationRepository.getAll();

        }

        public static void Save()
        {
            Serializer.saveJsonToFile<List<Doctor>>(doctors, link.doctors);
            Serializer.saveJsonToFile<List<Patient>>(patients, link.patients);
            Serializer.saveJsonToFile<List<Examination>>(examinations, link.examinations);
            Serializer.saveJsonToFile<List<MedicalRecord>>(medicalRecords, link.medicalRecords);
            Serializer.saveJsonToFile<List<Equipment>>(equipment, link.equipment);
            Serializer.saveJsonToFile<List<Room>>(rooms, link.rooms);
            Serializer.saveJsonToFile<List<User>>(users, link.users);
            Serializer.saveJsonToFile<List<PatientRecord>>(patientRecords, link.patientRecords);
            Serializer.saveJsonToFile<List<Admission>>(admissions, link.admissions);
            Serializer.saveJsonToFile<List<EquipmentProcurementRequest>>(equipmentProcurementRequests, link.equipmentProcurementRequests);
            Serializer.saveJsonToFile<List<EquipmentTransferRequest>>(equipmentTransferRequests, link.equipmentTransferRequests);
            Serializer.saveJsonToFile<List<Notification>>(notifications, link.notification);
            Serializer.saveJsonToFile<List<ExamRoom>>(examrooms, link.examrooms);
        }
    }
}