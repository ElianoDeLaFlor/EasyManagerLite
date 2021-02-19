using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public abstract class ProduitSorti
    {
        public int Id { get; set; }
        public int ProduitId { get; set; }
        public double Quantite { get; set; }
        public decimal PrixUnitaire { get; set; }
        public decimal Discount { get; set; }

        //public decimal Montant { get; set; }
       
        public decimal Montant { get; set; }

        public void GetTotal(decimal unitPrice=0)
        {
            if (unitPrice == 0)
                Montant = PrixUnitaire * (decimal)Quantite;
            else
                Montant = unitPrice * (decimal)Quantite;
        }
        [Write(false)]
        public string GetDiscount
        {
            get
            {
                if(Discount>=0 && Discount <= 1)
                {
                    return $"{Discount * 100} %";
                }
                else
                {
                    return Discount.ToString();
                }
            }
        }

    }
}
