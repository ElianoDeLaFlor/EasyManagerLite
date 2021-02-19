using CsvHelper.Configuration.Attributes;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class Vente
    {
        public int Id { get; set; }
        [Ignore]
        public int? CommandeId { get; set; }
        public decimal Montant { get; set; }
        public DateTime Date { get; set; }
        [Ignore]
        public int UtilisateurId { get; set; }
        [Ignore]
        public int? ClientId { get; set; }
        [Name("Taux de réduction")]
        public decimal ValueDiscount { get; set; }
        [Name("Annulé")]
        [BooleanFalseValues("Non")]
        [BooleanTrueValues("Oui")]
        public bool Canceled { get; set; }
        [Name("Date d'annulation")]
        [NullValues("Null")]
        public DateTime? CanceledDate { get; set; }

        public Vente()
        {
            //check table column

        }

        private Utilisateur Utilisateur;

        private string CmdClient;

        public void SetUser(Utilisateur user)
        {
            Utilisateur = user;
        }
        public Utilisateur GetUser()
        {
            return Utilisateur;
        }

        [Write(false)]
        [Name("Utilisateur")]
        public string UserName => Utilisateur == null ? null : $"{Utilisateur.Prenom} {Utilisateur.Nom}";

        [Write(false)]
        [Name("Nom Client / N° Commande")]
        public string CommandeClient => CmdClient;

        public void SetCmdClient(string CmdrefClient)
        {
            CmdClient = CmdrefClient;
        }
    }
}
