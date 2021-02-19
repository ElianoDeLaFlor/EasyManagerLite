using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyManagerAPI.Models
{
    public class AppVersion
    {
        public int Id { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public int Revision { get; set; }
        public  DateTime CreationDate { get; set; }
    }
}