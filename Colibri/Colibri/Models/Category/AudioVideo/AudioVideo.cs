using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Models.Category.AudioVideo
{
    public class AudioVideo
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public AudioVideo(int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }
    }
}
