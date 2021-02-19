using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberLetterLibrary
{
    public class Millionnieme:Millieme
    {
        public static bool IsBillion { get; set; } = false;
        public static string MillionniemeInLetter(long k)
        {
            string million, rslt;
            //modulo
            long i = k % 1000000;

            long a = k - i;
            long cnt = a / 1000000;

            if (cnt.IsUnite())
            {
                //unite
                million = InLetter(cnt) + " ";
            }
            else if (cnt.IsDizaine())
            {
                //dizaine
                million = DizainesInLetter(cnt) + " ";
            }
            else
            {
                //centaine
                million = CentaineInLetter(cnt) + " ";
            }

            if (i.IsUnite())
            {
                //Unite
                rslt = million + "million " + UniteCentaineInLetter(i);

            }
            else if (i.IsDizaine())
            {
                //dizaine
                rslt = million + "million " + DizainesInLetter(i);
            }
            else if (i.IsCentaine())
            {
                // centaine
                rslt = million + "million " + CentaineInLetter(i);
            }
            else
            {
                //million
                if (IsBillion)
                {
                    if (million=="Zéro ")
                        rslt = MilleInLetter(i);
                    else
                        rslt = million + "million " + MilleInLetter(i);
                }
                else
                {
                    rslt = million + "million " + MilleInLetter(i);
                }
            }

            return rslt.FirstToUpperCase();
        }
    }
}
