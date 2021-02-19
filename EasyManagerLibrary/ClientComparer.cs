using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class ClientComparer : IEqualityComparer<Client>
    {
        public bool Equals(Client x, Client y)
        {
            if (x == y)
                return true;
            if (x == null || y == null)
                return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(Client obj)
        {
            if (obj == null)
                return 0;
            return (obj.Id == 0 ? 0 : obj.Id.GetHashCode());
        }
    }
}
