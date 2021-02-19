using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberLetterLibrary
{
    public static class Letter
    {
        public static string Chiffre { get; set; }
        public static int ZeroCnt { get; set; }
        private static Tuple<bool,long,long> GetPart(string str)
        {
            bool b=false;
            long hole, dec;
            string Decstr;
            string[] tbl=new string[2];
            if (str.Contains(','))
            {
                b = true;
                hole = str.Split(',')[0].ToLong();
                Decstr = str.Split(',')[1];
                if (Decstr.Length != 0)
                {
                    GetZeroCount(Decstr);
                    dec = Decstr.ToLong();
                }
                else
                    dec = 0;
                
                if (dec == 0)
                    b = false;
                return new Tuple<bool, long, long>(b, hole, dec);
            }
            else
            {
                hole = str.ToLong();
                return new Tuple<bool, long, long>(b, hole, 0L);
            }
        }

        private static void GetZeroCount(string str)
        {
            int i = 0;
            while (str[i]=='0')
            {
                i++;
            }
            ZeroCnt = i;
        }

        private static string WriteZeroCount(int cnt)
        {
            string str = "";
            int k = 1;
            while (cnt >= k)
            {
                if (k == cnt)
                    str += "zéro";
                else
                    str+= "zéro ";
                k++;
            }
            return str;
        }

        private static string PartReader(long l)
        {
            string rslt;
            if (l.IsUnite())
            {
                rslt = Unite.InLetter(l);
            }
            else if(l.IsDizaine())
            {
                rslt = Dizaine.DizainesInLetter(l);
            }
            else if (l.IsCentaine())
            {
                rslt = Centaine.CentaineInLetter(l);
            }
            else if (l.IsMillieme())
            {
                rslt = Millieme.MilleInLetter(l);
            }
            else if (l.IsMillionnieme())
            {
                rslt = Millionnieme.MillionniemeInLetter(l);
            }
            else //if (l.IsBillion())
            {
                rslt = Billion.BillionInLetter(l);
            }
            return rslt;
        }

        public static string Result()
        {
            Tuple<bool, long, long> tuple = GetPart(Chiffre);
            string resultat;

            if (tuple.Item1)
            {
                //decimal
                resultat = $"{PartReader(tuple.Item2)} virgule {WriteZeroCount(ZeroCnt)} {PartReader(tuple.Item3).ToLower()}";
            }
            else
            {
                resultat = $"{PartReader(tuple.Item2)}";
            }
            return resultat;
        }
    }
}
