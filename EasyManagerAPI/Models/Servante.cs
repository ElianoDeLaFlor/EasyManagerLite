using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EasyManagerAPI.Models
{
    public class Servante
    {
        /// <summary>
        /// Recreate a file name
        /// </summary>
        /// <param name="Fpath">Folder name</param>
        /// <param name="filename">File Name</param>
        /// <param name="k"></param>
        /// <returns>the new created file name</returns>
        public static string FileSavePath(string Fpath, string filename, out string k)
        {

            var name = Guid.NewGuid().ToString().Replace("-", "");
            name += GetExtension(filename);
            filename = name;
            string newpath = Path.Combine(Fpath, filename);
            int p = 1;
            string s = filename;
            while (IsFileExist(newpath))
            {
                s = $"{p}{filename}";
                newpath = Path.Combine(Fpath, s);
                k = $"{p}{filename}";
                p++;
            }
            k = s;
            return newpath;
        }

        public static string GetExtension(string str)
        {
            FileInfo fi = new FileInfo(str);
            return fi.Extension;
        }

        /// <summary>
        /// Check if file exist
        /// </summary>
        /// <param name="path">path to check</param>
        /// <returns>returns true if file exist otherwise false</returns>
        public static bool IsFileExist(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="p">file path to delete</param>
        /// <returns> True if deleted or false if not</returns>
        public static bool Delete(string p)
        {
            try
            {
                if (File.Exists(p))
                {
                    File.SetAttributes(p, FileAttributes.Normal);
                    File.Delete(p);
                }
                return true;

            }
            catch
            {
                return false;
            }

        }

        public static void CheckPath(string str)
        {
            if (!Directory.Exists(str))
                Directory.CreateDirectory(str);

        }
        /// <summary>
        /// read the file at the specified location in to byte array
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] DownloadImage(string path)
        {
            byte[] byt = File.ReadAllBytes(path);
            return byt;
        }

        public static int AppVersionToNumber(AppVersion version)
        {
            string str = $"{version.Major}{version.Minor}{version.Build}{version.Revision}";
            int val = int.Parse(str);
            return val;
        }

        public static int CompareAppVersion(AppVersion x,AppVersion y)
        {
            if (x.Major < y.Major)
                return -1;
            if (x.Major > y.Major)
                return 1;
            if (x.Major == y.Major)
            {
                if (x.Minor < y.Minor)
                    return -1;
                if (x.Minor > y.Minor)
                    return 1;
                if (x.Minor == y.Minor)
                {
                    if (x.Build < y.Build)
                        return -1;
                    if (x.Build > y.Build)
                        return 1;
                    if (x.Build == y.Build)
                    {
                        if (x.Revision < y.Revision)
                            return -1;
                        if (x.Revision > y.Revision)
                            return 1;
                    }
                }

            }
            return 0;
        }
    }
}