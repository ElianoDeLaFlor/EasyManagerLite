using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyManagerAPI.Models
{
    public class AppVersionComparer : IEqualityComparer<AppVersion>
    {
        public bool Equals(AppVersion x, AppVersion y)
        {
            if (x.Major == y.Major && x.Minor==y.Minor && x.Build==y.Build && x.Revision==y.Revision)
                return true;
            if (x == null || y == null)
                return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(AppVersion obj)
        {
            return (obj.Id == 0 ? 0 : obj.Id.GetHashCode());
        }
    }
}