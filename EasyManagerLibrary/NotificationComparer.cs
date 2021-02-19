using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    class NotificationComparer : IEqualityComparer<Notifications>
    {
        public bool Equals(Notifications x, Notifications y)
        {
            if (x == y)
                return true;
            if (x == null || y == null)
                return false;
            return x.ProduitNom == x.ProduitNom;
        }

        public int GetHashCode(Notifications obj)
        {
            if (obj == null)
                return 0;
            return (obj.Id == 0 ? 0 : obj.Id.GetHashCode());
        }
    }
}
