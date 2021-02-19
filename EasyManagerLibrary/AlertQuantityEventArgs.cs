using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class AlertQuantityEventArgs:EventArgs
    {
        public Produit Produit { get; private set; }

        public AlertQuantityEventArgs(Produit produit)
        {
            Produit = produit;
        }
    }
}
