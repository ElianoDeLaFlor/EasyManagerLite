using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class QuantiteEdition
    {
        public int Id { get; set; }
        public int ProduitId { get; set; }
        public DateTime DateEdition { get; set; }
        public int UtilisateurId { get; set; }
        public double Quantite { get; set; }
        public decimal PrixUnitaire { get; set; }

        private Produit _produit;
        private Utilisateur _utilisateur;

        public void SetProduit(Produit produit) { _produit = produit; }

        public Produit GetProduit() { return _produit; }

        public void SetUser(Utilisateur user)
        {
            _utilisateur = user;
        }
        public Utilisateur GetUser()
        {
            return _utilisateur;
        }

        [Write(false)]
        public string UserName => _utilisateur == null ? null : $"{_utilisateur.Prenom} {_utilisateur.Nom}";

        [Write(false)]
        public string UnitPrice => InfoChecker.CurrencyFormat(PrixUnitaire);
        [Write(false)]
        public string Date => InfoChecker.AjustDateWithTimeDMYT(DateEdition);
        [Write(false)]
        public string ProduitNom => _produit == null ? "-" : _produit.Nom;
    }
}
