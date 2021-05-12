using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace EasyManagerLibrary
{
    public class InfoChecker
    {

        public int ConnectedUserId { get; set; }
        public static bool IsPathOne { get; set; }
        public static DateTime StringToDate(string str)
        {
            if (DateTime.TryParse(str, out DateTime dateTime))
                return dateTime;
            else
                return DateTime.Now;
        }

        public static bool IsNameOk(string nom)
        {
            /*if (LenghtChecher(nom))
            {
                return NameRegExp(nom);
            }
            return false;*/
            return LenghtChecher(nom) && NameRegExp(nom);
        }

        public static string CurrencyFormat(decimal dec)
        {
            return string.Format("{0:C2}", dec);
        }

        public static string CurrencyFormatter(decimal dec)
        {
            
            return dec.ToString("C", CultureInfo.CurrentCulture);
        }

        public static bool IsConnected()
        {
            try
            {
                using(var client=new System.Net.WebClient())
                    using (client.OpenRead("https://google.com/generate_204"))
                        return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsPriceOk(string price)
        {
            string pattern = @"[0-9]*";
            Match m = Regex.Match(price, pattern);
            if (m.Success)
                return m.Value.Length == price.Length;
            else
                return false;
        }

        public static bool IsPassOk(string nom)
        {
            if (PassLenghtChecher(nom))
            {
                return PassRegExp(nom);
            }
            return false;
        }

        private static bool LenghtChecher(string str)
        {
            return str.Length >= 3;
        }

        private static bool PassLenghtChecher(string str)
        {
            return str.Length >= 6;
        }

        private static bool NameRegExp(string str)
        {
            string pattern = @"[a-zA-Z]*";
            Match m = Regex.Match(str, pattern);
            if (m.Success)
                return m.Value.Length == str.Length;
            else
                return false;
        }

        public static bool MailRegExp(string str)
        {
            /*string pattern = @"\A([a-z0-9]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
                Match m = Regex.Match(str, pattern);
                return m.Success;*/
            //Todo Correct mail regex partern
            return true;
        }

        private static bool PassRegExp(string str)
        {
            string pattern = @"[a-z,A-Z,0-9]*";
            Match m = Regex.Match(str, pattern);
            if (m.Success)
            {
                if (m.Value.Length == str.Length)
                {
                    return ContainsNumber(m.Value);
                }
                else
                    return false;
            }
            else
                return false;
        }

        private static bool ContainsNumber(string str)
        {
            int cnt = 0;
            foreach (var item in str)
            {
                if (int.TryParse(item.ToString(), out int k))
                    cnt++;
            }

            return cnt > 0;
        }

        public static string NumericOnlyRegExp(string str)
        {
            string pattern = @"[0-9]*";
            Match m = Regex.Match(str, pattern);
            if (m.Success)
                return m.Value;
            else
                return "";
        }

        public static string ContactRegExp(string str)
        {
            string pattern = @"[+0-9 ]*";
            Match m = Regex.Match(str, pattern);
            if (m.Success)
                return m.Value;
            else
                return "";
        }

        public static string AlphabetOnlyRegExp(string str)
        {
            string pattern = @"[A-Za-zéè]*";
            Match m = Regex.Match(str, pattern);
            if (m.Success)
                return m.Value;
            else
                return "";
        }

        public static string NumericDecOnlyRegExp(string str)
        {
            string pattern = @"[0-9.]*";
            Match m = Regex.Match(str, pattern);
            if (m.Success)
                return m.Value;
            else
                return "";
        }

        public static decimal ConvertTODecimal(string str)
        {
            return Convert.ToDecimal(str);
        }

        public static double SetHeight(Window window)
        {
            double oldsize = window.ActualHeight;
            return oldsize - 30;
        }

        public static bool IsSame(string str, string str2)
        {
            return str.Equals(str2, StringComparison.CurrentCulture);
        }

        public static string Encrypt(string str)
        {
            byte[] data = Encoding.ASCII.GetBytes(str);
            data = new SHA384Managed().ComputeHash(data);
            //data = new System.Security.Cryptography.SHA384Managed().ComputeHash(data);
            return To64Base(data); //Encoding.ASCII.GetString(data);
        }

        public static bool IsPassFormatOk(string pass)
        {
            if (PassLenghtChecher(pass))
                return PassRegExp(pass);
            return false;

        }

        public static bool IsEmpty(string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Fais la mise en forme de <code>int k</code>
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static string FormatIdent(int k)
        {
            string num = k.ToString();
            int nbrdechiffre = 10;
            int nbrdezeroo = nbrdechiffre - num.Length;
            string zeros = "";
            for (int i = 0; i < nbrdezeroo; i++)
            {
                zeros += "0";
            }
            return zeros + num;
        }

        /// <summary>
        /// Gère le nom sous lequel le fichier sera enregisté
        /// </summary>
        /// <param name="filename">Nom d'enregistrement</param>
        /// <param name="folder"> Dossier d'enregistrement</param>
        /// <returns></returns>
        public static string SaveLoc(string filename, string folder)
        {
            string chemin;
            int cnt = 2;
            string s = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(s + "\\EasyManager\\" + folder))
                Directory.CreateDirectory(s + "\\EasyManager\\" + folder);

            chemin = s + "\\EasyManager\\" + folder + "\\" + filename + ".pdf";
            while (File.Exists(chemin))
            {
                chemin = s + "\\EasyManager\\" + folder + "\\" + filename + "_" + cnt + ".pdf";
                ++cnt;
            }
            return chemin;
        }

        public static string SaveDir(string folder)
        {
            string s = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(s + "\\EasyManager\\" + folder))
                Directory.CreateDirectory(s + "\\EasyManager\\" + folder);

            return s + "\\EasyManager\\" + folder;
        }

        public static bool SavePicture(string filepath,out string logoname)
        {
            try
            {
                //Icon Name
                string iconname = Guid.NewGuid().ToString().Replace("-", ""); ;

                //Is icon folder exist?
                if (!Directory.Exists(Path.GetFullPath("IconFolder")))
                    Directory.CreateDirectory(Path.GetFullPath("IconFolder"));

                
                FileInfo fileInfo = new FileInfo(filepath);
                string ext = fileInfo.Extension;
                iconname += ext;
                // Todo delete the existing file

                File.Copy(filepath, Path.GetFullPath("IconFolder/" + iconname));
                logoname = iconname;
                return true;
            }
            catch
            {
                logoname = string.Empty;
                return false;
            }
            
        }

        public static bool SavePictureTwo(string filepath, out string logoname)
        {
            try
            {
                //Icon Name
                string iconname = Guid.NewGuid().ToString().Replace("-", ""); ;

                //Is icon folder exist?
                if (!Directory.Exists(Path.GetFullPath("IconFolder")))
                    Directory.CreateDirectory(Path.GetFullPath("IconFolder"));


                FileInfo fileInfo = new FileInfo(filepath);
                string ext = fileInfo.Extension;
                iconname += ext;
                // Todo delete the existing file

                File.Copy(filepath, Path.GetFullPath("IconFolder/" + iconname));
                logoname = iconname;
                return true;
            }
            catch
            {
                logoname = string.Empty;
                return false;
            }

        }

        public static bool SavePictureThree(string filepath, out string logoname)
        {
            try
            {
                //Icon Name
                string iconname = Guid.NewGuid().ToString().Replace("-", ""); ;

                //Is icon folder exist?
                if (!Directory.Exists(Path.GetFullPath("IconFolder")))
                    Directory.CreateDirectory(Path.GetFullPath("IconFolder"));


                FileInfo fileInfo = new FileInfo(filepath);
                string ext = fileInfo.Extension;
                iconname += ext;
                // Todo delete the existing file

                File.Copy(filepath, Path.GetFullPath("IconFolder/" + iconname));
                logoname = iconname;
                return true;
            }
            catch
            {
                logoname = string.Empty;
                return false;
            }

        }

        private static bool DeleteImages(string path,string pathtwo,string paththree)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);

                if (File.Exists(pathtwo))
                    File.Delete(pathtwo);

                if (File.Exists(paththree))
                    File.Delete(paththree);

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public static string SetShopLogoPath(string name)
        {
            return (Path.GetFullPath("IconFolder/"+name));
        }

        public static string ShopLogoDefault()
        {
            return (Path.GetFullPath("Logo/LogoB.png"));
        }

        public static LicenceType StringToLicenceType(string str)
        {
            if (str.Equals("Infinity", StringComparison.CurrentCultureIgnoreCase))
                return LicenceType.Ininity;
            return LicenceType.Periodic;
        }

        public static string LicenceTypeToString(LicenceType licenceType)
        {
            if (licenceType == LicenceType.Ininity)
                return "Ininity";
            return "Periodic;";
        }
        public static LicenceType IntToLicenceType(int str)
        {
            if (str==1)
                return LicenceType.Ininity;
            return LicenceType.Periodic;
        }

        public static int LicenceTypeToint(LicenceType licenceType)
        {
            if (licenceType == LicenceType.Ininity)
                return 1;
            return 0;
        }

        /// <summary>
        /// Génère et enregistre la facture
        /// </summary>
        /// <param name="nomclient">Le nom sous lequel la facture sera enregistrée</param>
        /// <returns></returns>
        public static bool GenerateFacture(string nomclient)
        {
            Office office = new Office();

            return office.GenFacture(Path.GetFullPath("Files\\Facture_Prototype.dotx"), SaveLoc(nomclient, "facture"));
        }

        /// <summary>
        /// Vérifie si la date donnée a une durée d'au moins un mois comparée avec la date actuelle
        /// </summary>
        /// <param name="dt">La date à vérifier</param>
        /// <returns></returns>
        public static bool IsMonthOld(DateTime dt)
        {
            DateTime now = DateTime.Now;
            TimeSpan span = now - dt;

            return span.Days >= 35;
        }

        /// <summary>
        /// Retourne un <seealso cref="string"/> vrai=>true et un <seealso cref="string"/> faux=>false
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string Solder(bool b)
        {
            return b == true ? "Oui" : "Non";
        }

        /// <summary>
        /// Retranche de la date actuelle le nombre de jour donnée
        /// </summary>
        /// <param name="j">Nombre de jour donnée</param>
        /// <returns></returns>
        public static DateTime DateArriere(int j)
        {
            long date = DateTime.Now.Ticks;
            TimeSpan span0 = new TimeSpan(date);
            TimeSpan span1 = new TimeSpan(24 * j, 0, 0);
            var span = span0 - span1;
            var rslt = new DateTime(span.Ticks);
            return new DateTime(rslt.Year, rslt.Month, rslt.Day);
        }

        public static DateTime DateArriere(DateTime d1,int j)
        {
            long date = d1.Ticks;
            TimeSpan span0 = new TimeSpan(date);
            TimeSpan span1 = new TimeSpan(24 * j, 0, 0);
            var span = span0 - span1;
            var rslt = new DateTime(span.Ticks);
            return new DateTime(rslt.Year, rslt.Month, rslt.Day);
        }

        public static double DateDiff(DateTime d1, DateTime d2)
        {
            long date1 = d1.Ticks;
            long date2 = d2.Ticks;
            TimeSpan ts1 = new TimeSpan(date1);
            TimeSpan ts2 = new TimeSpan(date2);
            var span = ts2 - ts1;
            return span.TotalDays;
        }

        /// <summary>
        /// Increment the date by the giving days
        /// </summary>
        /// <param name="d1">Date to increment</param>
        /// <param name="j">Nomber of days to add</param>
        /// <returns></returns>
        public static DateTime NextDate(DateTime d1, int j)
        {
            //long date = DateTime.Now.Ticks;
            TimeSpan span0 = new TimeSpan(d1.Ticks);
            TimeSpan span1 = new TimeSpan(24 * j, 0, 0);
            var span = span0 + span1;
            var rslt = new DateTime(span.Ticks);
            return rslt;
        }

        public static string AjustDateWithTime(DateTime date)
        {
            string m = date.Month > 9 ? $"{date.Month}" : $"0{date.Month}";
            string d = date.Day > 9 ? $"{date.Day}" : $"0{date.Day}";
            return $"{date.Year}-{m}-{d} {date.TimeOfDay}";
        }

        /// <summary>
        /// Retourne la date au format DD-MM-YYYY hh:mm:ss
        /// </summary>
        /// <param name="date">La date à ajuster</param>
        /// <returns>string</returns>
        public static string AjustDateWithTimeDMYT(DateTime date)
        {
            /*string m = date.Month > 9 ? $"{date.Month}" : $"0{date.Month}";
            string d = date.Day > 9 ? $"{date.Day}" : $"0{date.Day}";
            return $"{d}-{m}-{date.Year} {date.Hour}:{date.Minute}:{date.Second}";*/
            return AjustDateWithDMY(date);
        }

        /// <summary>
        /// Retourne la date au format YYYY-MM-DD hh:mm:ss
        /// </summary>
        /// <param name="date">La date à ajuster</param>
        /// <returns>string</returns>
        public static string AjustDateWithTimeYMDT(DateTime date)
        {
            string m = date.Month > 9 ? $"{date.Month}" : $"0{date.Month}";
            string d = date.Day > 9 ? $"{date.Day}" : $"0{date.Day}";
            return $"{date.Year}-{m}-{d}";
        }
        public static string AjustToEnglishDate(DateTime date)
        {
            string m = date.Month > 9 ? $"{date.Month}" : $"0{date.Month}";
            string d = date.Day > 9 ? $"{date.Day}" : $"0{date.Day}";
            string h = date.Hour > 12 ? $"{date.Hour % 12}:{date.Minute}:{date.Second} PM" : $"{date.Hour}:{date.Minute}:{date.Second} AM";
            if (date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                return $"{m}-{d}-{date.Year}";
            return $"{m}-{d}-{date.Year} {h}";
        }

        public static string AjustDateDb(DateTime date)
        {
            string m = date.Month > 9 ? $"{date.Month}" : $"0{date.Month}";
            string d = date.Day > 9 ? $"{date.Day}" : $"0{date.Day}";
            if (date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                return $"{date.Year}-{m}-{d}";
            return $"{date.Year}-{m}-{d}  {date.Hour}:{date.Minute}:{date.Second}";
        }

        /// <summary>
        /// Ajuste la date suivant la langue
        /// </summary>
        /// <param name="date">La date à ajuster</param>
        /// <returns>string</returns>
        public static string AjustDateWithDMY(DateTime date)
        {
            if (GetAppLang() == "fr")
            {
                // français
                string m = date.Month > 9 ? $"{date.Month}" : $"0{date.Month}";
                string d = date.Day > 9 ? $"{date.Day}" : $"0{date.Day}";

                string h = date.Hour > 9 ? $"{date.Hour}" : $"0{date.Hour}";
                string mm = date.Minute > 9 ? $"{date.Minute}" : $"0{date.Minute}";
                string s = date.Second > 9 ? $"{date.Second}" : $"0{date.Second}";

                if (date.Hour==0 && date.Minute==0 && date.Second==0)
                    return $"{d}-{m}-{date.Year}";
                return $"{d}-{m}-{date.Year}  {h}:{mm}:{s}";
            }
            else
            {
                // anglais
                return AjustToEnglishDate(date);
            }
            
        }

        public static bool IsWithinDate(DateTime debut, DateTime fin, DateTime datetocheck)
        {
            return debut <= datetocheck && datetocheck <= fin;
        }

        public static bool HasExpire(DateTime fin, DateTime dateTime)
        {
            return fin < dateTime;
        }

        /// <summary>
        /// Retourne une liste de date <see cref="DateArriere(int)"/>
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        public static List<DateTime> LasteDates(int j)
        {
            List<DateTime> lst = new List<DateTime>();
            for (int k = 1; k <= j; k++)
            {
                DateTime date = DateArriere(k);
                lst.Add(date);
            }
            return lst;
        }

        /// <summary>
        /// Ajute la date en format YYYY-MM-DD
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string AjustDate(DateTime date)
        {
            string m = date.Month > 9 ? $"{date.Month}" : $"0{date.Month}";
            string d = date.Day > 9 ? $"{date.Day}" : $"0{date.Day}";
            return $"{date.Year}-{m}-{d}";
        }

        /// <summary>
        /// Ajute la date en format DD-MM-YYYY
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string DMY_AjustDate(DateTime date)
        {
            string m = date.Month > 9 ? $"{date.Month}" : $"0{date.Month}";
            string d = date.Day > 9 ? $"{date.Day}" : $"0{date.Day}";
            return $"{d}-{m}-{date.Year}";
        }

        /// <summary>
        /// Restart the application
        /// </summary>
        public static void RestartApp()
        {
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
            System.Windows.Forms.Application.Exit();
        }
        public static void ShutdownApp()
        {
            Application.Current.Shutdown();
            System.Windows.Forms.Application.Exit();
        }

        /// <summary>
        /// make a backup copy of the database
        /// </summary>
        /// <param name="info"></param>
        /// <returns>0=>errors<br/> 1=>success<br/> 2=>backup dir not exist</returns>
        public static int Backup(BackupInfo info)
        {
            string dbpath = Path.GetFullPath("EasyManagerDb.db");
            string path = Path.Combine(info.Dir, "EasyManagerDb.EasyManager");
            try
            {
                if (!Directory.Exists(info.Dir)) return 2;
                //check if the backup directory still exist
                //check if the file already exist
                if (File.Exists(path))
                    //delete the file if already exist
                    File.Delete(path);
                File.Copy(dbpath, path);               
                return 1;
            }
            catch
            {   
                return 0;
            }
            
        }

        public static bool Backup()
        {
            string tempbackup= Path.GetTempPath()+"EasyManager";
            string dbpath = Path.GetFullPath("EasyManagerDb.db");
            string path = Path.Combine(tempbackup, "EasyManagerDb.EasyManager");
            try
            {
                if (!Directory.Exists(tempbackup))
                {
                    //create the directory
                    Directory.CreateDirectory(tempbackup);
                }
                //check if the backup directory still exist
                //check if the file already exist
                if (File.Exists(path))
                    //delete the file if already exist
                    File.Delete(path);
                File.Copy(dbpath, path);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public static bool Restore(BackupInfo info)
        {
            string dbpath = Path.GetFullPath("EasyManagerDb.db");
            string path = Path.Combine(info.Dir, "EasyManagerDb.EasyManager");
            try
            {
                
                //check if the backup directory still exist
                if (!Directory.Exists(info.Dir)) return false;
                //check if the file already exist
                if (File.Exists(path))
                {
                    //delete the used database
                    if (File.Exists(dbpath))
                    {
                        File.Delete(dbpath);
                    }
                    File.Copy(path, dbpath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }

        }

        public static byte[] EncryptData(byte[] Data, RSAParameters RSAKey, bool DoOAEPPadding = true)
        {
            try
            {
                byte[] EncryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKey);
                    EncryptedData = RSA.Encrypt(Data, DoOAEPPadding);
                }
                return EncryptedData;
            }
            catch (CryptographicException exp)
            {
                Console.WriteLine(exp);
                return null;
            }
        }

        public static byte[] DecryptData(byte[] CypherData, RSAParameters RSAKey, bool DoOAEPPadding = true)
        {
            try
            {
                byte[] DecryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKey);
                    DecryptedData = RSA.Decrypt(CypherData, DoOAEPPadding);
                }
                return DecryptedData;
            }
            catch (CryptographicException exp)
            {
                Console.WriteLine(exp);
                return null;
            }
        }

        public static string EncryptData(string toEncrypt, bool useHashing=true)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
            // Get the key from config file
            string key = "Il n'y a pas de vin sans";// "012345678901234567890123";
            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {

                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice
                hashmd5.Clear();
            }
            else
                keyArray = Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                //set the secret key for the tripleDES algorithm
                Key = keyArray,
                //mode of operation. there are other 4 modes.
                //We choose CBC(Cypher Block Chaining)
                Mode = CipherMode.ECB,
                //padding mode(if any extra byte added)

                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string DecryptData(string cipherString, bool useHashing=true)
        {
            byte[] keyArray;
            //get the byte code of the string
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);
            //Get your key from config file to open the lock!
            string key = "Il n'y a pas de vin sans";// (string)settingsReader.GetValue("SecurityKey",typeof(String));
            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider
                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = Encoding.UTF8.GetBytes(key);
            }
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                //set the secret key for the tripleDES algorithm
                Key = keyArray,
                //mode of operation. there are other 4 modes. 
                //We choose ECB(Electronic code Book)
                Mode = CipherMode.ECB,
                //padding mode(if any extra byte added)
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// Process activation code informations
        /// </summary>
        /// <param name="code">Activation code</param>
        /// <returns> boolean</returns>
        public static LicenceInformation ManageActivationCodeInfo(string code)
        {
            string[] info = code.Split(';');
            DateTime PaymentDate = DateTime.Parse(GetData(info[0]));
            string PaymentMethod = GetData(info[1]);
            string name = GetData(info[2]);
            int Duration = GetData(info[3]).ToInt();
            var LicenceType = InfoChecker.StringToLicenceType(GetData(info[4]));
            DateTime StartDate = StartDateManager(PaymentDate, out int daystosubstract);
            DateTime endDate = NextDate(StartDate, Duration*30);
            DateTime ActivationEndDate = DateArriere(endDate, daystosubstract);

            LicenceInformation licence = new LicenceInformation();
            licence.Duration = Duration;
            licence.EndDate = ActivationEndDate;
            licence.StartDate = StartDate;
            licence.PaymentDate = PaymentDate;
            licence.PaymentMethod = PaymentMethod;
            licence.Name = name;
            licence.TypeLicence = LicenceType;
            if (PaymentDate > DateTime.UtcNow)
            {
                licence.HasExpired = true;
            }
            else
            {
                licence.HasExpired = daystosubstract > Duration * 30 || DateTime.UtcNow > ActivationEndDate;
            }
            

            return licence;

        }

        public static LicenceInformation ManageActivationCodeInfo(string code,out string appid)
        {
            string[] info = code.Split(';');
            DateTime PaymentDate = DateTime.Parse(GetData(info[0]));
            string PaymentMethod = GetData(info[1]);
            string name = GetData(info[2]);
            int Duration = GetData(info[3]).ToInt();
            var LicenceType = InfoChecker.StringToLicenceType(GetData(info[4]));
            appid = GetData(info[info.Length-1]);
            DateTime StartDate = StartDateManager(PaymentDate, out int daystosubstract);
            DateTime endDate = NextDate(StartDate, Duration * 30);
            DateTime ActivationEndDate = DateArriere(endDate, daystosubstract);

            LicenceInformation licence = new LicenceInformation();
            licence.Duration = Duration;
            licence.EndDate = ActivationEndDate;
            licence.StartDate = StartDate;
            licence.PaymentDate = PaymentDate;
            licence.PaymentMethod = PaymentMethod;
            licence.Name = name;
            licence.TypeLicence = LicenceType;
            if (PaymentDate > DateTime.UtcNow)
            {
                licence.HasExpired = true;
            }
            else
            {
                licence.HasExpired = daystosubstract > Duration * 30 || DateTime.UtcNow > ActivationEndDate;
            }


            return licence;

        }

        public static LicenceInformation GetTryLicence()
        {
            DateTime PaymentDate = DateTime.UtcNow;
            string PaymentMethod = "Try";
            string name = "Try";
            int Duration = 1;
            int daystosubstract;
            DateTime StartDate = StartDateManager(PaymentDate, out daystosubstract);
            DateTime endDate = NextDate(StartDate, Duration * 30);
            DateTime ActivationEndDate = DateArriere(endDate, daystosubstract);

            LicenceInformation licence = new LicenceInformation();
            licence.Duration = Duration;
            licence.EndDate = ActivationEndDate;
            licence.StartDate = StartDate;
            licence.PaymentDate = PaymentDate;
            licence.PaymentMethod = PaymentMethod;
            licence.Name = name;
            licence.TypeLicence = LicenceType.Periodic;
            licence.HasExpired = daystosubstract > Duration * 30 || DateTime.UtcNow > ActivationEndDate;

            return licence;
        }

        public static Tuple<double,double> ProgressValue(double max,double leftdays)
        {
            var r= (leftdays * 100) / max;
            return new Tuple<double, double>(r, leftdays);
        }

        public static string GetData(string data)
        {
            return data.Split('=')[1];
        }

        public static DateTime StartDateManager(DateTime paymentdate,out int k)
        {
            //current date
            DateTime today = DateTime.UtcNow;
            double overflowdays;
            //Limit activation date
            DateTime LimitDate = NextDate(paymentdate, 30);
            //Is Limit date execeted
            if (today <= LimitDate)
            {
                //Is withing
                k = 0;
                return today;
            }
            else
            {
                //execeted
                // number of day exceted
                overflowdays = DateDiff(LimitDate,today);
                double d = Math.Round(overflowdays);
                int days = int.Parse(d.ToString());
                k = days;
                return today;
            }
        }

        public static byte[] ToByteArray(string str)
        {
            UnicodeEncoding encoding = new UnicodeEncoding();
            return encoding.GetBytes(str);
        }
        public static string From64Base(string str)
        {
            var k = Convert.FromBase64String(str);
            UnicodeEncoding encoding = new UnicodeEncoding();
            return encoding.GetString(k);
        }

        public static string To64Base(byte[] byt)
        {
            var k = Convert.ToBase64String(byt);
            return k;
        }

        /// <summary>
        /// Updates key value
        /// </summary>
        /// <param name="key">key to update</param>
        /// <param name="value">update value</param>
        /// <returns></returns>
        public static bool SetKeyValue(string key, string value)
        {
            try
            {
                Configuration appconf = ConfigurationManager.OpenExeConfiguration(AppExe());
                appconf.AppSettings.Settings[key].Value = value;
                appconf.Save(ConfigurationSaveMode.Modified, true);
                ConfigurationManager.RefreshSection(key);
                return true;
            }
            catch
            {
                //MessageBox.Show(ex.Message);
                return false;
                
            }
        }

        /// <summary>
        /// return the app exec name
        /// </summary>
        /// <returns></returns>
        private static string AppExe()
        {
            return AppDomain.CurrentDomain.FriendlyName;
        }

        /// <summary>
        /// Gets the value of the giving key
        /// </summary>
        /// <param name="key">key name</param>
        /// <returns></returns>
        public static string KeyValue(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch
            {
                return null;
            }
        }

        public static string GetAppLang()
        {
            return KeyValue("Lang");
        }

        public static Utilisateur ConnectedUser { get; set; }

        public static List<BackupDeviceInfo> BackupDevices()
        {
            var devices=new List<BackupDeviceInfo>();
            using (var search=new ManagementObjectSearcher(@"SELECT * FROM Win32_USBHub"))
            {
                var collection = search.Get();
                foreach (var item in collection)
                {
                    var device = new BackupDeviceInfo
                    {
                        DeviceId = (string) item.GetPropertyValue("DeviceID"),
                        PnpDeiceId = (string) item.GetPropertyValue("PNPDeviceID"),
                        Description = (string) item.GetPropertyValue("Description")
                    };

                    devices.Add(device);
                }
                collection.Dispose();
            }

            return devices;
        }
        
    }

    public static class Converter
    {
        public static bool IsNumeric(string str)
        {
            return decimal.TryParse(str, out _);
        }
        public static int ToInt(this string str)
        {
            if (IsNumeric(str))
                return int.Parse(str);
            else
                return 0;
        }
    }
}
