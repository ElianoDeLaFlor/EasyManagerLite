using EasyManagerDb;
using EasyManagerLibrary;
using Licence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using APIManagerLibrary.APIResponse;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for KeyActivation.xaml
    /// </summary>
    public partial class KeyActivation : Window
    {
        readonly UnicodeEncoding ByteConverter = new UnicodeEncoding();
        private readonly RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
        private string Key { get; set; } = $"DatePaiement=12/03/2019;MethodePaiement=PayPal;" +
            $"Name=Eliano;Duration=3;DateDebut=12/04/2019;DateFin=11/04/2019";
        
        string CypherText;
        string EncryptText;
        string DecryptText;
        public Home GetHome { get; set; }
        public KeyActivation()
        {
            InitializeComponent();
            CreateTableLicenceInfoServer();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtcode.Text.Length <= 0)
                return;
            try
            {
                CypherText = txtcode.Text;
                DecryptText = InfoChecker.DecryptData(CypherText, true);
                //check if the code is already used
                if(await IsAlreadyUsed(CypherText))
                {
                    // the licence code is already used
                    MessageBox.Show(Properties.Resources.CodeUsed, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    //Todo check if the code is already used by anyone else
                    return;
                }

                //licence information
                var data = InfoChecker.ManageActivationCodeInfo(DecryptText);
                data.Code = CypherText;
                //if(data.)
                if (data.HasExpired)
                {
                    // the licence has expired
                    MessageBox.Show(Properties.Resources.ActivationEntredExpire, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    //save licence info to database
                    int id= DbManager.SaveData(data);
                    // activate the application
                    if (id > 0)
                    {
                        if (LicenceInfo.ActivateApp(data,id))
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
                                Version = "Paid",
                                Status = true,
                                Type = data.TypeLicence
                            };
                            DbManager.Save(regInfoServer);

                            ////Enregistre les info du registre sur le serveur
                            if (InfoChecker.IsConnected())
                            {
                                //todo reverse connection check
                                var rslt = await Saveregliceninfoserver(regInfoServer);
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
                                var rslt = await Saveliceninfoserver(infoServer);
                            }

                            //App activated
                            MessageBox.Show(Properties.Resources.ActivationSuccess, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                            GetHome.IsLicencing = true;
                            InfoChecker.RestartApp();
                        }
                        else
                        {
                            //activation error
                            if(MessageBox.Show(Properties.Resources.ActivationError, Properties.Resources.MainTitle, MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                            {
                                GetHome.IsLicencing = true;
                                InfoChecker.RestartApp();
                            }
                        }
                    }
                    else
                    {
                        // error saving into database
                        if (MessageBox.Show(Properties.Resources.ActivationError, Properties.Resources.MainTitle, MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                        {
                            GetHome.IsLicencing = true;
                            InfoChecker.RestartApp();
                        }
                    }
                    
                }
                //display the decrypted code to the user
                //txtcode.Text = DecryptText;
            }
            catch (Exception)
            {

                MessageBox.Show(Properties.Resources.ActivationCodeInvalid, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            


        }

        private async Task<bool> Saveliceninfoserver(LicenceInfoServer infoServer)
        {
            ApiResponse.BaseUrl = InfoChecker.KeyValue("ApiPath");
            ApiResponse.Url = "api/v1/LicenceInformation";
            var rslt = await ApiResponse.PostJson(infoServer);

            return rslt.Item2 == System.Net.HttpStatusCode.OK;

        }

        private async Task<LicenceInfoServer> Getliceninfoserver(string code)
        {
            ApiResponse.BaseUrl = InfoChecker.KeyValue("ApiPath");
            ApiResponse.Url = $"api/v1/LicenceInformation/Code";
            LicenceCode licenceCode = new LicenceCode
            {
                Code = code
            };
            var rslt = await ApiResponse.PostWithData<LicenceInfoServer,LicenceCode>(licenceCode);

            if (rslt.Item2 == System.Net.HttpStatusCode.OK)
                return rslt.Item1;
            return null;

        }

        private async Task<bool> Saveregliceninfoserver(LicenceRegInfoServer infoServer)
        {
            ApiResponse.BaseUrl = InfoChecker.KeyValue("ApiPath");
            ApiResponse.Url = "api/v1/LienceRegInfoServers";
            var rslt = await ApiResponse.Post(infoServer);

            return rslt.Item2 == System.Net.HttpStatusCode.OK;

        }

        private void gen_Click(object sender, RoutedEventArgs e)
        {
            EncryptText = InfoChecker.EncryptData(Key, true);
            string Base64 = Convert.ToBase64String(ByteConverter.GetBytes(EncryptText));
            txtcode.Text = Base64;
        }

        private async Task<bool> IsAlreadyUsed(string code)
        {
            var licencelst = DbManager.GetByColumnName<LicenceInformation>("Code", code);
            if (licencelst.Count != 0)
            {
                //activation key Is Already Used
                return true;
            }
            else
            {
                if (InfoChecker.IsConnected())
                {
                    var l = await Getliceninfoserver(code);
                    return l != null;
                }
                return false;
            }

        }

        private String GenAppKey()
        {
            var appkey = new Guid();
            return appkey.ToString();
        }

        private bool SaveAppKey(string key)
        {
            //ckeck if appkey exist
            if (!AppKeyExist())
            {
                // not exist
                Settings settings = new Settings
                {
                    CreationDate = DateTime.UtcNow,
                    Data = key,
                    Name = "AppKey"
                };

                return DbManager.Save(settings);
            }
            return true;
        }

        private bool AppKeyExist()
        {
            var appkey = DbManager.GetByColumnName<Settings>("Name", "AppKey");
            return appkey != null;
        }

        private void CreateTableLicenceInfoServer()
        {
            //create table if not exist
            string query = "CREATE TABLE IF NOT EXISTS LicenceRegInfoServer (" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "Start TEXT," +
                    "End TEXT," +
                    "Version TEXT," +
                    "EasyId TEXT," +
                    "Status Integer," +
                    "Type TEXT," +
                    "AppKey TEXT)";
            DbManager.CreateNewTable(query);
        }
    }
}
