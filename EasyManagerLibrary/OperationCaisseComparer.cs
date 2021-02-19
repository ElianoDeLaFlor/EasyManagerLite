using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class OperationCaisseComparer: IEqualityComparer<OperationCaisse>
    {
        public bool Equals(OperationCaisse x, OperationCaisse y)
        {
            if (x == y)
                return true;
            if (x == null || y == null)
                return false;
            return x.OperationId == y.OperationId;
        }

        public int GetHashCode(OperationCaisse obj)
        {
            return (obj.Id == 0 ? 0 : obj.Id.GetHashCode());
        }

        
    }
}
