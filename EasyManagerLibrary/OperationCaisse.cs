using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class OperationCaisse
    {

        private Operation _operation;
        private Utilisateur utilisateur;
        public int Id { get; set; }
        public int UtilisateurId { get; set; }
        public DateTime Date { get; set; }
        public int OperationId { get; set; }
        public decimal Montant { get; set; }

        public void SetOperation(Operation prod)
        {
            _operation = prod;
        }

        public void SetUser(Utilisateur prod)
        {
            utilisateur = prod;
        }

        public Operation GetOperation()
        {
            return _operation;
        }

        public Utilisateur GetUser()
        {
            return utilisateur;
        }

        [Write(false)]
        public string NomUtilisateur => utilisateur == null ? "-" : $"{utilisateur.Prenom} {utilisateur.Nom}";
    }
}
