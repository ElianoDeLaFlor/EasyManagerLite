using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyManagerAPI.Models
{
    public class AppVersionEqualityComparer : IComparer<AppVersion>, IEqualityComparer<AppVersion>
    {
        public int Compare(AppVersion x, AppVersion y)
        {
            if (x.Major < y.Major)
                return -1;
            if (x.Major > y.Major)
                return 1;
            if (x.Major == y.Major)
            {
                if (x.Minor < y.Minor)
                    return -1;
                if (x.Minor > y.Minor)
                    return 1;
                if (x.Minor == y.Minor)
                {
                    if (x.Build < y.Build)
                        return -1;
                    if (x.Build > y.Build)
                        return 1;
                    if (x.Build == y.Build)
                    {
                        if (x.Revision < y.Revision)
                            return -1;
                        if (x.Revision > y.Revision)
                            return 1;
                    }
                }

            }
            return 0;
        }

        public bool Equals(AppVersion x, AppVersion y)
        {
            if (x.Major == y.Major && x.Minor == y.Minor && x.Build == y.Build && x.Revision == y.Revision)
                return true;
            if (x == null || y == null)
                return false;
            return x.Id == y.Id;

        }

        public int GetHashCode(AppVersion obj)
        {
            throw new NotImplementedException();
        }
    }
}