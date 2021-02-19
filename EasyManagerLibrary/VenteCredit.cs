using CsvHelper.Configuration.Attributes;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;
using System;

namespace EasyManagerLibrary
{
    public class VenteCredit
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        [Name("Date de vente")]
        public DateTime Date { get; set; }
        [Ignore]
        public int UtilisateurId { get; set; }
        public decimal Montant { get; set; }
        [Name("Montant restant")]
        public decimal MontantRestant { get; set; }
        [Ignore]
        public decimal ValueDiscount { get; set; }
        [Name("Annulé")]
        [BooleanFalseValues("Non")]
        [BooleanTrueValues("Oui")]
        public bool Canceled { get; set; }
        [Name("Date d'annulation")]
        [NullValues("Null")]
        public DateTime? CanceledDate { get; set; }

        private Client Client;
        
        private Utilisateur Utilisateur;

        public void SetClient(Client client)
        {
            Client = client;
        }
        public Client GetClient()
        {
            return Client;
        }
        public void SetUser(Utilisateur user)
        {
            Utilisateur = user;
        }
        public Utilisateur GetUser()
        {
            return Utilisateur;
        }

        [Write(false)]
        [Name("Client")]
        public string NomClient => Client == null ? null : $"{Client.Prenom} {Client.Nom}";

        [Write(false)]
        [Name("Utilisateur")]
        public string UserName => Utilisateur == null ? null : $"{Utilisateur.Prenom} {Utilisateur.Nom}";

        [Write(false)]
        [Name("Soldé")]
        public string Solde => InfoChecker.Solder(MontantRestant == 0);

        public string GetSolde() { return InfoChecker.Solder(MontantRestant == 0); }

    }

}