using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberLetterLibrary
{
    public class Dizaine:Unite
    {

        public static readonly string[] TableauDizaine = new[] { "Dix", "Onze", "Douze", "Treize", "Quatorze", "Quinze", "Seize", "Dix-" };
        public static readonly string[] TableauVingtaine = new[] { "Vingt", "Vingt-et-un", "Vingt-" };
        public static readonly string[] TableauTrentaine = new[] { "Trente", "Trente-et-un", "Trente-" };
        public static readonly string[] TableauQuarantaine = new[] { "Quarante", "Quarante-et-un", "Quarante-" };
        public static readonly string[] TableauCinquantaine = new[] { "Cinquante", "Cinquante-et-un", "Cinquante-" };
        public static readonly string[] TableauSoixantaine = new[] { "Soixante", "Soixante-et-un", "Soixante-" };
        public static readonly string[] TableauSoixanteDizaine = new[] { "Soixante-dix", "Soixante-" };
        public static readonly string[] TableauQuatreVingtaine = new[] { "Quatre-vingt", "Quatre-vingt-et-un", "Quatre-vingt-" };
        public static readonly string[] TableauQuatreVingtDizaine = new[] { "Quatre-vingt-dix", "Quatre-vingt-"};


        public static string DizainesInLetter(long k)
        {

            if (k >= 10 && k <= 19)
            {
                //dizaine
                return DizaineInLetter(k);
            }
            else if (k >= 20 && k <= 29)
            {
                // vingtaine
                return VingtaineInLetter(k);
            }
            else if (k >= 30 && k <= 39)
            {
                //trentaine
                return TrentaineInLetter(k);
            }
            else if (k >= 40 && k <= 49)
            {
                //quarantaine
                return QuarantaineInLetter(k);
            }
            else if (k >= 50 && k <= 59)
            {
                // cinquantaine
                return CinquantaineInLetter(k);
            }
            else if (k >= 60 && k <= 69)
            {
                //soixantaine
                return SoixantaineInLetter(k);
            }
            else if (k >= 70 && k <= 79)
            {
                //soixante-dizaine
                return SoixanteDizaineInLetter(k);
            }
            else if (k >= 80 && k <= 89)
            {
                //quatre-vingtaine
                return QuatreVingtaineInLetter(k);
            }
            else //if(k>=90 && k <= 99)
            {
                // quatre-vingt-dizaine
                return QuatreVingtDizaineInLetter(k);
            }

        }

        public static string DizaineInLetter(long k)
        {
            string rslt;
            long i = k % 10;
            switch (i)
            {
                case 7:
                    {
                        rslt = TableauDizaine[7] + (InLetter(i)).ToLower();
                        break;
                    }
                case 8:
                    {
                        rslt = TableauDizaine[7] + (InLetter(i)).ToLower();
                        break;
                    }
                case 9:
                    {
                        rslt = TableauDizaine[7] + (InLetter(i)).ToLower();
                        break;
                    }
                default:
                    {
                        rslt = TableauDizaine[i];
                        break;
                    }
            }
            return rslt;
        }
        public static string VingtaineInLetter(long k)
        {
            string rslt;
            long i = k % 20;
            switch (i)
            {
                case 0:
                    {
                        rslt = TableauVingtaine[i];
                        break;
                    }
                case 1:
                    {
                        rslt = TableauVingtaine[i];
                        break;
                    }
                default:
                    {
                        rslt = TableauVingtaine[2] + (InLetter(i)).ToLower();
                        break;
                    }
            }
            return rslt;
        }
        public static string TrentaineInLetter(long k)
        {
            string rslt;
            long i = k % 30;
            switch (i)
            {
                case 0:
                    {
                        rslt = TableauTrentaine[i];
                        break;
                    }
                case 1:
                    {
                        rslt = TableauTrentaine[i];
                        break;
                    }
                default:
                    {
                        rslt = TableauTrentaine[2] + (InLetter(i)).ToLower();
                        break;
                    }
            }
            return rslt;
        }
        public static string QuarantaineInLetter(long k)
        {
            string rslt;
            long i = k % 40;
            switch (i)
            {
                case 0:
                    {
                        rslt = TableauQuarantaine[i];
                        break;
                    }
                case 1:
                    {
                        rslt = TableauQuarantaine[i];
                        break;
                    }
                default:
                    {
                        rslt = TableauQuarantaine[2] + (InLetter(i)).ToLower();
                        break;
                    }
            }
            return rslt;
        }
        public static string CinquantaineInLetter(long k)
        {
            string rslt;
            long i = k % 50;
            switch (i)
            {
                case 0:
                    {
                        rslt = TableauCinquantaine[i];
                        break;
                    }
                case 1:
                    {
                        rslt = TableauCinquantaine[i];
                        break;
                    }
                default:
                    {
                        rslt = TableauCinquantaine[2] + (InLetter(i)).ToLower();
                        break;
                    }
            }
            return rslt;
        }
        public static string SoixantaineInLetter(long k)
        {
            string rslt;
            long i = k % 60;
            switch (i)
            {
                case 0:
                    {
                        rslt = TableauSoixantaine[i];
                        break;
                    }
                case 1:
                    {
                        rslt = TableauSoixantaine[i];
                        break;
                    }
                default:
                    {
                        rslt = TableauSoixantaine[2] + (InLetter(i)).ToLower();
                        break;
                    }
            }
            return rslt;
        }
        public static string SoixanteDizaineInLetter(long k)
        {
            string rslt;
            long i = k % 70;
            switch (i)
            {
                case 0:
                    {
                        rslt = TableauSoixanteDizaine[i];
                        break;
                    }
                default:
                    {
                        rslt = TableauSoixanteDizaine[1] + (DizaineInLetter(i)).ToLower();
                        break;
                    }
            }
            return rslt;
        }
        public static string QuatreVingtaineInLetter(long k)
        {
            string rslt;
            long i = k % 80;
            switch (i)
            {
                case 0:
                    {
                        rslt = TableauQuatreVingtaine[i];
                        break;
                    }
                case 1:
                    {
                        rslt = TableauQuatreVingtaine[i];
                        break;
                    }
                default:
                    {
                        rslt = TableauQuatreVingtaine[2] + (InLetter(i)).ToLower();
                        break;
                    }
            }
            return rslt;
        }
        public static string QuatreVingtDizaineInLetter(long k)
        {
            string rslt;
            long i = k % 90;
            switch (i)
            {
                case 0:
                    {
                        rslt = TableauQuatreVingtDizaine[i];
                        break;
                    }
                default:
                    {
                        rslt = TableauQuatreVingtDizaine[1] + (DizaineInLetter(i)).ToLower();
                        break;
                    }
            }
            return rslt;
        }

    }
}
