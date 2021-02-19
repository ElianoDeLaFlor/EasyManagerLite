using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public interface IAccessControl
    {
        Module GetModule { get; set; }

        bool CanAccess(Utilisateur utilisateur);
    }
}
