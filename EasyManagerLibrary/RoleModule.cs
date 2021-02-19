using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class RoleModule
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int ModuleId { get; set; }

        public RoleModule() {}

        public RoleModule(int roleid,int moduleid)
        {
            Id = 0;
            RoleId = roleid;
            ModuleId = moduleid;
        }
    }
}
