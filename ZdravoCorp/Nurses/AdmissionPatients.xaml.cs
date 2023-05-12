using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Windows;
using System.Windows.Navigation;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Nurses
{
    /// <summary>
    /// Interaction logic for AdmissionPatients.xaml
    /// </summary>
    public partial class AdmissionPatients : Window
    {
        public AdmissionPatients()
        {
            InitializeComponent();
            LoadExaminations();
        }

        public void LoadExaminations()
        {
            DateTime nowDateTime = DateTime.Now;
            List<Examination> examinations = MainRepository.examinations;
            for (int i = 0; i < examinations.Count; i++)
            {
                Admission admission = GetAdmission(examinations[i].id);
                if (examinations[i].status != Examination.Status.CANCELLED && examinations[i].timeSlot.start > nowDateTime && admission != null && admission.admission == false)
                {
                    DataGridAdmission.Items.Add(examinations[i]);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonAdmission_Click(object sender, RoutedEventArgs e)
        {
            int selectedExaminationId = DataGridAdmission.SelectedIndex;
            if (selectedExaminationId == -1)
            {
                MessageBox.Show("You have to select examination in list!", "Error");
            }
            else
            {
                CheckAdmission(selectedExaminationId);
            }
        }

        private void CheckAdmission(int selectedExaminationId)
        {
            Examination selectedExamination = FindNoneCancelledExamination(selectedExaminationId);
            if (selectedExamination != null)
            {
                DateTime nowDateTime = DateTime.Now;
                DateTime next15Minutes = nowDateTime.AddMinutes(15);
                if (next15Minutes > selectedExamination.timeSlot.start)
                {
                    if (selectedExamination.patientId == 0)
                    {
                        MessageBox.Show("You must to create account for this patient!");
                        PatientDataWindow patientDataWindow = new PatientDataWindow(this, selectedExamination);
                        patientDataWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("Start admission!");
                        CreateMedicalRecord createMedicalRecord = new CreateMedicalRecord(FindMedicalRecord(FindMedicalRecordIdForPatient(selectedExamination.patientId)), this, selectedExamination);
                        createMedicalRecord.Show();
                    }

                }
                else
                {
                    MessageBox.Show("Unable to start admission!");
                }
            }
        }
        private Examination FindNoneCancelledExamination(int selectedExaminationId)
        {
            DateTime nowDateTime = DateTime.Now;
            List<Examination> examinations = MainRepository.examinations;
            List<Admission> admissions = MainRepository.admissions;
            int noneCancelledId = 0;
            for (int i = 0; i < examinations.Count; i++)
            {
                Admission admission = GetAdmission(examinations[i].id);
                if (examinations[i].status != Examination.Status.CANCELLED && examinations[i].timeSlot.start > nowDateTime && admission != null && admission.admission == false)
                {
                    if (noneCancelledId == selectedExaminationId)
                    {
                        return examinations[i];
                    }
                    noneCancelledId++;
                }
            }
            return null;
        }

        private Admission GetAdmission(int examinationId)
        {
            List<Admission> admissions = MainRepository.admissions;
            for (int i = 0; i < admissions.Count; i++)
            {
                if (admissions[i].examinationId == examinationId)
                {
                    return admissions[i];
                }
            }
            return null;
        }

        private void CorrectAdmission(int examinationId)
        {
            List<Admission> admissions = MainRepository.admissions;
            for (int i = 0; i < admissions.Count; i++)
            {
                if (admissions[i].examinationId == examinationId)
                {
                    admissions[i].admission = true;
                    MainRepository.Save();
                    return;
                }
            }
        }

        private int FindMedicalRecordIdForPatient(int patientId)
        {
            List<Patient> patients = MainRepository.patients;
            for(int i = 0; i < patients.Count; i++)
            {
                if (patients[i].id == patientId)
                {
                    return patients[i].medicalRecordId;
                }
            }
            return -1;
        }

        private MedicalRecord FindMedicalRecord(int medicalRecordId)
        {
            List<MedicalRecord> medicalRecords = MainRepository.medicalRecords;
            for(int i = 0; i < medicalRecords.Count; i++)
            {
                if (medicalRecords[i].id == medicalRecordId)
                {
                    return medicalRecords[i];
                }
            }
            return null;
        }

        public void OverAdmission(Examination examination)
        {
            CorrectAdmission(examination.id);
            DataGridAdmission.Items.Clear();
            LoadExaminations();
        }
    }
}
