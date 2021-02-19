using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberLetterLibrary
{
    public class Billion:Millionnieme
    {
        public static string BillionInLetter(long k)
        {
            string billion, rslt;
            //modulo
            long i = k % 1000000000;
            long a = k - i;
            long cnt = a / 1000000000;

            if (cnt.IsUnite())
            {
                //unite
                billion = InLetter(cnt) + " ";
            }
            else if (cnt.IsDizaine())
            {
                //dizaine
                billion = DizainesInLetter(cnt) + " ";
            }
            else if (cnt.IsCentaine())
            {
                //centaine
                billion = CentaineInLetter(cnt) + " ";
            }
            else
            {
                //millieme
                billion = MilleInLetter(cnt) + " ";
            }



            if (i.IsUnite())
            {
                //Unite
                rslt = billion + "milliards " + UniteDizaineCentaineInLetter(i);
            }
            else if (i.IsDizaine())
            {
                //dizaine
                rslt = billion + "milliards " + DizainesInLetter(i);
            }
            else if (i.IsCentaine())
            {
                // centaine
                rslt = billion + "milliards " + CentaineInLetter(i);
            }
            else if(i.IsMillieme())
            {
                //mille
                rslt = billion + "milliards " + MilleInLetter(i);
            }
            else
            {
                //million
                Millionnieme.IsBillion = true;
                rslt = billion + "milliards " + MillionniemeInLetter(i);
            }

            return rslt.FirstToUpperCase();
        }
    }
}