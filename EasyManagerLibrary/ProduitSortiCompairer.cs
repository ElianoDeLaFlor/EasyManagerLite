using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class ProduitSortiCompairer : IEqualityComparer<ProduitSorti>
    {
        public bool Equals(ProduitSorti x, ProduitSorti y)
        {
            if (x == y)
                return true;
            if (x == null || y == null)
                return false;
            return x.ProduitId == y.ProduitId;
        }

        public int GetHashCode(ProduitSorti obj)
        {
            return (obj.ProduitId == 0 ? 0 : obj.ProduitId.GetHashCode());
        }
    }
}
