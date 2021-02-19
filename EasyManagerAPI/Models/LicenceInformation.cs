using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyManagerAPI.Models
{
    public class LicenceInformation
    {
        public int Id { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        public DateTime PaymentDate { get; set; }
        [StringLength(90)]
        public string PaymentMethod { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool HasExpired { get; set; }
        [StringLength(500)]
        public string Code { get; set; }
        public LicenceType TypeLicence { get; set; }
        public Guid AppKey { get; set; }
    }
}