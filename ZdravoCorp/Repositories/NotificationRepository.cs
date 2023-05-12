using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.ZdravoCorpData.Activities;
using ZdravoCorp.ZdravoCorpData.Examinations;
using ZdravoCorp.ZdravoCorpData.Links;

namespace ZdravoCorp.Repositories
{
    public class NotificationRepository
    {

        Link link;
        public NotificationRepository(Link link)
        {
            this.link = link;
        }
        public List<Notification> getAll()
        {
            return JsonConvert.DeserializeObject<List<Notification>>(File.ReadAllText(link.notification));
        }
    }
}
