using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class LicenceDataHolder
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public LicenceType TypeLicence { get; set; }
        public string Version { get; set; }
        public bool Status { get; set; }
        public int EasyId { get; set; }
    }
}
