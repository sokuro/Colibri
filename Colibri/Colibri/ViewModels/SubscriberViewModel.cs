using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colibri.Models;

namespace Colibri.ViewModels
{
    public class SubscriberViewModel
    {
        public Notifications Notifications { get; set; }

        //public List<string> MyMessage { get; set; }

        //public IEnumerable<Notifications> NotificationsEnumerable { get; set; }

        //public List<Notifications> NotificationList { get; set; }

        // Custom Pagination
        public PagingInfo PagingInfo { get; set; }
    }
}
