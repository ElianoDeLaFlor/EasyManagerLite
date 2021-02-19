using System.Collections.Generic;

namespace EasyManagerLibrary
{
    public class RoleModuleComparer:IEqualityComparer<RoleModule>
    {
        bool IEqualityComparer<RoleModule>.Equals(RoleModule x, RoleModule y)
        {
            if (x == y)
                return true;
            if (x == null || y == null)
                return false;
            return x.Id == y.Id && x.RoleId == y.RoleId && x.ModuleId == y.ModuleId;

        }

        int IEqualityComparer<RoleModule>.GetHashCode(RoleModule obj)
        {
            return (obj.Id == 0 ? 0 : obj.Id.GetHashCode()) ^ (obj.RoleId == 0 ? 0 : obj.RoleId.GetHashCode()) ^
                   (obj.ModuleId == 0 ? 0 : obj.ModuleId.GetHashCode());
        }

    }
}