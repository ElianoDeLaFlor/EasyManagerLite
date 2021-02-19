using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberLetterLibrary
{
    public class Centaine:Dizaine
    {
        public static string CentaineInLetter(long k)
        {
            string cent, rslt;
            //modulo
            long i = k % 100;

            long a = k - i;
            long cnt = a / 100;



            cent = UniteCentaineInLetter(cnt);
            if (i.IsUnite())
            {
                //unite
                rslt = cent + "cent " + UniteDizaineCentaineInLetter(i);
            }
            else //if(i.IsDizaine())
            {
                // dizaine
                rslt = cent + "cent " + (DizainesInLetter(i)).ToLower();
            }
            
            return rslt.FirstToUpperCase();
        }

    }
}
