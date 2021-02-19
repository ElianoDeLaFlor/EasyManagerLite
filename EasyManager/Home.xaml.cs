using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EasyManager.MenuItems;
using EasyManagerDb;
using EasyManagerLibrary;
using Licence;
using LiveCharts;
using LiveCharts.Wpf;
using MaterialDesignThemes.Wpf;
using EasyLib = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window, INotifyPropertyChanged
    {
        public static Snackbar Snackbar;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> Menu { get; set; }
        private int MenuIndex { get; set; }
        public List<string> NombreJour { get; set; }
        private List<string> lstprod { get; set; }
        private List<string> lstjour { get; set; }
        private int ProdIndex { get; set; } = 0;
        private int JourIndex { get; set; } = 0;
        private MainWindowViewModel _Context;
        private bool IsReconnexion { get; set; }
        private bool IsClicked { get; set; } = true;
        public bool IsLicencing { get; set; }
        public Parametre GetParametre { get; set; }

        public Home()
        {
            InitializeComponent();
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(2500);
            }).ContinueWith(t =>
            {
                //note you can use the message queue from any thread, but just for the demo here we 
                //need to get the message queue from the snackbar, so need to be on the dispatcher
                MainSnackbar.MessageQueue.Enqueue(Properties.Resources.WelcomMessage);
            }, TaskScheduler.FromCurrentSynchronizationContext());
            _Context = new MainWindowViewModel(MainSnackbar.MessageQueue, this);
            DataContext = _Context;//new MainWindowViewModel(MainSnackbar.MessageQueue, this);

            Snackbar = MainSnackbar;
        }

        public Home(Utilisateur user)
        {
            InitializeComponent();
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(2500);
            }).ContinueWith(t =>
            {
                //note you can use the message queue from any thread, but just for the demo here we 
                //need to get the message queue from the snackbar, so need to be on the dispatcher
                MainSnackbar.MessageQueue.Enqueue(Properties.Resources.WelcomMessage);
            }, TaskScheduler.FromCurrentSynchronizationContext());
            _Context = new MainWindowViewModel(MainSnackbar.MessageQueue, this);
            //new MainWindowViewModel(MainSnackbar.MessageQueue, this);

            Snackbar = MainSnackbar;
            IsReconnexion = true;
            _Context.ConnectedUser = user;

            //DataContext = _Context;
        }

        public MainWindowViewModel DataContextt { get { return _Context; } set { _Context = value; OnPropertyChanged("DataContextt"); } }

        private async  void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (await ActivationManagerAsync())
            {
                //Application activate
                //Check if Activation has expired
                if (LicenceInfo.HasExpire())
                {
                    ShowActivationInfo(true);
                    IsLicencing = true;
                    Close();
                    
                }
                else
                {
                    if (IsReconnexion)
                    {
                        MenuToggleButton.Visibility = Visibility.Visible;
                        _Context.IsConnected = Visibility.Visible;
                        SetMenu();
                    }
                    //Set notification count on start
                    _Context.NotificationCount = DbManager.GetAll<EasyLib.Notifications>().Count();
                    /*CreateTable();
                    CreateTableSettings();
                    CreateTableLicenceInfoServer();
                    CreateTableBackupServer();
                    CreateTableAppUserInfo();
                    ManagerTable("TypeLicence");
                    ManagerTable("PrixGrossiste", "REAL", "0");
                    ManagerClientTable("ClientType", "INTEGER", "0");
                    ManagerCompanyTable("Consigne", "Text","");*/
                    WatchDriveAdding();
                    WatchDriveRemoving();
                    NetWorkChange();
                }
            }
            else
            {
                //Application not activate
                MessageBox.Show(Properties.Resources.AppNotActivate, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                Tuple<double, double> tuple = new Tuple<double, double>(0, 0);
                if(LicenceInfo.HasUserChangeDate())
                    ActivationWindow(Properties.Resources.NoLicence, tuple, DateTime.UtcNow,false,Visibility.Hidden);
                else
                    ActivationWindow(Properties.Resources.NoLicence, tuple, DateTime.UtcNow);
                IsLicencing = true;
                Close();
            }
        }

        private void ActivationWindow2(string status,Tuple<double,double> day,DateTime date,bool expire=false,Visibility visible=Visibility.Visible)
        {
            Activation activation = new Activation(date,expire);
            activation.GetHome = this;
            activation.ActivationStatus = status;
            activation.LeftDays = day.Item1;
            activation.JourRestant = day.Item2;
            activation.ShowTry = visible;
            activation.Show();
        }

        private void ActivationWindow(string status, Tuple<double, double> day, DateTime date, bool expire = false, Visibility visible = Visibility.Visible)
        {
            Activation activation = new Activation(date, expire);
            activation.GetHome = this;
            activation.ActivationStatus = status;
            activation.LeftDays = day.Item1;
            activation.JourRestant = day.Item2;
            activation.ShowTry = visible;
            var licencetype = LicenceInfo.GetLicenceType();
            if(licencetype!=null)
                activation.LicenceDetails = licencetype.Equals("Infinity") ? Visibility.Collapsed : Visibility.Visible;
            activation.Show();
        }



        private void SetMenu()
        {
            //Get user role
            var userrole = DbManager.GetByColumnName<Role>("Libelle", _Context.ConnectedUser.RoleLibelle);
            // Get modules that user can access
            var lst = UserMenus(userrole[0].Id);

            foreach (var item in lst)
            {
                //Categorie|Client|ClientList|Compte|Discount|DiscounteListe|EditionQuantite|Produit|ProduitListe|RoleModule|Vente|VenteCredit|VenteCreditList|Liste des comptes
                if (item == Properties.Resources.ModuleCategorie)
                {
                    MenuContent menu = new MenuContent("Hexagons", Properties.Resources.CategoryTitle, item, new CategorieUC(this));
                    _Context.MenuContent = menu;
                }
                else if (item == Properties.Resources.ModuleClient)
                {
                    MenuContent menu = new MenuContent("PersonTie", Properties.Resources.ClientTitle, item, new ClientUC(this));
                    _Context.MenuContent = menu;
                }
                else if (item == Properties.Resources.ModuleCompte)
                {
                    MenuContent menu = new MenuContent("AccountAdd", Properties.Resources.CompteTitle, item, new CompteUC(this));
                    _Context.MenuContent = menu;
                }
                else if (item == Properties.Resources.ModuleDiscount)
                {
                    MenuContent menu = new MenuContent("TrolleyMinus", Properties.Resources.Discount, item, new DiscountUC(this));
                    _Context.MenuContent = menu;
                }
                else if (item == Properties.Resources.ModuleProduit)
                {
                    MenuContent menu = new MenuContent("DatabasePlus", Properties.Resources.ProduitTitle, item, new ProduitUC(this));
                    _Context.MenuContent = menu;
                }
                else if (item == Properties.Resources.ModuleEditonQuantite)
                {
                    MenuContent menu = new MenuContent("DatabaseEdit", Properties.Resources.ModuleEditonQuantite, item, new EditionQuantiteUC(this));
                    _Context.MenuContent = menu;
                }
                else if (item == Properties.Resources.ModuleVente)
                {
                    MenuContent menu = new MenuContent("CashMultiple", Properties.Resources.ModuleVente, item, new VenteUC(this));
                    _Context.MenuContent = menu;
                }
                else if (item == Properties.Resources.ModuleVenteCredit)
                {
                    MenuContent menu = new MenuContent("CashRefund", Properties.Resources.VenteCreditTitle, item, new VenteCreditUC(this));
                    _Context.MenuContent = menu;
                }
                else if (item == Properties.Resources.ModuleVenteCredit)
                {
                    MenuContent menu = new MenuContent("Collage", Properties.Resources.ModuleListeRole, item, new CategorieListeUC(this));
                    _Context.MenuContent = menu;
                }
                else if (item == Properties.Resources.ModuleListClient)
                {
                    MenuContent menu = new MenuContent("AccountSupervisor", Properties.Resources.ModuleListClient, item, new ClientListeUC(this));
                    _Context.MenuContent = menu;
                }
                else if (item == Properties.Resources.ModuleDiscountList)
                {
                    MenuContent menu = new MenuContent("ViewSequential", Properties.Resources.ModuleDiscountList, item, new DiscountListUC(this));
                    _Context.MenuContent = menu;
                }
                else if (item == Properties.Resources.ModuleListeProduits)
                {
                    MenuContent menu = new MenuContent("AlphaPBox", Properties.Resources.ModuleListeProduits, item, new ProduitListUC(this));
                    _Context.MenuContent = menu;
                }
                else if (item == Properties.Resources.ModuleEditonQuantiteListe)
                {
                    MenuContent menu = new MenuContent("BusDoubleDecker", Properties.Resources.ModuleEditonQuantiteListe, item, new EditionQuantiteListeUC(this));
                    _Context.MenuContent = menu;
                }
                else if (item == Properties.Resources.ModuleListeVente)
                {
                    MenuContent menu = new MenuContent("CurrencyNgn", Properties.Resources.ModuleListeVente, item, new VenteListeUC(this));
                    _Context.MenuContent = menu;
                }
                else if (item == Properties.Resources.ModuleListeVenteCredit)
                {
                    MenuContent menu = new MenuContent("CreditCardRefund", Properties.Resources.ModuleListeVenteCredit, item, new VenteCreditListeUC(this));
                    _Context.MenuContent = menu;
                }
                else if (item == Properties.Resources.ModuleListeRole)
                {
                    MenuContent menu = new MenuContent("Cards", Properties.Resources.ModuleListeRole, item, new RoleListUC(this));
                    _Context.MenuContent = menu;
                }
                else if (item == Properties.Resources.ModuleListeUtilisateur)
                {
                    MenuContent menu = new MenuContent("AccountGroup", Properties.Resources.ModuleListeUtilisateur, item, new UserListUC(this));
                    _Context.MenuContent = menu;
                }
            }
            DataContext = _Context;
            MenuItemsListBox.ItemsSource = null;
            MenuItemsListBox.ItemsSource = _Context.MenuContents;
            //MenuItemsListBox.SelectedIndex = 0;
            DataContextt.MenuIndex = 0;
        }

        private List<string> UserMenus(int roleid)
        {
            //get user allowed modules
            var modules = DbManager.GetByColumnName<RoleModule>("RoleId", roleid.ToString());
            //get menu that user can access
            List<string> menu = new List<string>();
            foreach (var item in modules)
            {
                menu.Add(DbManager.GetById<Module>(item.ModuleId).Libelle);
            }
            return menu;
        }

        private void OnHover(object sender, RoutedEventArgs e)
        {
            //#46ad46
            var origine = sender as ListBoxItem;
            origine.Foreground = new SolidColorBrush(Colors.MediumSeaGreen);
        }
        private void OnLeave(object sender, RoutedEventArgs e)
        {
            var origine = sender as ListBoxItem;
            origine.Foreground = new SolidColorBrush(Colors.Black);
        }
        /// <summary>
        /// Close the drawer on an items click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private void MenuPopupButton_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void Prod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*var box = sender as ComboBox;
            ProdIndex = box.SelectedIndex;
            //Mise à jour du textblock info
            LblInfo.Text = $"{Properties.Resources.LblInfoOne} {lstprod[ProdIndex]} {Properties.Resources.LblInfoTwo} {lstjour[JourIndex]} {Properties.Resources.LblInfoThree}";
            PieChart.Datas = ChartData(int.Parse(lstjour[JourIndex]), GetProductIdByName(lstprod[ProdIndex]));*/
        }

        private void NbrJour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*var box = sender as ComboBox;
            JourIndex = box.SelectedIndex;
            //Mise à jour du textblock info
            LblInfo.Text = $"{Properties.Resources.LblInfoOne} {lstprod[ProdIndex]} {Properties.Resources.LblInfoTwo} {lstjour[JourIndex]} {Properties.Resources.LblInfoThree}";

            //PieChart.Datas = testData(int.Parse(lstjour[JourIndex]));
            PieChart.Datas = ChartData(int.Parse(lstjour[JourIndex]), GetProductIdByName(lstprod[ProdIndex]));*/
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainSnackbar.MessageQueue.Enqueue("dfghjjjjjjhhj");
            Snackbar = MainSnackbar;
        }

        private void BtnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            //_Context.ConnectedUser = null;
            //Connexion connexion = new Connexion();
            //connexion.Show();
            Close();

        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btnnotif_Click(object sender, RoutedEventArgs e)
        {
            NotificationUI notificationUI = new NotificationUI();
            notificationUI.ShowDialog();
            _Context.NotificationCount = DbManager.GetAll<EasyLib.Notifications>().Count();
        }

        private void btnparametre_Click(object sender, RoutedEventArgs e)
        {
            Parametre parametre = new Parametre(this);
            parametre.ShowDialog();
        }

        private void btnshowtext_Click(object sender, RoutedEventArgs e)
        {
            if (IsClicked)
            {
                //clicked => hide text

                var lstmenu = DataContextt.MenuContents;
                foreach (var item in lstmenu)
                {
                    item.ShowText = Visibility.Visible;
                }
                DataContextt.MenuContents = lstmenu;
                IsClicked = false;
            }
            else
            {
                //uncliked => show text
                var lstmenu = DataContextt.MenuContents;
                foreach (var item in lstmenu)
                {
                    item.ShowText = Visibility.Collapsed;
                }
                DataContextt.MenuContents = lstmenu;
                IsClicked = true;
            }
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            PasseChange passeChange = new PasseChange(DataContextt.ConnectedUser);
            passeChange.ShowDialog();
        }

        private void btnrapport_Click(object sender, RoutedEventArgs e)
        {
            Rapport rapport = new Rapport(this);
            rapport.ShowDialog();
        }

        private void btnnewsupplier_Click(object sender, RoutedEventArgs e)
        {
            Supplier supplier = new Supplier(this);
            supplier.ShowDialog();
        }

        private void btnlistsupplier_Click(object sender, RoutedEventArgs e)
        {
            SupplierList supplierList = new SupplierList(this);
            supplierList.ShowDialog();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //closing because of licence issues
            if (IsLicencing)
                e.Cancel = false;
            else
            {
                // normal closing
                if (MessageBox.Show(Properties.Resources.AreYouSureToContinue, Properties.Resources.MainTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    e.Cancel = true;
                else
                {
                    Connexion connexion = new Connexion();
                    connexion.Show();
                }

            }
        }

        private async Task<bool> ActivationManagerAsync()
        {
            return await Task.Run(() => ActivationSync());
        }

        private bool ActivationSync()
        {
            var lri = DbManager.GetAll<LicenceRegInfoServer>();
            
            LicenceInfo.LicenceRegInfo = lri;
            return LicenceInfo.IsActivate();
        }

        private void btnactivation_Click(object sender, RoutedEventArgs e)
        {
            ShowActivationInfo(LicenceInfo.HasExpire());
        }

        private void ShowActivationInfo(bool expire)
        {
            string state;
            if (LicenceInfo.IsTry())
            {
                if (LicenceInfo.HasExpire())
                {
                    state = Properties.Resources.TryExpired;
                }
                else
                    state = Properties.Resources.IsTryVersion;
            }
            else
            {
                state = Properties.Resources.Activated;
            }

            var leftdays = InfoChecker.ProgressValue(LicenceInfo.LicenceDuration(),LicenceInfo.LeftDays());

            DateTime datetime = LicenceInfo.EndDate();
            ActivationWindow(state, leftdays, datetime,expire,Visibility.Hidden);
        }

        private void Btncaisse_Click(object sender, RoutedEventArgs e)
        {
            var caisse = new GestionDeCaisse(this);
            //caisse.viewModel.GetHome = this;
            caisse.ShowDialog();
        }

        private void WatchDriveAdding()
        {
            ManagementEventWatcher watcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType=2");
            watcher.EventArrived += new EventArrivedEventHandler(watcher_EventArrived);
            watcher.Query = query;
            watcher.Start();
            //watcher.WaitForNextEvent();
        }

        private void WatchDriveRemoving()
        {
            ManagementEventWatcher watcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType=3");
            watcher.EventArrived += new EventArrivedEventHandler(watcher_EventArrived_Removed);
            watcher.Query = query;
            watcher.Start();
            //watcher.WaitForNextEvent();
        }

        private BackupInfo SetBackupInfo()
        {
            var lst = DbManager.GetAll<BackupInfo>();
            if (lst.Count() == 0)
                return null;
            var backup = lst.Last();

            return backup;
        }

        private void watcher_EventArrived_Removed(object sender, EventArrivedEventArgs e)
        {
            var r = e.NewEvent.Properties["DriveName"].Value.ToString();
            var b = SetBackupInfo();
            if (b == null)
                return;
            if (!Directory.Exists(b.Dir))
            {
                var drive = b.Dir.Split(':')[0];
                if ($"{drive}:" == r)
                {
                    MessageBox.Show(Properties.Resources.DriveRemoved, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            
            
        }

        private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var r = e.NewEvent.Properties["DriveName"].Value.ToString();
            var b = SetBackupInfo();
            if (b == null)
                return;
            if (Directory.Exists(b.Dir))
            {
                var drive = b.Dir.Split(':')[0];
                if ($"{drive}:" == r)
                {
                    MessageBox.Show(Properties.Resources.DriveAdded, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }            
        }

        private void NetWorkChange()
        {
            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
        }

        private void NetworkChange_NetworkAvailabilityChanged(object sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
        {
            if (InfoChecker.IsConnected())
            {

            }
        }

        private void Btninfo_Click(object sender, RoutedEventArgs e)
        {
            About about = new About(this);
            about.ShowDialog();
        }

        private void CreateTable()
        {
            //create table if not exist
            string query = "CREATE TABLE IF NOT EXISTS ShopLogo (" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "Name TEXT NOT NULL," +
                    "CreationDate TEXT NOT NULL)";
            DbManager.CreateNewTable(query);

        }

        private void CreateTableSettings()
        {
            //create table if not exist
            string query = "CREATE TABLE IF NOT EXISTS Settings (" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "Name TEXT NOT NULL UNIQUE," +
                    "Data TEXT NOT NULL," +
                    "CreationDate TEXT NOT NULL)";
            DbManager.CreateNewTable(query);

        }

        private void CreateTableAppUserInfo()
        {
            //create table if not exist
            string query = "CREATE TABLE IF NOT EXISTS AppUserInfo (" +
                    "Id TEXT PRIMARY KEY," +
                    "Nom TEXT," +
                    "Contact TEXT," +
                    "Email TEXT)";
            DbManager.CreateNewTable(query);

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
        private void CreateTableBackupServer()
        {
            //create table if not exist
            string query = "CREATE TABLE IF NOT EXISTS BackupInfoServer (" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "Dir TEXT," +
                    "Date TEXT," +
                    "LastBackupDate TEXT," +
                    "AppId TEXT,"+
                    "ServerId TEXT)";
            DbManager.CreateNewTable(query);
        }

        public void ManagerTable(string columnName)
        {
            //check if the column already exist the table
            if (!DbManager.CustumQueryCheckColumn<LicenceInformation>(columnName))
            {
                // the column is not in the table
                // so we have to create it
                DbManager.CreateNewColumn<LicenceInformation>(columnName, "TEXT", "Periodic");
            }

        }
        public void ManagerTable(string columnName, string typ, string defaultvalue)
        {
            //check if the column already exist the table
            if (!DbManager.CustumQueryCheckColumn<EasyLib.Produit>(columnName))
            {
                // the column is not in the table
                // so we have to create it
                DbManager.CreateNewColumn<EasyLib.Produit>(columnName, typ, defaultvalue);
            }

        }
        public void ManagerClientTable(string columnName, string typ, string defaultvalue)
        {
            //check if the column already exist the table
            if (!DbManager.CustumQueryCheckColumn<EasyLib.Client>(columnName))
            {
                // the column is not in the table
                // so we have to create it
                DbManager.CreateNewColumn<EasyLib.Client>(columnName, typ, defaultvalue);
            }

        }

        public void ManagerCompanyTable(string columnName, string typ, string defaultvalue)
        {
            //check if the column already exist the table
            if (!DbManager.CustumQueryCheckColumn<EasyLib.CompanyInfo>(columnName))
            {
                // the column is not in the table
                // so we have to create it
                DbManager.CreateNewColumn<EasyLib.CompanyInfo>(columnName, typ, defaultvalue);
            }

        }

        private void btncanceledsell_Click(object sender, RoutedEventArgs e)
        {
            AnnulationVente annulation = new AnnulationVente();
            annulation.ShowDialog();

        }
    }

    
}
