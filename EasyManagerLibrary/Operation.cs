using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class Operation
    {
        public int Id { get; set; }
        public string Libelle { get; set; }
        public TypeOperation TypeOperation { get; set; }

        public override string ToString()
        {
            if(TypeOperation==TypeOperation.Sortie)
            {
                return "Sortie";
            }
            else
            {
                return "Entrer";
            }
        }
    }

    public enum TypeOperation
    {
        Entree=1,
        Sortie=0
    }
}
