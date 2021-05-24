using APIManagerLibrary.APIResponse;
using EasyManagerDb;
using EasyManagerLibrary;
using Licence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for Activation.xaml
    /// </summary>
    public partial class Activation : Window
    {
        public string ActivationStatus { get; set; } = "testing";
        public double LeftDays { get; set; }=32.4;
        public double JourRestant { get; set; } = 0;
        public string LicenceEnd { get; set; } = "votre lience expire dans 54 jours";
        public bool ShowTryButton { get; set; }
        public bool ActivatationState { get; set; }
        public Visibility ShowTry { get; set; }
        public Visibility LicenceDetails { get; set; }
        public LicenceInformation LicenceInformation { get; set; } = null;
        public string AppId { get; set; }
        public Activation()
        {
            InitializeComponent();
            var LInformation = GetLicence();

            LicenceInformation = LInformation;

            DataContext = this;
        }
        public Activation(DateTime dateTime,bool expire)
        {
            InitializeComponent();
            if(expire)
                LicenceEnd = $"{Properties.Resources.ActivationExpire} {dateTime.ToLongDateString()}";
            else
                LicenceEnd = $"{Properties.Resources.ActivationEnd} {dateTime.ToLongDateString()}";

            var LInformation = GetLicence();

            LicenceInformation = LInformation;
            SetAppKey();

            DataContext = this;
        }
        public Home GetHome { get; set; }

        private void btnactivate_Click(object sender, RoutedEventArgs e)
        {
            KeyActivation keyActivation = new KeyActivation
            {
                GetHome = GetHome
            };
            keyActivation.ShowDialog();
        }

        private LicenceInformation GetLicence()
        {
            int id= LicenceInfo.FromRegistry("EasyId").ToInt();
            LicenceInformation information = EasyManagerDb.DbManager.GetById<LicenceInformation>(id);
            return information;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ManagerTable("TypeLicence");
        }

        private async void Btntry_Click(object sender, RoutedEventArgs e)
        {
            var data = InfoChecker.GetTryLicence();
            //save licence info to database
            int id = DbManager.SaveData(data);
            if (id > 0)
            {
                if (LicenceInfo.ActivateTry(data, id))
                {
                    //Gen AppKey
                    string key;
                    if (!AppKeyExist())
                    {
                        key = GenAppKey();
                        SaveAppKey(key);
                    }
                    else
                    {
                        key = DbManager.GetByColumnName<Settings>("Name", "AppKey").FirstOrDefault().Data;
                    }
                    //Enregistre les info du registre dans la base locale
                    LicenceRegInfoServer regInfoServer = new LicenceRegInfoServer
                    {
                        AppKey = key,
                        EasyId = id,
                        Start = Convert.ToBase64String(InfoChecker.ToByteArray(data.StartDate.ToShortDateString())),
                        End = Convert.ToBase64String(InfoChecker.ToByteArray(data.EndDate.ToShortDateString())),
                        Version = "Try",
                        Status = false,
                        Type = data.TypeLicence
                    };
                    DbManager.Save(regInfoServer);

                    //Enregistre les info du registre sur le serveur
                    if (InfoChecker.IsConnected())
                    {
                        _ = await Saveregliceninfoserver(regInfoServer);
                    }

                    //Save lience information on the server
                    LicenceInfoServer infoServer = new LicenceInfoServer
                    {
                        AppKey = new Guid(key),
                        Code = data.Code,
                        Duration = data.Duration,
                        EndDate = data.EndDate,
                        HasExpired = data.HasExpired,
                        Name = data.Name,
                        PaymentDate = data.PaymentDate,
                        PaymentMethod = data.PaymentMethod,
                        StartDate = data.StartDate,
                        TypeLicence = data.TypeLicence
                    };
                    //Save lience information to the local database
                    DbManager.Save(infoServer);
                    //check for internet
                    if (InfoChecker.IsConnected())
                    {
                        _ = await Saveliceninfoserver(infoServer);
                    }
                    
                    //App activated
                    MessageBox.Show($"{Properties.Resources.ActivateTryInfo}", Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    InfoChecker.RestartApp();
                    Close();
                }
                else
                {
                    //activation error
                    if (MessageBox.Show(Properties.Resources.ActivationError, Properties.Resources.MainTitle, MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                    {
                        InfoChecker.RestartApp();
                       // Close();
                    }
                }
            }
            else
            {
                // error saving into database
                if (MessageBox.Show(Properties.Resources.ActivationError, Properties.Resources.MainTitle, MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    InfoChecker.RestartApp();
                   // Close();
                }
            }

        }

        private async Task<bool> Saveliceninfoserver(LicenceInfoServer infoServer)
        {
            ApiResponse.BaseUrl = InfoChecker.KeyValue("ApiPath");
            ApiResponse.Url = "api/v1/LicenceInformation";
            var rslt = await ApiResponse.Post(infoServer);

            return rslt.Item2 == System.Net.HttpStatusCode.OK;

        }

        private async Task<bool> Saveregliceninfoserver(LicenceRegInfoServer infoServer)
        {
            ApiResponse.BaseUrl = InfoChecker.KeyValue("ApiPath");
            ApiResponse.Url = "api/v1/LienceRegInfoServers";
            var rslt = await ApiResponse.Post(infoServer);

            return rslt.Item2 == System.Net.HttpStatusCode.OK;

        }

        public void ManagerTable(string columnName)
        {
            //check if the column already exist the table
            if (!DbManager.CustumQueryCheckColumn<LicenceInformation>(columnName))
            {
                // the column is not in the table
                // so we have to create it
                DbManager.CreateNewColumn<LicenceInformation>(columnName, "TEXT", "'Periodic'");
            }

        }

        private String GenAppKey()
        {
            var appkey = Guid.NewGuid();
            return appkey.ToString();
        }

        private bool SaveAppKey(string key)
        {
            //ckeck if appkey exist
            if (!AppKeyExist())
            {
                // not exist
                Settings settings = new Settings();
                settings.CreationDate = DateTime.UtcNow;
                settings.Data = key;
                settings.Name = "AppKey";

                return DbManager.Save(settings);
            }
            return true;
        }

        private bool AppKeyExist()
        {
            var appkey = DbManager.GetByColumnName<Settings>("Name", "AppKey");
            return appkey.Count!=0;
        }

        private Settings GetAppKey()
        {
            var query = "SELECT * FROM  Settings WHERE Name='AppKey'";
            var rslt = DbManager.CustumQuery<Settings>(query);

            if (rslt.Count == 0)
                return null;
            else
                return rslt.FirstOrDefault();
        }

        private void SetAppKey()
        {
            var appkey = GetAppKey();
            if (appkey == null)
                AppId = "";
            else
                AppId = appkey.Data;
        }
    }

}
