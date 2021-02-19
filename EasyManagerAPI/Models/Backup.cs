using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyManagerAPI.Models
{
    public class Backup
    {
        public Guid Id { get; set; }
        [StringLength(60)]
        public string BackupFileName { get; set; }
        public Guid AppUserInfoId { get; set; }
    }
}