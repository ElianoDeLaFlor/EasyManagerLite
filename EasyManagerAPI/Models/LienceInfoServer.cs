using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyManagerAPI.Models
{
    public class LienceRegInfoServer
    {
        public int Id { get; set; }
        [StringLength(150)]
        public string Start { get; set; }
        [StringLength(150)]
        public string End { get; set; }
        [StringLength(30)]
        public string Version { get; set; }
        [StringLength(30)]
        public string EasyId { get; set; }
        [StringLength(30)]
        public string Type { get; set; }
        public Guid AppKey { get; set; }
        public bool Status { get; set; }
    }
}