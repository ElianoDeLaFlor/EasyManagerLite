using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class Role
    {
        public int Id { get; set; }
        public string Libelle { get; set; }

        public Role() {}

        public Role(int id,string libelle)
        {
            Id = id;
            Libelle = libelle;
        }
    }
}
