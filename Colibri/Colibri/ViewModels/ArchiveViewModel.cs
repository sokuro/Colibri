using Colibri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colibri.ViewModels
{
    public class ArchiveViewModel
    {
        public IEnumerable<ArchiveEntry> ArchiveEntryList { get; set; }

        public ArchiveEntry ArchiveEntry { get; set; }

        public IEnumerable<CategoryGroups> CategoryGroups { get; set; }
        public IEnumerable<CategoryTypes> CategoryTypes { get; set; }

    }
}
