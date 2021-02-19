using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class CSVProduit
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public double QuantiteTotale { get; set; }
        public decimal PrixUnitaire { get; set; }
        public double QuantiteAlerte { get; set; }
        public string Supplier { get; set; }
        public decimal PrixGrossiste { get; set; }
        public double QuantiteRestante { get; set; }
    }
}
