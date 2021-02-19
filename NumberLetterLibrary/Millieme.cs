using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberLetterLibrary
{
    public class Millieme:Centaine
    {
        public static string MilleInLetter(long k)
        {
            string mille, rslt;
            //modulo
            long i = k % 1000;

            long a = k - i;
            long cnt = a / 1000;


            
            if (cnt.IsUnite())
            {
                //unite
                mille = UniteCentaineInLetter(cnt);
            }
            else if (cnt.IsDizaine())
            {
                //dizaine
                mille = DizainesInLetter(cnt)+" ";
            }
            else
            {
                //centaine
                mille = CentaineInLetter(cnt)+" ";
            }

            if (i.IsUnite())
            {
                //Unite
                rslt = mille + "mille " + UniteDizaineCentaineInLetter(i);
            }
            else if (i.IsDizaine())
            {
                //dizaine
                rslt = mille + "mille " + DizainesInLetter(i);
            }
            else
            {
                // centaine
                rslt = mille + "mille " + CentaineInLetter(i);
            }
            
                
            
            return rslt.FirstToUpperCase();
        }
    }
}
