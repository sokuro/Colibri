using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colibri.Models;

namespace Colibri.ViewModels
{
    public class UserSubscribeNotificationsIndexViewModel
    {
        public IEnumerable<ApplicationUserCategoryTypesSubscriber> UserSubscriber { get; set; }

        public IEnumerable<CategoryTypes> CategoryTypes { get; set; }
    }
}
