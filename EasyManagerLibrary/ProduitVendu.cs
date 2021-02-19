using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class ProduitVendu: ProduitSorti
    {
        public int VenteId { get; set; }
        private Vente _vente;
        private Client _client;
        [Write(false)]
        public Produit Product { get=>Produits; set=>Produits=value; }
        private Produit Produits;
        public void SetProduit(Produit prod)
        {
            Produits= prod;
        }   
        
        public Produit GetProduit()
        {
            return Produits;
        }

        public void SetClient(Client client) { _client = client; }
        public Client GetClient() { return _client; }

        public void SetVente(Vente vente) { _vente = vente; }
        public Vente GetVente() { return _vente; }

        [Write(false)]
        public string ProduitNom => GetProduit().Nom;

        [Write(false)]
        public string NomClient => _client==null?"-":$"{_client.Prenom} {_client.Nom}";

        [Write(false)]
        public decimal SommeTotale => _vente.Montant;
    }
}
