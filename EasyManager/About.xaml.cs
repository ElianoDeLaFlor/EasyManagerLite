using APIManagerLibrary.APIResponse;
using EasyManagerDb;
using EasyManagerLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window,INotifyPropertyChanged
    {
        private bool _pudateprog;
        private Visibility _show;
        private int _downloadprogress;

        private string _appinfo;

        public string AppInfo
        {
            get { return _appinfo; }
            set { _appinfo = value; OnPropertyChanged("AppInfo"); }
        }


        public Home GetHome { get; set; }
        public About()
        {
            InitializeComponent();
            DataContext = this;
        }


        public About(Home home)
        {
            InitializeComponent();
            DataContext = this;
            GetHome = home;
        }

        public bool UpdateProgress
        {
            get => _pudateprog;
            set
            {
                if (_pudateprog == value)
                    return;
                _pudateprog = value;
                OnPropertyChanged();
            }
        }

        public int DownloadProgress
        {
            get => _downloadprogress;
            set
            {
                if (_downloadprogress == value)
                    return;
                _downloadprogress = value;
                OnPropertyChanged();
            }
        }

        public Visibility ShowProgress
        {
            get => _show;
            set
            {
                if (_show == value)
                    return;
                _show = value;
                OnPropertyChanged();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowProgress = Visibility.Hidden;
            var versioninfo = DbManager.GetAll<VersionInfo>();
            if (versioninfo.Count == 0)
            {
                // fallback values
                txtversion.Text = $"{InfoChecker.KeyValue("AppVersion")}";
            }
            else
            {
                var version = versioninfo.Last();
                txtversion.Text = $"{Properties.Resources.Version} {version.VersionNumber}";
            }
            SetAppKey();
        }

        private void Btnclose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void btnupdate_Click(object sender, RoutedEventArgs e)
        {
            if (InfoChecker.IsConnected())
            {
                var key = DbManager.GetByColumnName<Settings>("Name", "AppKey").FirstOrDefault().Data;
                UpdateProgress = true;
                if (await CheckUpdate(key))
                {
                    // there is an update
                    UpdateProgress = false;
                    if (MessageBox.Show(Properties.Resources.ThereIsUpdate, Properties.Resources.MainTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        // proced the download
                        //Download the update

                        //if (UpdateFile())
                        //{
                        //    CallUpdater("AutoUpdater.exe");
                        //    GetHome.IsLicencing = true;
                        //    InfoChecker.ShutdownApp();
                        //}
                        //else
                        //{
                        //    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                        //}
                        var baseurl = InfoChecker.KeyValue("ApiPath");
                        string url = $"{ baseurl}api/v1/AppVersions/Download";
                        DownLoadFile(url);

                    }
                }
                else
                {
                    // There is no update
                    UpdateProgress = false;
                    MessageBox.Show(Properties.Resources.ThereIsNoUpdate, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.NoConnection, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            
        }

        private async Task<bool> CheckUpdate(string appkey)
        {
            ApiResponse.BaseUrl = InfoChecker.KeyValue("ApiPath");
            ApiResponse.Url = $"api/v1/AppVersions/CheckUpdate/{appkey}";
            var version = InfoChecker.KeyValue("AppVersion").Split('.');
            AppVersion appVersion = new AppVersion
            {
                Major = int.Parse(version[0]),
                Minor = int.Parse(version[1]),
                Build = int.Parse(version[2]),
                Revision = int.Parse(version[3])
            };

            var rslt = await ApiResponse.PostWithData<bool, AppVersion>(appVersion);

            if (rslt.Item2 == System.Net.HttpStatusCode.OK)
                return rslt.Item1;
            return false;
            

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CallUpdater(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = path;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            string tempbackup = System.IO.Path.GetTempPath() + @"EasyManager\Update";
            string param = System.IO.Path.Combine(tempbackup, "EasyManagerSetup.msi");
            startInfo.Arguments = param;
            try
            {
                using(Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private bool UpdateFile()
        {
            var baseurl = InfoChecker.KeyValue("ApiPath");
            return GetOnlineBackupFile(baseurl);
        }

        private bool GetOnlineBackupFile(string baseurl)
        {
            try
            {
                FileDownloader.Chemin = $"{baseurl}api/v1/AppVersions/Download";
                string tempbackup = System.IO.Path.GetTempPath() + @"EasyManager\Update";
                if (!Directory.Exists(tempbackup))
                    Directory.CreateDirectory(tempbackup);
                string path = System.IO.Path.Combine(tempbackup, "EasyManagerSetup.msi");
                if (File.Exists(path))
                    File.Delete(path);
                return FileDownloader.SaveDownload(path);
            }
            catch
            {

                return false;
            }

        }

        public void DownLoadFile(string address)
        {
            string tempbackup = System.IO.Path.GetTempPath() + @"EasyManager\Update";
            if (!Directory.Exists(tempbackup))
                Directory.CreateDirectory(tempbackup);
            string path = System.IO.Path.Combine(tempbackup, "EasyManagerSetup.msi");
            if (File.Exists(path))
                File.Delete(path);

            WebClient client = new WebClient();
            var url = new Uri(address);

            //complete handler
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(FileDownlodedCallback);

            //progress notification
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);

            client.DownloadFileAsync(url, path);
        }

        private void DownloadProgressCallback(object sender,DownloadProgressChangedEventArgs e)
        {
            //Console.WriteLine("{0} downloaded {1} of {2} bytes. {3} % complete ...", (string)e.UserState, e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage);
            ShowProgress = Visibility.Visible;
            DownloadProgress = e.ProgressPercentage;
        }

        private void FileDownlodedCallback(object sender,AsyncCompletedEventArgs e)
        {
            if(e.Cancelled)
            {
                //
                ShowProgress = Visibility.Hidden;
            }
            if (e.Error != null)
            {
                ShowProgress = Visibility.Hidden;
            }
            CallUpdater("AutoUpdater.exe");
            GetHome.IsLicencing = true;
            InfoChecker.ShutdownApp();
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
                AppInfo = "";
            else
                AppInfo = $"AppId: {appkey.Data}";
        }
    }
}
