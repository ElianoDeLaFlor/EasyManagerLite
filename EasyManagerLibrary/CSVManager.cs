using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class CSVManager
    {
        public static bool WriteData<T>(string path,T t) where T : class, new()
        {
            try
            {
                using (var writer = new StreamWriter(path))
                {
                    using (var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecord<T>(t);
                        return true;
                    }
                }
            }
            catch (Exception)
            {

                return false;
            }
        }

        public static bool WriteDatas<T>(string path, List<T> t) where T : class, new()
        {
            try
            {
                using (var writer = new StreamWriter(path,false,Encoding.UTF8))
                {
                    using (var csv = new CsvHelper.CsvWriter(writer, CultureInfo.CurrentCulture))
                    {
                        csv.WriteRecords<T>(t);
                        return true;
                    }
                }
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
