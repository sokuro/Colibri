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

        public IEnumerable<Notifications> NotificationsEnumerable { get; set; }

        public IEnumerable<CategoryTypes> CategoryTypes { get; set; }

        public ApplicationUserCategoryTypesSubscriber ApplicationUserCategoryTypesSubscriber { get; set; }

        public string CurrentUserId { get; set; }

        // for Filtering
        public string SearchFilter { get; set; }

        // Custom Pagination
        public PagingInfo PagingInfo { get; set; }
    }
}
