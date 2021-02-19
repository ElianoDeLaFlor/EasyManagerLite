using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class Caisse
    {
        public int Id { get; set; }
        public int OperationCaisseId { get; set; }
        public decimal Montant { get; set; }
    }
}
