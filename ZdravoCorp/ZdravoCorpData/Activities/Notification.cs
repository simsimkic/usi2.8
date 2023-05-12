using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZdravoCorp.ZdravoCorpData.Activities
{
    public class Notification
    {
        public int userId { get; set; }
        public string message { get; set; }

        public Notification(int userId, string message)
        {
            this.userId = userId;
            this.message = message;
        }

        public void SendMessage()
        {
            MessageBox.Show(this.message);
        }
    }
}
