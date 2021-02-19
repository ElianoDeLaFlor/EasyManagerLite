using System;

namespace EasyManagerLibrary
{
    public class Utilisateur:Personne
    {
        public string Password { get; set; }
        public string Login { get; set; }
        public string RoleLibelle { get; set; }
        public DateTime PassDate { get; set; }
        public bool Deleted { get; set; }
    }
}
