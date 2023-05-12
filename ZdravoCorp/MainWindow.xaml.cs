using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Text.Json;
using ZdravoCorp.ZdravoCorpData.Users;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Links;
using ZdravoCorp.ZdravoCorpData.Equipments;
using ZdravoCorp.ZdravoCorpData.Rooms;
using ZdravoCorp.Nurses;
using ZdravoCorp.Repositories;
using System.Configuration;
using ZdravoCorp.ZdravoCorpData.Activities;

namespace ZdravoCorp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    

    public partial class MainWindow : Window
    {

        public MainWindow()
        { 
            InitializeComponent();
            MainRepository.Load();
            EquipmentProcurementRequestRepository.handleRequests();
            EquipmentTransferRepository.handleRequests();

        }

        private void btnExitProgram_Click(object sender, EventArgs e)
        {   
            //MainRepository.Save();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LogIn();
        }

        public (string, string) GetInputData()
        {
            string userName = username.Text;
            string pword = password.Password.ToString();
            return (userName, pword);
        }


        public void OpenWindow(User user)
        {
            string message = "Successfully logged in as ";

            switch (user.role)
            {
                case ROLE.MANAGER:
                    MessageBox.Show(message + "manager, " + user.name + " " + user.lastname + "!", "Welcome");
                    SendNotifications(user);
                    new ManagerWindow(user).Show();
                    this.Close();
                    break;

                case ROLE.DOCTOR:
                    MessageBox.Show(message + "doctor, " + user.name + " " + user.lastname + "!", "Welcome");
                    SendNotifications(user);
                    new DoctorWindow(user).Show();
                    this.Close();
                    break;

                case ROLE.NURSE:
                    MessageBox.Show(message + "nurse, " + user.name + " " + user.lastname + "!", "Welcome");
                    SendNotifications(user);
                    new NurseWindow(user).Show();
                    this.Close();
                    break;

                default:
                    MessageBox.Show(message + "patient, " + user.name + " " + user.lastname + "!", "Welcome");
                    SendNotifications(user);
                    new PatientWindow(user).Show();
                    this.Close();
                    break;
            }
        }

        public void SendNotifications(User user)
        {
            List<Notification> notifications = MainRepository.notifications;
            for(int i = 0; i < notifications.Count; i++)
            {
                if (notifications[i].userId == user.id)
                {
                    notifications[i].SendMessage();
                    MainRepository.notifications.Remove(notifications[i]);
                }
            }
            MainRepository.Save();
            MainRepository.Load();
        }

        public void LogIn()
        {
            var tuple = GetInputData();
            string usernameInput = tuple.Item1;
            string passwordInput = tuple.Item2;

            if (usernameInput == "" || passwordInput == "")
            {
                MessageBox.Show("Empty fields! Please fill  username and password field.");
                return;
            }

            User user = FindRole(usernameInput, passwordInput);
            if (user == null)
            {
                MessageBox.Show("Invalid username or password. Try again!");
                username.Clear();
                password.Clear();
                return;
            }

            OpenWindow(user);

        }


        public User? FindRole(string username, string password)
        {
            List<User> users = MainRepository.users;
            for (int i = 0; i <users.Count; i++)
            {
                if (username == users[i].username)
                {
                    if (password == users[i].password)
                    {
                        return users[i];
                    }
                }
            }
            return null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainRepository.Save();
        }
    }

}

