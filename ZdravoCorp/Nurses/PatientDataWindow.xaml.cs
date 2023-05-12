using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Users;

namespace ZdravoCorp.Nurses
{
    /// <summary>
    /// Interaction logic for PatientDataWindow.xaml
    /// </summary>
    public partial class PatientDataWindow : Window
    {
        private List<User> users;
        private bool actionButton;
        private Patient patient;
        private MedicalRecord medicalRecord;
        private CRUDPatients crudPatients;
        private AdmissionPatients _admissionPatients;
        private Examination _examination;

        public PatientDataWindow(CRUDPatients crudPatients)
        {
            InitializeComponent();
            SetTitleWindow("Add a new patient");
            ClearTextBoxs();
            users = MainRepository.users;
            actionButton = true;
            patient = null;
            medicalRecord = null;
            this.crudPatients = crudPatients;
            this._admissionPatients = null;
            this._examination = null;
        }

        public PatientDataWindow(Patient patient, CRUDPatients crudPatients)
        {
            InitializeComponent();
            SetTitleWindow("Update a patient");
            this.patient = patient;
            this.medicalRecord = getMedicalRecord(this.patient);
            FillTextBoxs(patient);
            users = MainRepository.users;
            actionButton = false;
            this.crudPatients = crudPatients;
            this._admissionPatients = null;
            this._examination = null;
        }

        public PatientDataWindow(AdmissionPatients admissionPatients, Examination examination)
        {
            InitializeComponent();
            SetTitleWindow("Add a new patient");
            ClearTextBoxs();
            users = MainRepository.users;
            actionButton = true;
            patient = null;
            medicalRecord = null;
            this.crudPatients = null;
            this._admissionPatients = admissionPatients;
            this._examination = examination;
        }
        private MedicalRecord getMedicalRecord(Patient patient)
        {
            List<MedicalRecord> medicalRecords = MainRepository.medicalRecords;
            for(int i = 0; i < medicalRecords.Count; i++)
            {
                if (medicalRecords[i].id == patient.medicalRecordId)
                {
                    return medicalRecords[i];
                }
            }
            return null;
        }
        private void ClearTextBoxs()
        {
            TextBoxName.Clear();
            TextBoxLastname.Clear();
            TextBoxUsername.Clear();
            TextBoxPassword.Clear();
            TextBoxPhoneNumber.Clear();
            TextBoxHeight.Clear();
            TextBoxWeight.Clear();
            TextBoxAllergens.Clear();
            ListViewAnamnesis.Items.Clear();
        }

        private void FillTextBoxs(Patient patient)
        {
            TextBoxName.Text = patient.name;
            TextBoxLastname.Text = patient.lastname;
            TextBoxUsername.Text = patient.username;
            TextBoxPassword.Text = patient.password;
            TextBoxPhoneNumber.Text = patient.phoneNumber;
            if (patient.gender == GENDER.FEMALE)
                ComboBoxGender.SelectedItem = 1;
            TextBoxHeight.Text = medicalRecord.height.ToString();
            TextBoxWeight.Text = medicalRecord.weight.ToString();
            TextBoxAllergens.Text = medicalRecord.allergens;
            for (int i = 0; i < medicalRecord.medicalHistory.Count; i++)
            {
                ListViewAnamnesis.Items.Add(medicalRecord.medicalHistory[i]);
            }
        }

        private void SetTitleWindow(string title)
        {
            patientDataWindow.Title = title;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            if (actionButton)
            {
                if (CheckDataAdd())
                {
                    if(crudPatients == null)
                    {
                        MessageBox.Show("Successful registration and admisson!", "Created account and admission is over");
                        SetPatientValues();
                        MainRepostiroyListsAdd();
                        _admissionPatients.OverAdmission(_examination);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Successful registration!", "Created account for patient");
                        SetPatientValues();
                        MainRepostiroyListsAdd();
                        crudPatients.LoadData();
                        ClearTextBoxs();
                    }
                }
            }
            else
            {
                if(CheckDataUpdate(patient.username))
                {
                    MessageBox.Show("Successful change!", "Update account for patient");
                    SetPatientValues();
                    ChangeMainRepository();
                    MainRepostiroyListsAdd();
                    crudPatients.LoadData();
                    ClearTextBoxs();
                }
            }
        }

        private void MainRepostiroyListsAdd()
        {
            MainRepository.medicalRecords.Add(medicalRecord);
            MainRepository.patients.Add(patient);
            User user = new User(patient.id, patient.name, patient.lastname, patient.username, patient.password, patient.phoneNumber, patient.gender, patient.role);
            MainRepository.users.Add(user);
        }

        private void ChangeMainRepository()
        {
            for (int i = 0; i < MainRepository.medicalRecords.Count; i++)
            {
                if (MainRepository.medicalRecords[i].id == medicalRecord.id)
                {
                    MainRepository.medicalRecords.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < MainRepository.patients.Count; i++)
            {
                if (MainRepository.patients[i].id == patient.id)
                {
                    MainRepository.patients.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < MainRepository.patients.Count; i++)
            {
                if (MainRepository.patients[i].id == patient.id)
                {
                    MainRepository.patients.RemoveAt(i);
                    break;
                }
            }
        }

        private bool CheckDataAdd()
        {
            if (!CheckDataNameOrLastname(TextBoxName.Text))
            {
                MessageBox.Show("The name should contain only letters!", "Error");
                return false;
            }
            if (CheckDataEmptyField(TextBoxName.Text))
            {
                MessageBox.Show("You must fill in the name field!", "Error");
                return false;
            }
            if (!CheckDataNameOrLastname(TextBoxLastname.Text))
            {
                MessageBox.Show("The surname should contain only letters!", "Error");
                return false;
            }
            if (CheckDataEmptyField(TextBoxLastname.Text))
            {
                MessageBox.Show("You must fill in the surname field!", "Error");
                return false;
            }
            if (!CheckDataUsername(TextBoxUsername.Text))
            {
                MessageBox.Show("Username can have a maximum of 11 characters, which are letters or numbers.", "Error");
                return false;
            }
            if (!CheckDataUsernameAlreadyExists(TextBoxUsername.Text))
            {
                MessageBox.Show("This username already exists!", "Error");
                return false;
            }
            if (!CheckDataPassword(TextBoxPassword.Text))
            {
                MessageBox.Show("The password must have more than 4 characters!", "Error");
                return false;
            }
            if (!CheckDataPhoneNumber(TextBoxPhoneNumber.Text))
            {
                MessageBox.Show("The phone number contains only numbers and is [9,11] long.", "Error");
                return false;
            }
            if (!CheckDataNumber(TextBoxHeight.Text))
            {
                MessageBox.Show("You must enter a number for the height!", "Error");
                return false;
            }
            if (!CheckDataNumber(TextBoxWeight.Text))
            {
                MessageBox.Show("You must enter a number for the weight!", "Error");
                return false;
            }
            return true;
        }

        private bool CheckDataUpdate(string usernamePatient)
        {
            if (!CheckDataNameOrLastname(TextBoxName.Text))
            {
                MessageBox.Show("The name should contain only letters!", "Error");
                return false;
            }
            if (CheckDataEmptyField(TextBoxName.Text))
            {
                MessageBox.Show("You must fill in the name field!", "Error");
                return false;
            }
            if (!CheckDataNameOrLastname(TextBoxLastname.Text))
            {
                MessageBox.Show("The surname should contain only letters!", "Error");
                return false;
            }
            if (CheckDataEmptyField(TextBoxLastname.Text))
            {
                MessageBox.Show("You must fill in the surname field!", "Error");
                return false;
            }
            if (!CheckDataUsername(TextBoxUsername.Text))
            {
                MessageBox.Show("Username can have a maximum of 11 characters, which are letters or numbers.", "Error");
                return false;
            }
            if (TextBoxUsername.Text != usernamePatient && !CheckDataUsernameAlreadyExists(TextBoxUsername.Text))
            {
                MessageBox.Show("This username already exists!", "Error");
                return false;
            }
            if (!CheckDataPassword(TextBoxPassword.Text))
            {
                MessageBox.Show("The password must have more than 4 characters!", "Error");
                return false;
            }
            if (!CheckDataPhoneNumber(TextBoxPhoneNumber.Text))
            {
                MessageBox.Show("The phone number contains only numbers and is [9,11] long!", "Error");
                return false;
            }
            if (!CheckDataNumber(TextBoxHeight.Text))
            {
                MessageBox.Show("You must enter a number for the height!", "Error");
                return false;
            }
            if (!CheckDataNumber(TextBoxWeight.Text))
            {
                MessageBox.Show("You must enter a number for the weight!", "Error");
                return false;
            }
            return true;
        }

        private bool CheckDataNameOrLastname(string fieldText)
        {
            for (int i = 0; i < fieldText.Length; i++)
                if (!Char.IsLetter(fieldText[i])) 
                    return false;
            return true;
        }

        private bool CheckDataEmptyField(string fieldText)
        {
            return fieldText == "";
        }

        private bool CheckDataUsername(string username)
        {
            string pattern = @"^[a-zA-Z][a-zA-Z0-9]{4,11}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(username);
        }

        private bool CheckDataUsernameAlreadyExists(string username)
        {
            for(int i = 0; i < users.Count; i++)
                if (users[i].username == username) return false;
            return true;
        }

        private bool CheckDataPassword(string password)
        {
            return password.Length >= 5; 
        }

        private bool CheckDataPhoneNumber(string phoneNumber) {
            if (phoneNumber.Length < 9 || phoneNumber.Length > 12)
                return false;
            for (int i = 0; i < phoneNumber.Length; i++)
                if (!Char.IsNumber(phoneNumber[i]))
                    if (i != 3 && phoneNumber[i] != '/')
                        return false;
            return true;
        }

        private bool CheckDataNumber(string numberString)
        {
            int numberInt;
            return int.TryParse(numberString, out numberInt);
        }

        private void SetPatientValues()
        {
            if (patient == null)
            {
                medicalRecord = new MedicalRecord(generateNextIdMedicalRecord(), int.Parse(TextBoxHeight.Text), int.Parse(TextBoxWeight.Text), readMedicalHistory(), null, TextBoxAllergens.Text);
                if (medicalRecord.allergens == "") medicalRecord.allergens = "None";
                patient = new Patient(generateNextIdUser(), TextBoxName.Text, TextBoxLastname.Text, TextBoxUsername.Text, TextBoxPassword.Text, TextBoxPhoneNumber.Text, ComboBoxGender.SelectedIndex == 0 ? GENDER.MALE : GENDER.FEMALE, ROLE.PATIENT, medicalRecord.id);
            }
            else
            {
                patient.name = TextBoxName.Text;
                patient.lastname = TextBoxLastname.Text;
                patient.username = TextBoxUsername.Text;
                patient.password = TextBoxPassword.Text;
                patient.phoneNumber = TextBoxPhoneNumber.Text;
                patient.role = ROLE.PATIENT;
                patient.gender = ComboBoxGender.SelectedIndex == 0 ? GENDER.MALE : GENDER.FEMALE;
                medicalRecord.height = int.Parse(TextBoxHeight.Text);
                medicalRecord.weight = int.Parse(TextBoxWeight.Text);
                medicalRecord.medicalHistory = readMedicalHistory();
                medicalRecord.allergens = TextBoxAllergens.Text;
                if (medicalRecord.allergens == "") medicalRecord.allergens = "None";
            }
        }

        private List<Anamnesis> readMedicalHistory()
        {
            List<Anamnesis> medicalHistoryList = new List<Anamnesis>();
            for (int i = 0; i < ListViewAnamnesis.Items.Count; i++)
            {
                Anamnesis medicalHistory = (Anamnesis)ListViewAnamnesis.Items.GetItemAt(i);
                medicalHistoryList.Add(medicalHistory);
            }
            return medicalHistoryList;
        }

        private int generateNextIdUser() {
            int id = 0;
            for(int i = 0; i < users.Count; i++)
                if (users[i].id > id)
                    id = users[i].id;
            return id + 1;
        }

        private int generateNextIdMedicalRecord()
        {
            int id = 0;
            List<MedicalRecord> medicalRecords = MainRepository.medicalRecords;
            for (int i = 0; i < medicalRecords.Count; i++)
                if (medicalRecords[i].id > id)
                    id = medicalRecords[i].id;
            return id + 1;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            AnamnesisWindow anamnesisWindow = new AnamnesisWindow(this);
            anamnesisWindow.Show();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            ListViewAnamnesis.Items.RemoveAt(ListViewAnamnesis.SelectedIndex);
        }
    }
}
