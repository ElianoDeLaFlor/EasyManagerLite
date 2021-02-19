using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyManagerAPI.Models
{
    public class AppUserInfo
    {
        public Guid Id { get; set; }
        [StringLength(120)]
        public string Nom { get; set; }
        [StringLength(60)]
        public string Contact { get; set; }
        [StringLength(100)]
        public string Email { get; set; }

        public virtual ICollection<Backup> GetBackups { get; set; }
    }
}