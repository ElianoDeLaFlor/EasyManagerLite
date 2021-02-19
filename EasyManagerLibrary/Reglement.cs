using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class Reglement
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int VenteCreditId { get; set; }
        public int UtilisateurId { get; set; }
        public decimal Montant { get; set; }
    }
}
