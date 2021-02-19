using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class LicenceRegInfoServer
    {
        public int Id { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Version { get; set; }
        public int EasyId { get; set; }
        public LicenceType Type { get; set; }
        public string AppKey { get; set; }
        public bool Status { get; set; }
    }
}
