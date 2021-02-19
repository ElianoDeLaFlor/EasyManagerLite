using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class RapportProduit
    {
        [Name("Quantité vendue")]
        [Index(1)]
        public string QuantiteVendue { get; set; }
        [Index(2)]
        [Name("Montant des ventes payées")]
        public string Montant { get; set; }
        [Name("Quantité des crédits payés")]
        [Index(3)]
        public string QuantiteCreditPayer { get; set; }
        [Name("Montant des Credits Payés")]
        [Index(4)]
        public string MontantCreditPayer { get; set; }
        [Name("Quanité des credits")]
        [Index(5)]
        public string QuaniteCredit { get; set; }
        [Name("Montant des credits")]
        [Index(6)]
        public string MontantCredit { get; set; }
        [Name("Produit")]
        [Index(0)]
        public string Name { get; set; }
    }
}
