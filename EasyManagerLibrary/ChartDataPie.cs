using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class ChartDataPie
    {
        public string Titre { get; set; }
        public double Valeur { get; set; }
        public List<double> Valeurs { get; set; } = new List<double>();
        public bool LblDate { get; set; }
    }
}
