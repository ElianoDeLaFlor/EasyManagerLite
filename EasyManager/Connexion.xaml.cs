using EasyManagerDb;
using EasyManagerLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
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
    /// Interaction logic for Connexion.xaml
    /// </summary>
    public partial class Connexion : Window
    {
        public Utilisateur GetUtilisateur { get; set; } = new Utilisateur();
        public Connexion()
        {
            InitializeComponent();
        }

        private void BtnCompte_Click(object sender, RoutedEventArgs e)
        {
            Compte cpt = new Compte
            {
                IsCreation = true
            };
            cpt.ShowDialog();
            txtlogin.Focus();
        }

        private void BtnValidate_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
            {
                //we're good
                GetUtilisateur = EasyManagerDb.DbManager.GetUserByLogin(txtlogin.Text.ToLower());
                if (GetUtilisateur != null)
                {
                    if (IsPassCorrect(GetUtilisateur, txtpasse.Password))
                    {
                        if (IsPassCorrect(GetUtilisateur, txtpasse.Password))
                        {
                            if (GetUtilisateur.Deleted == true)
                            {
                                MessageBox.Show(Properties.Resources.AccountDeleted, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            Home home = new Home(GetUtilisateur);
                            home.Show();
                            Close();
                        }
                        else
                            MessageBox.Show(Properties.Resources.WrongCredential, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                        MessageBox.Show(Properties.Resources.WrongCredential, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                    MessageBox.Show(Properties.Resources.WrongCredential, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                //a field is empty
                MessageBox.Show(Properties.Resources.WrongCredential, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private bool IsPassCorrect(Utilisateur utilisateur,string mdp)
        {
            return Equals(utilisateur.Password, InfoChecker.Encrypt(mdp));
        }

        private bool CheckFields()
        {
            return string.IsNullOrWhiteSpace(txtlogin.Text) || string.IsNullOrWhiteSpace(txtpasse.Password);
        }

        //private void PerformClick(Button btn)
        //{
        //    ButtonAutomationPeer peer = new ButtonAutomationPeer(btn);
        //    IInvokeProvider invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
        //    invokeProvider.Invoke();
        //}

        

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //scrollb.Height = Math.Abs(ActualHeight - 80);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AutoBackup();
            OnlineAutoBackupAsync();
        }

        private void AutoBackup()
        {
            var lst = DbManager.GetAll<BackupInfo>();
            if (lst.Count == 0) return;
            var backup = lst.Last();
            var result = InfoChecker.Backup(backup);
            if (result != 1) return;
            var query = $"UPDATE BackupInfo SET LastBackupDate='{InfoChecker.AjustDateWithTime(DateTime.UtcNow)}' WHERE Id={backup.Id}";
            DbManager.UpdateCustumQuery(query);
        }

        private void OnlineAutoBackupAsync()
        {
            if (InfoChecker.IsConnected())
            {
                var linfoserver = DbManager.GetAll<LicenceRegInfoServer>().FirstOrDefault();
                string appid = linfoserver.AppKey.ToString();
                //there are an existing backup?
                var serverbackup = DbManager.GetAll<BackupInfoServer>();
                if (serverbackup.Count > 0)
                {
                    //update
                    var bup = serverbackup[serverbackup.Count - 1];

                    string baseurl = InfoChecker.KeyValue("ApiPath");
                    //backup file
                    if (InfoChecker.Backup())
                    {
                        string tempbackup = System.IO.Path.GetTempPath() + "EasyManager";
                        string path = System.IO.Path.Combine(tempbackup, "EasyManagerDb.EasyManager");

                        var byt = File.ReadAllBytes(path);

                        if (MakeOnlineBackupUpdate(bup.AppId, appid, byt, baseurl))
                        {
                            //save backup info to local bd
                            string query = $"UPDATE BackupInfoServer SET LastBackupDate='{InfoChecker.AjustDateDb(DateTime.UtcNow)}' WHERE AppId='{appid}'";
                            DbManager.UpdateCustumQuery(query);

                            //MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                        else
                        {
                            //MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        // Unenable to copy the db to the temp file
                        //MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    //first backup
                    string baseurl = InfoChecker.KeyValue("ApiPath");
                    //backup file
                    if (InfoChecker.Backup())
                    {
                        string tempbackup = System.IO.Path.GetTempPath() + "EasyManager";
                        string path = System.IO.Path.Combine(tempbackup, "EasyManagerDb.EasyManager");

                        var byt = File.ReadAllBytes(path);

                        if (MakeOnlineBackupPost(appid, byt, baseurl))
                        {
                            //save backup info to local bd
                            BackupInfoServer backupInfoServer = new BackupInfoServer();
                            backupInfoServer.AppId = appid;
                            backupInfoServer.Date = DateTime.UtcNow;
                            backupInfoServer.Dir = path;
                            backupInfoServer.LastBackupDate = backupInfoServer.Date;

                            DbManager.Save(backupInfoServer);

                            //MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            //MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        // Unenable to copy the db to the temp file
                        //MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    }


                }
            }
        }

        private bool MakeOnlineBackupUpdate(string id, string appid, byte[] filebytearray, string baseurl)
        {
            using (var multiPartStream = new MultipartFormDataContent())
            {
                multiPartStream.Add(new StringContent(id), "Id");
                multiPartStream.Add(new ByteArrayContent(filebytearray, 0, filebytearray.Length), "BackupFileName", "EasyManagerDb.EasyManager");
                multiPartStream.Add(new StringContent(appid), "AppUserInfoId");
                //api/v1/backups/update/181a4e1e-afd7-40c9-870f-9b18efde908e
                string requesturl = $"{baseurl}/api/v1/backups/update/{id}";
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Put, requesturl);
                httpRequest.Content = multiPartStream;

                //httpRequest.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("multipart/form-data"));

                HttpClient httpClient = new HttpClient();
                using (HttpResponseMessage httpResponse = httpClient.SendAsync(httpRequest).Result)
                {
                    return httpResponse.IsSuccessStatusCode;
                }
            }
        }

        private bool MakeOnlineBackupPost(string appid, byte[] filebytearray, string baseurl)
        {
            using (var multiPartStream = new MultipartFormDataContent())
            {
                multiPartStream.Add(new ByteArrayContent(filebytearray, 0, filebytearray.Length), "BackupFileName", "EasyManagerDb.EasyManager");
                multiPartStream.Add(new StringContent(appid), "AppUserInfoId");
                //api/v1/backups/update/181a4e1e-afd7-40c9-870f-9b18efde908e
                string requesturl = $"{baseurl}/api/v1/backups/frm";
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, requesturl);
                httpRequest.Content = multiPartStream;

                //httpRequest.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("multipart/form-data"));

                HttpClient httpClient = new HttpClient();
                using (HttpResponseMessage httpResponse = httpClient.SendAsync(httpRequest).Result)
                {
                    return httpResponse.IsSuccessStatusCode;
                }
            }

        }
    }
}
