using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberLetterLibrary
{
    public class Unite
    {
        public static long Unit { get; set; }

        private static readonly string[] TableauUnite = new[] { "Zéro", "Un", "Deux", "Trois", "Quatre", "Cinq", "Six", "Sept", "Huit", "Neuf" };
        private static readonly string[] TableauUniteCntaine = new[] { "", "", "Deux ", "Trois ", "Quatre ", "Cinq ", "Six ", "Sept ", "Huit ", "Neuf " };
        private static readonly string[] TableauUniteDizaineCntaine = new[] { "", "Un", "Deux", "Trois", "Quatre", "Cinq", "Six", "Sept", "Huit", "Neuf" };

        public static string InLetter()
        {
            return TableauUnite[Unit];
        }

        public static string InLetter(string str)
        {
            long k = str.ToLong();
            return TableauUnite[k];
        }
        public static string InLetter(long k)
        {
            return TableauUnite[k];
        }
        public static string UniteCentaineInLetter(long k)
        {
            return TableauUniteCntaine[k];
        }
        public static string UniteDizaineCentaineInLetter(long k)
        {
            return TableauUniteDizaineCntaine[k];
        }
    }
}
