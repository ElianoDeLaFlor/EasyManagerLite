using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using EasyManagerLibrary;
using Microsoft.Win32;

namespace Licence
{
    public static class LicenceInfo
    {
        public static string SubKeyName { get; set; } = @"SOFTWARE\EasyManager";
        /// <summary>
        /// Indicate if is the try version
        /// </summary>
        public static bool IsTryVersion { get; set; } = true;
        /// <summary>
        /// Indicate if is the payed version
        /// </summary>
        public static bool IsPayedVersion { get; set; } = false;
        public static List<LicenceRegInfoServer> LicenceRegInfo { get; set; }

        /// <summary>
        /// Grabbe the start date from the local machine
        /// </summary>
        /// <returns>DateTime</returns>
        public static DateTime TryStartDate()
        {
            return DateTime.UtcNow;
        }

        /// <summary>
        /// Get the end date
        /// </summary>
        /// <param name="startdate"></param>
        /// <returns>DateTime</returns>
        public static DateTime TryEndDate(DateTime startdate)
        {
            return DateAvant(startdate, 30);
        }

        /// <summary>
        /// Ajoute à la date actuelle le nombre de jour donnée
        /// </summary>
        /// <param name="j">Nombre de jour donnée</param>
        /// <returns></returns>
        public static DateTime DateAvant(int j)
        {
            long date = DateTime.Now.Ticks;
            TimeSpan span0 = new TimeSpan(date);
            TimeSpan span1 = new TimeSpan(24 * j, 0, 0);
            var span = span0 + span1;
            var rslt = new DateTime(span.Ticks);
            return new DateTime(rslt.Year, rslt.Month, rslt.Day);
        }

        /// <summary>
        /// Ajoute à la date donée le nombre de jour donnée
        /// </summary>
        /// <param name="j">Nombre de jour donnée</param>
        /// <returns></returns>
        public static DateTime DateAvant(DateTime dt, int j)
        {
            long date = dt.Ticks;
            TimeSpan span0 = new TimeSpan(date);
            TimeSpan span1 = new TimeSpan(24 * j, 0, 0);
            var span = span0 + span1;
            var rslt = new DateTime(span.Ticks);
            return new DateTime(rslt.Year, rslt.Month, rslt.Day);
        }

        public static bool IsLocalDateOk(DateTime dateTime)
        {
            return DateTime.Now >= dateTime;
        }

        //write the starting and the ending date of the try to the registry

        /// <summary>
        /// Write to the resgistry
        /// </summary>
        /// <param name="name">key</param>
        /// <param name="valu"></param>
        /// <returns></returns>
        public static bool ToRegistry(string name, string valu)
        {
            try
            {
                string user = Environment.UserDomainName + "\\" + Environment.UserName;

                RegistrySecurity rs = new RegistrySecurity();

                // Allow the current user to read and delete the key.
                //
                rs.AddAccessRule(new RegistryAccessRule(user,
                    RegistryRights.ReadKey | RegistryRights.Delete,
                    InheritanceFlags.None,
                    PropagationFlags.None,
                    AccessControlType.Allow));
                // Prevent the current user from writing or changing the
                // permission set of the key. Note that if Delete permission
                // were not allowed in the previous access rule, denying
                // WriteKey permission would prevent the user from deleting the
                // key.
                rs.AddAccessRule(new RegistryAccessRule(user,
                    RegistryRights.WriteKey | RegistryRights.ChangePermissions,
                    InheritanceFlags.None,
                    PropagationFlags.None,
                    AccessControlType.Allow));

                // Create the example key with registry security.
                RegistryKey rk = null;

                rk = Registry.LocalMachine.CreateSubKey(SubKeyName,
                    RegistryKeyPermissionCheck.Default, rs);

                rk.SetValue(name, valu);
                rk.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /*
                RegistrySecurity rs = new RegistrySecurity(); // it is right string for this code
                string currentUserStr = Environment.UserDomainName + "\\" + Environment.UserName;
                rs.AddAccessRule(new RegistryAccessRule(currentUserStr, RegistryRights.WriteKey | RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.FullControl, AccessControlType.Allow));

 
                RegistryKey Key = Registry.LocalMachine.OpenSubKey("Software",RegistryKeyPermissionCheck.);
                Key.CreateSubKey(SubKeyName);
                Key = Key.OpenSubKey(SubKeyName, true);

                Key.CreateSubKey(ActivationSubKeyName);
                Key = Key.OpenSubKey(ActivationSubKeyName,true);

                Key.SetValue(name, valu);
                return true;
    */

        /// <summary>
        /// Read from the registry
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string FromRegistry(string name)
        {
            //opening the subkey
            RegistryKey key = Registry.LocalMachine.OpenSubKey(SubKeyName);

            //check if the key exist and retrieve the stored value
            if (key != null)
            {
                var rslt = key.GetValue(name).ToString();
                key.Close();
                return rslt;
            }
            return null;
        }

        public static bool IsSubKeyExist(string subkeyname)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(SubKeyName);
            return key != null;
        }

        /// <summary>
        /// Activate the app in try mode by saving the try information to the registy
        /// </summary>
        /// <returns>Returns true if the try info is correctely saved to the registry</returns>
        public static bool ActivateTry(DateTime date)
        {
            if (IsLocalDateOk(date))
            {
                var Start = ToRegistry("Start", Convert.ToBase64String(InfoChecker.ToByteArray(TryStartDate().ToShortDateString())));
                var End = ToRegistry("End", Convert.ToBase64String(InfoChecker.ToByteArray(TryEndDate(TryStartDate()).ToShortDateString())));
                var Version = ToRegistry("Version", "Try");
                var Trystatus = ToRegistry("Status", IsTryVersion.ToString());
                var LType = ToRegistry("Type", "Periodic");
                var CurrentDate = SetCurrentDate();

                return Start && End && Version && Trystatus && CurrentDate && LType;
            }
            return false;
        }

        public static bool ActivateTry(LicenceInformation licence, int id)
        {

            var Start = ToRegistry("Start", Convert.ToBase64String(InfoChecker.ToByteArray(licence.StartDate.ToShortDateString())));
            var End = ToRegistry("End", Convert.ToBase64String(InfoChecker.ToByteArray(licence.EndDate.ToShortDateString())));
            var Version = ToRegistry("Version", "Try");
            var db_id = ToRegistry("EasyId", id.ToString());
            var Trystatus = ToRegistry("Status", IsPayedVersion.ToString());
            var LType = ToRegistry("Type", InfoChecker.LicenceTypeToint(licence.TypeLicence).ToString());
            var CurrentDate = SetCurrentDate();

            return Start && End && Version && Trystatus && CurrentDate && db_id && LType;
        }

        public static bool ActivateTry(LicenceDataHolder licence)
        {

            var Start = ToRegistry("Start", licence.StartDate);
            var End = ToRegistry("End", licence.EndDate);
            var Version = ToRegistry("Version", licence.Version);
            var db_id = ToRegistry("EasyId", licence.EasyId.ToString());
            var Trystatus = ToRegistry("Status", licence.Status.ToString());
            var LType = ToRegistry("Type", InfoChecker.LicenceTypeToint(licence.TypeLicence).ToString());
            var CurrentDate = SetCurrentDate();

            return Start && End && Version && Trystatus && CurrentDate && db_id && LType;
        }

        public static bool ActivateApp(DateTime datedebut, DateTime datefin,int id)
        {

            var Start = ToRegistry("Start", Convert.ToBase64String(InfoChecker.ToByteArray(datedebut.ToShortDateString())));
            var End = ToRegistry("End", Convert.ToBase64String(InfoChecker.ToByteArray(datefin.ToShortDateString())));
            var Version = ToRegistry("Version", "Paid");
            var db_id = ToRegistry("EasyId", id.ToString());
            var Trystatus = ToRegistry("Status", IsPayedVersion.ToString());
            var CurrentDate = SetCurrentDate();

            return Start && End && Version && Trystatus && CurrentDate && db_id;
        }

        public static bool ActivateApp(LicenceInformation licence, int id)
        {
            if (licence.TypeLicence!=LicenceType.Periodic && licence.TypeLicence!=LicenceType.Ininity)
                return false;
            var Start = ToRegistry("Start", Convert.ToBase64String(InfoChecker.ToByteArray(licence.StartDate.ToShortDateString())));
            var End = ToRegistry("End", Convert.ToBase64String(InfoChecker.ToByteArray(licence.EndDate.ToShortDateString())));
            var Version = ToRegistry("Version", "Paid");
            var db_id = ToRegistry("EasyId", id.ToString());
            var Trystatus = ToRegistry("Status", IsPayedVersion.ToString());
            var LType = ToRegistry("Type", InfoChecker.LicenceTypeToint(licence.TypeLicence).ToString());
            var CurrentDate = SetCurrentDate();

            return Start && End && Version && Trystatus && CurrentDate && db_id && LType;
        }

        public static bool ActivateApp(LicenceDataHolder licence)
        {
            var Start = ToRegistry("Start", licence.StartDate);
            var End = ToRegistry("End", licence.EndDate);
            var Version = ToRegistry("Version", licence.Version);
            var db_id = ToRegistry("EasyId", licence.EasyId.ToString());
            var Trystatus = ToRegistry("Status", IsPayedVersion.ToString());
            var LType = ToRegistry("Type", InfoChecker.LicenceTypeToint(licence.TypeLicence).ToString());
            var CurrentDate = SetCurrentDate();

            return Start && End && Version && Trystatus && CurrentDate && db_id && LType;
        }

        /// <summary>
        /// Sets the current date
        /// </summary>
        /// <returns></returns>
        public static bool SetCurrentDate()
        {
            return ToRegistry("CurrentDate", DateTime.UtcNow.ToShortDateString());
        }


        /// <summary>
        /// Check if the user have'nt try to modify the date of the machine
        /// </summary>
        /// <returns></returns>
        public static bool HasUserChangeDate()
        {
            //var dd = RegCurrentDate();
            return RegCurrentDate() > DateTime.UtcNow;
        }

        public static bool HasExpire()
        {
            if (HasUserChangeDate())
                return true;
            if (GetLicenceType().Equals("Infinity"))
                return false;

            return EndDate() < DateTime.UtcNow;
        }
        public static bool HasExpire(string ltype)
        {
            if (HasUserChangeDate())
                return true;
            if (GetLicenceType().Equals("Infinity"))
                return false;

                return EndDate() < DateTime.UtcNow;
        }

        public static bool Status()
        {
            var sta = FromRegistry("Status");
            var b = bool.Parse(sta);
            return b;
        }

        public static double LicenceDuration()
        {
            var r=Math.Round(DateDiff(StartDate(), EndDate()));
            return r;
        }

        public static bool IsTry()
        {
            try
            {
                var sta = FromRegistry("Version");
                return sta == "Try";
            }
            catch
            {
                return false;
            }
        }

        public static string Version()
        {
            return FromRegistry("Version"); ;
        }

        public static DateTime StartDate()
        {
            return DateTime.Parse(InfoChecker.From64Base(FromRegistry("Start")));
        }
        public static DateTime EndDate()
        {
            return DateTime.Parse(InfoChecker.From64Base(FromRegistry("End")));
        }

        public static string GetLicenceType()
        {
            return FromRegistry("Type");
        }

        /// <summary>
        /// Gets the current date in the registry
        /// </summary>
        /// <returns></returns>
        private static DateTime RegCurrentDate()
        {
            try
            {
                return DateTime.Parse(FromRegistry("CurrentDate"));
            }
            catch
            {

                return DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Caculates the left days until the activation expiration
        /// </summary>
        /// <returns></returns>
        public static double LeftDays()
        {
            DateTime start = DateTime.UtcNow;
            var r= Math.Round(DateDiff(start, EndDate()));
            return r;
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

        public static bool IsActivate()
        {
            if (IsSubKeyExist(SubKeyName))
            {
                // the activation subkey exist
                if (!HasUserChangeDate())
                {
                    // Dates are ok
                    if (!HasExpire(GetLicenceType()))
                    {
                        //try has not yet expire
                        return true;
                    }
                    return true;
                }
                return false;
            }
            else
            {
                //check if it was deleted
                if (LicenceRegInfo != null && LicenceRegInfo.Count>0)
                {
                    var lri = LicenceRegInfo[LicenceRegInfo.Count - 1];
                    if (!lri.Status)
                    {
                        //try
                        LicenceDataHolder li = new LicenceDataHolder();
                        li.StartDate = lri.Start;
                        li.EndDate = lri.End;
                        li.TypeLicence = lri.Type;
                        li.Status = lri.Status;
                        li.Version = lri.Version;
                        ActivateTry(li);
                        return IsActivate();
                    }
                    else
                    {
                        //paid
                        LicenceDataHolder li = new LicenceDataHolder();
                        li.StartDate = lri.Start;
                        li.EndDate = lri.End;
                        li.TypeLicence = lri.Type;
                        li.Status = lri.Status;
                        li.Version = lri.Version;
                        ActivateApp(li);
                        return IsActivate();
                    }
                }
                else
                {
                    return false;
                }
                
            }
            //return false;
        }

        public static bool IsActivate(string str)
        {
            if (IsSubKeyExist(SubKeyName))
            {
                // the activation subkey exist
                if (!HasUserChangeDate())
                {
                    // Dates are ok
                    if (!HasExpire(str))
                    {
                        //try has not yet expire
                        return true;
                    }
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
