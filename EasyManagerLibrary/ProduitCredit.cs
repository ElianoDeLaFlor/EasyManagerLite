using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class ProduitCredit: ProduitSorti
    {
        public int CommandeId { get; set; }
        public double QuantiteRestante { get; set; }
        private VenteCredit _venteCredit;
        private Client _client;
        private Produit Produits;
        [Write(false)]
        public string ProductName => Produits.Nom;

        [Write(false)]
        public Produit Product { get => Produits; set => Produits = value; }

        [Write(false)]
        public decimal SommeRestante => GetSommeRestante();

        [Write(false)]
        public decimal SommeTotale => _venteCredit.Montant;

        [Write(false)]
        public decimal ResteTotale => _venteCredit.MontantRestant;

        [Write(false)]
        public string NomClient => $"{_client.Prenom} {_client.Nom}";

        public void SetProduit(Produit prod)
        {
            Produits = prod;
        }

        public Produit GetProduit()
        {
            return Produits;
        }

        public void SetVenteCredit(VenteCredit venteCredit) { _venteCredit = venteCredit; }
        public VenteCredit GetVenteCredit() { return _venteCredit; }

        public void SetClient(Client client) { _client = client; }
        public Client GetClient() { return _client; }

        private decimal GetSommeRestante()
        {
            decimal rslt;
            if (Discount > 0)
            {
                //Il y a une réduction
                rslt=(decimal)QuantiteRestante * (PrixUnitaire*(1-Discount));
            }
            else
            {
                //Il n'y  pas de réduction
                rslt=(decimal)QuantiteRestante * PrixUnitaire;
            }
            return rslt;
        }
    }
}
