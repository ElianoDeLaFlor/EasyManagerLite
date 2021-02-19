using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class DeleteRefComparer : IEqualityComparer<DeleteRef>
    {
        public bool Equals(DeleteRef x, DeleteRef y)
        {
            if (x == y)
                return true;
            if (x == null || y == null)
                return false;
            return x.RowIndex == y.RowIndex;
        }

        public int GetHashCode(DeleteRef obj)
        {
            if (obj == null)
                return 0;
            return (obj.RowIndex == 0 ? 0 : obj.RowIndex.GetHashCode());
        }
    }
}
