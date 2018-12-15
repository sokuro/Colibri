using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colibri.Models;

namespace Colibri.ViewModels
{
    public class UserSubscribeNotificationsViewModel
    {
        public ApplicationUserCategoryTypesSubscriber ApplicationUserCategoryTypesSubscriber { get; set; }
        public IEnumerable<ApplicationUserCategoryTypesSubscriber> UserSubscriber { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }

        public IEnumerable<CategoryTypes> CategoryTypes { get; set; }


        public string ApplicationUserId { get; set; }
        public int CategoryTypeId { get; set; }

        // Custom Pagination
        public PagingInfo PagingInfo { get; set; }
    }
}
