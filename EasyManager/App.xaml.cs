using EasyManagerLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        App()
        {
            //get the system language
            CultureInfo ci = CultureInfo.InstalledUICulture;
            var langcode = ci.TwoLetterISOLanguageName;
            //if is french set the application language to french
            
            if (GetAppLang() == "fr")//langcode ==GetAppLang()
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            }
            //otherwise set the language to english

            
        }
        private string GetAppLang()
        {
            return InfoChecker.KeyValue("Lang");
        }
    }
}
