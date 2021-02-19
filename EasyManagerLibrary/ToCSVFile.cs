using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class ToCSVFile
    {
        public StreamWriter GetStreamWritter { get; set; }
        public string FileName { get; set; }

        public bool Write<T,U>(T t, U u) where U : class, new() 
        {
            try
            {
                U result = new U();
                CsvHelper.Configuration.CsvConfiguration csvConfiguration = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture);
                var csv = new CsvWriter(GetStreamWritter, csvConfiguration);
                csv.WriteHeader(u.GetType());
                csv.WriteRecord(t);
                //foreach (var item in t)
                //{
                //    csv.WriteRecords(item);
                //}
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Write(List<Produit> t)
        {
            try
            {
                //T result = new T();
                //GetStreamWritter = new StreamWriter(FileName);
                CsvHelper.Configuration.CsvConfiguration csvConfiguration = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture);
                var csv = new CsvWriter(GetStreamWritter, csvConfiguration);
                csv.WriteHeader(typeof(Produit));

                foreach (var item in t)
                {
                    csv.WriteRecord(item);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
