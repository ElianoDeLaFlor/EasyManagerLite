using System;

using Dapper.Contrib.Extensions;

namespace EasyManagerLibrary
{
    public class Discount
    {
        public int Id { get; set; }
        public decimal Taux { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string ProduitNom { get; set; }
        public int? CategorieId { get; set; }
        public int? ClientId { get; set; }
        public bool Canceled { get; set; }
        public bool IsValidForCredit { get; set; }
        public int UserId { get; set; }

        [Write(false)]
        public string CancelStatus { get; set; }

        [Write(false)]
        public string ForCredit { get; set; }

        private Client _client;
        private Categorie _categorie;
        private Utilisateur _utilisateur;

        public void SetCat(Categorie cat) => _categorie = cat;

        [Write(false)]
        public string Tau => $"{Taux * 100}%";
        

        public void SetClient(Client client)=>_client = client;

        public void SetUtilisateur(Utilisateur utilisateur) => _utilisateur = utilisateur;


        [Write(false)]
        public string NomClient => _client == null ? "-" : $"{_client.Prenom} {_client.Nom}";
        [Write(false)]

        public string NomUtilisateur => _utilisateur == null ? "-" : $"{_utilisateur.Prenom} {_utilisateur.Nom}";
        [Write(false)]

        public string Produit => string.IsNullOrEmpty(ProduitNom) ? "-" : $"{ProduitNom}";

        [Write(false)]
        public string NomCat => _categorie == null ? "-" : $"{_categorie.Libelle}";

        [Write(false)]
        public string Cancel => CancelStatus;

        [Write(false)]
        public string Credit => ForCredit;




    }
}
