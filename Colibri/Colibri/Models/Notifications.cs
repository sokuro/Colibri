using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models
{
    public class Notifications
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; }
        public string UserName { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CategoryTypeId { get; set; }

        // 1 Product = 1 Category Type
        [ForeignKey("CategoryTypeId")]
        public virtual CategoryTypes CategoryTypes { get; set; }
    }
}
