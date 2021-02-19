using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class LicenceInformation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool HasExpired { get; set; }
        public string Code { get; set; }
        public LicenceType TypeLicence { get; set; }
    }
}
