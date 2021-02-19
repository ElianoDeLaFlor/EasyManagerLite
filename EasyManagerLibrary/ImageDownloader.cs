using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public static class FileDownloader
    {

        public static string Chemin { get; set; }
        static WebClient client = new WebClient();
        static Uri url;
        /// <summary>
        /// Download the image
        /// </summary>
        /// <returns>Stream</returns>
        public static Stream GetStream()
        {
            url = new Uri(Chemin);
            Stream data = client.OpenRead(url);
            return data;
        }

        public static void DownLoadFile()
        {
            url = new Uri(Chemin);

            //complete handler

            //progress notification
        }

        /// <summary>
        /// Saves the downloaded image
        /// </summary>
        /// <param name="savepath">save directory</param>
        /// <param name="filename">saved file name</param>
        /// <returns></returns>
        public static bool SaveDownload(string savepath)
        {
            try
            {
                var data = GetDownloadData(GetStream());
                if(data!=null || data.Length > 0)
                {
                    File.SetAttributes(savepath, FileAttributes.Normal);
                    File.Delete(savepath);
                    File.WriteAllBytes(savepath, data);
                    File.SetAttributes(savepath, FileAttributes.Hidden);
                    return true;
                }
                return false;
                
            }
            catch
            {
                return false;
            }
        }

        public static byte[] GetDownloadData(Stream stream)
        {
            using(MemoryStream ms=new MemoryStream())
            {
                stream.CopyTo(ms);
                

                return ms.ToArray();
            }
        }
    }
}
