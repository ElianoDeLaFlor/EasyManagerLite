using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class ProduitVenduResume:ProduitVendu,INotifyPropertyChanged
    {
        public ProduitVenduResume() { }
        public ProduitVenduResume(ProduitVendu produitVendu) 
        {
            Discount = produitVendu.Discount;
            Montant = produitVendu.Montant;
            PrixUnitaire = produitVendu.PrixUnitaire;
            ProduitId = produitVendu.ProduitId;
            Quantite = produitVendu.Quantite;
            VenteId = produitVendu.VenteId;
        }
        private decimal _total;
        public decimal MontantTotal 
        { 
            get=>_total;
            set
            {
                if (_total == value)
                    return;
                if (Discount > 0 && Discount<=1)
                {
                    //Apply percentage discount
                    _total = value * (1- Discount);
                }
                else if (Discount>1)
                {
                    //Apply value discount
                    _total = value - Discount;
                }
                else
                    _total = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
