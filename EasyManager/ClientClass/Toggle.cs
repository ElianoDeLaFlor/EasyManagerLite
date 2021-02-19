using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyManagerLibrary;

namespace EasyManager.ClientClass
{
    public class Toggle
    {
        public bool State { get; set; } = true;
        public Module Module { get; set; } = new Module() { Id = 1, Libelle = "test" };
    }
}
