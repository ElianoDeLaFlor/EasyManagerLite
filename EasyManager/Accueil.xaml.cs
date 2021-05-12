using APIManagerLibrary.APIResponse;
using EasyManagerDb;
using EasyManagerLibrary;
using Notifications.Wpf;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EasyLib = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for Accueil.xaml
    /// </summary>
    public partial class Accueil : UserControl
    {
        #region Variable declaration
        public List<string> NombreJour { get; set; }
        private List<string> lstprod { get; set; }
        private List<string> lstjour { get; set; }
        private int ProdIndex { get; set; } = 0;
        private int JourIndex { get; set; } = 0;
        private Home GetHome { get; set; }
        private bool CheckForUpdate { get; set; } = true;
        public List<ChartDataPie> DataList { get; set; } = new List<ChartDataPie>();
        public EasyLib.Notifications GetNotifications { get; set; } = new EasyLib.Notifications();
        public bool ThereIsUpdate { get; set; }

        #endregion
        public Accueil()
        {
            InitializeComponent();
        }
        public Accueil(Home h)
        {
            InitializeComponent();
            GetHome = h;
            GetHome.SizeChanged += GetHome_SizeChanged;            
        }

        private void GetHome_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            scroll.Width = Math.Abs(GetHome.ActualWidth-60);
        }

        /// <summary>
        /// Génère la carte de statu de catégorie
        /// </summary>
        private List<StockStateUC> CatStatus()
        {
            //Liste des tules
            List<StockStateUC> stockStates = new List<StockStateUC>();
            //Liste des catégories
            List<EasyLib.Categorie> CatList = DbManager.GetAll<EasyLib.Categorie>();
            int count = 0;
            DataList.Clear();
            //Parcours la liste des catégories
            foreach (var item in CatList)
            {
                //Liste des produits par catégorie
                var lstprod = DbManager.GetByColumnName<EasyLib.Produit>("CategorieId", item.Id.ToString());
                double qty = 0;
                double rst = 0;
                double vendu;
                double progress;
                //Parcours la liste des produits
                foreach (var prod in lstprod)
                {
                    //La quantité de produit par catégorie
                    qty += prod.QuantiteTotale;
                    //La quantité de produit restant par catégorie
                    rst += prod.QuantiteRestante;       
                }
                //La quantité de produit vendu par catégorie
                vendu = qty - rst;
                progress = (rst / qty) * 100;

                ChartDataPie chartDataPie = new ChartDataPie();
                chartDataPie.Titre = item.Libelle;
                chartDataPie.Valeur = vendu;


                StockStateUC stockState = new StockStateUC();
                stockState.TotalItem = qty;
                stockState.SellItem = vendu;
                stockState.LeftItem = rst;
                stockState.ProgressValue = Math.Floor(progress);
                stockState.Categorie = item;
                stockState.Colors = count % 2 == 0 ? "DeepSkyBlue" : "CadetBlue";
                stockStates.Add(stockState);
                DataList.Add(chartDataPie);
                count++;
                Properties.Settings settings = new Properties.Settings();
                
            }
            return stockStates;
        }

        private void Prod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var box = sender as ComboBox;
            ProdIndex = box.SelectedIndex;
            if (ProdIndex >= 0)
            {
                //Mise à jour du textblock info
                LblInfo.Text = $"{Properties.Resources.LblInfoOne} {lstprod[ProdIndex]} {Properties.Resources.LblInfoTwo} {lstjour[JourIndex]} {Properties.Resources.LblInfoThree}";
                PieChart.Datas = ChartData(int.Parse(lstjour[JourIndex]), GetProductIdByName(lstprod[ProdIndex]));
            }
        }

        private void NbrJour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var box = sender as ComboBox;
                JourIndex = box.SelectedIndex;
                if (JourIndex >= 0)
                {
                    //Mise à jour du textblock info
                    LblInfo.Text = $"{Properties.Resources.LblInfoOne} {lstprod[ProdIndex]} {Properties.Resources.LblInfoTwo} {lstjour[JourIndex]} {Properties.Resources.LblInfoThree}";

                    //PieChart.Datas = testData(int.Parse(lstjour[JourIndex]));
                    PieChart.Datas = ChartData(int.Parse(lstjour[JourIndex]), GetProductIdByName(lstprod[ProdIndex]));
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                //MessageBox.Show(Properties.Resources.EmptyDataBase, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                //GetHome.MenuItemsListBox.SelectedIndex = 6;
            }
        }

        private int GetProductIdByName(string name)
        {
            return DbManager.GetProduitByName(name).Id;
        }

        private List<string> ProdList()
        {
            var lst = DbManager.GetAll<EasyManagerLibrary.Produit>();
            var strlist = new List<string>();
            foreach (var item in lst)
            {
                strlist.Add(item.Nom);
            }
            return strlist;
        }

        public List<ChartDataPie> testData(int k)
        {
            var Datas = new List<ChartDataPie>();
            for (int i = 0; i < k; i++)
            {
                var d = new ChartDataPie();
                d.Titre = $"Jour{i + 1}";
                d.Valeur = i + 3;
                d.LblDate = true;
                Datas.Add(d);
            }
            return Datas;
        }

        private ChartDataPie PieChartDateData(double qty, int jour, DateTime dt)
        {
            var d = new ChartDataPie();
            d.Titre = $"Jour {jour} -\t {InfoChecker.AjustDateWithDMY(dt)}";
            d.Valeur = qty;
            d.LblDate = true;

            return d;
        }

        private ChartDataPie PieChartStockData(double qty, string label)
        {
            var d = new ChartDataPie();
            d.Titre = label;
            d.Valeur = qty;
            d.LblDate = true;

            return d;
        }

        private List<ChartDataPie> ChartData(int nbrjour, int prodid)
        {
            int DayCount = 1;
            var lstpiedata = new List<ChartDataPie>();
            //Les jour à prendre en comptes
            var lstDates = InfoChecker.LasteDates(nbrjour);

            foreach (var item in lstDates)
            {
                var lstvente = ListVente(item);
                var QTY = ProduitVendusDateQTY(lstvente, prodid);
                if (QTY == 0)
                {
                    DayCount++;
                    continue;
                }
                lstpiedata.Add(PieChartDateData(QTY, DayCount, item));
                DayCount++;
            }

            return lstpiedata;
        }

        private List<ChartDataPie> ChartDataStock(int prodid)
        {
            var lstpiedata = new List<ChartDataPie>();
            var prod = DbManager.GetById<EasyLib.Produit>(prodid);
            for (int i = 0; i < 2; i++)
            {
                if(i==0)
                {
                    //Selled
                    var QTY = prod.QuantiteTotale - prod.QuantiteRestante;
                    if (QTY == 0)
                    {
                        continue;
                    }
                    lstpiedata.Add(PieChartStockData(QTY, Properties.Resources.SellQuantity));
                }
                else
                {
                    //Left
                    var QTY = prod.QuantiteRestante;
                    if (QTY == 0)
                    {
                        continue;
                    }
                    lstpiedata.Add(PieChartStockData(QTY, Properties.Resources.LeftQuantity));
                }
                
            }
            return lstpiedata;
        }

        /// <summary>
        /// Récupère les ventes à une date donnée x
        /// </summary>
        /// <param name="dateTime">Date x</param>
        /// <returns>Liste des ventes</returns>
        private List<EasyLib.Vente> ListVente(DateTime dateTime)
        {
            return DbManager.GetVenteByDate(InfoChecker.AjustDate(dateTime));
        }

        /// <summary>
        /// Récupère la quantité total du produit vendu à une date x
        /// </summary>
        /// <param name="ventes">Liste des ventes</param>
        /// <param name="ProdId">Id du produit vendu</param>
        /// <returns></returns>
        private double ProduitVendusDateQTY(List<EasyLib.Vente> ventes, int ProdId)
        {
            var lstProdVendu = new List<ProduitVendu>();
            double total = 0;
            foreach (var item in ventes)
            {
                lstProdVendu = DbManager.GetProduiVenduByProdIdVenteId(ProdId, item.Id);
                if (lstProdVendu == null)
                {
                    total += 0;
                    continue;
                }
                else
                {
                    foreach (var vendu in lstProdVendu)
                    {
                        total += vendu.Quantite;
                    }
                }

            }
            return total;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.MenuHome;
                GetHome.MainTitle.Text = Properties.Resources.MenuHome;
            }
            CatStatusPnl.Children.Clear();
            foreach (var item in CatStatus())
            {
                CatStatusPnl.Children.Add(item);
            }
            ChartColumn.Datas = null;
            ChartColumn.Datas = DataList;
            lstprod = ProdList();
            lstjour = new List<string> { "3", "4", "5", "7", "8", "10", "15", "20", "30" };
            NombreJour = lstjour;
            Prod.ItemsSource = lstprod;
            ProdName.ItemsSource = lstprod;
            if(lstprod!=null && lstprod.Count > 0)
            {
                ProdName.SelectedIndex = 0;
                ProdIndex = 0;
                StockChart.Datas = null;
                StockChart.Datas = ChartDataStock(GetProductIdByName(lstprod[ProdIndex]));
            }
            //LblInfo.Text = $"{Properties.Resources.LblInfoOne} {lstprod[0]} {Properties.Resources.LblInfoTwo} {lstjour[0]} {Properties.Resources.LblInfoThree}";
            NbrJour.ItemsSource = NombreJour;

            if (CheckForUpdate)
                AutoCheckUpdate();
        }

        private void ProdName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var box = sender as ComboBox;
            ProdIndex = box.SelectedIndex;
            if(ProdIndex>=0)
            {
                StockChart.Datas = null;
                StockChart.Datas = ChartDataStock(GetProductIdByName(lstprod[ProdIndex]));
            }
                
        }

        private void SaveNotification()
        {
            var notif = DbManager.GetAll<EasyLib.Notifications>();

            if (notif != null && notif.Count > 0)
            {
                var updatenotif = DbManager.GetNotificationByType(GetNotifications.ProduitNom, false);
                if (updatenotif != null)
                {
                    //update
                    var NotifToUpdate = updatenotif;
                    //Produit
                    if (ThereIsUpdate)
                    {
                        NotifToUpdate.Message = Properties.Resources.UpdateNotification;
                        NotifToUpdate.Couleur = "Yellowgreen";
                    }
                    else
                    {
                        //NotifToUpdate.Message = Properties.Resources.EmptyStock;
                        //NotifToUpdate.Couleur = "Red";
                    }

                    NotifToUpdate.NoticationEvent += NotifToUpdate_NoticationEvent;
                    NotifToUpdate.ProduitQuantiteRestante = GetNotifications.ProduitQuantiteRestante;
                    NotifToUpdate.Date = GetNotifications.Date;
                    DbManager.UpDate(NotifToUpdate, NotifToUpdate.Id);
                }
                else
                {
                    //new record
                    GetNotifications.NoticationEvent += NotifToUpdate_NoticationEvent;
                    DbManager.Save(GetNotifications);
                    GetNotifications.Save = true;
                }
            }
            else
            {
                //new record
                GetNotifications.NoticationEvent += NotifToUpdate_NoticationEvent;
                DbManager.Save(GetNotifications);
                GetNotifications.Save = true;
            }

        }

        private void NotifToUpdate_NoticationEvent(object sender, NotificationEventArgs e)
        {
            var notificationcount = DbManager.GetAll<EasyLib.Notifications>().Count();
            GetHome.DataContextt.NotificationCount = notificationcount;
        }


        private void UpdateNotificationEvent()
        {

            NotificationContent notif = new NotificationContent();
            notif.Title = Properties.Resources.MainTitle;

            NotificationManager notifmanag = new NotificationManager();

            EasyLib.Notifications notifications = new EasyLib.Notifications();
            notifications.Date = DateTime.Now;

            notifications.Couleur = "Yellowgreen";
            notifications.Message = Properties.Resources.UpdateNotification;

            notifications.IsApprovisionnement = false;
            notifications.ProduitNom = Properties.Resources.Update;
            notifications.ProduitQuantiteRestante = 0;

            GetNotifications = notifications;

            CheckForUpdate = false;

            notif.Message = $"{Properties.Resources.Update}{Environment.NewLine}{Properties.Resources.UpdateNotification}{Environment.NewLine}{Environment.NewLine}{DateTime.Now}";
            notif.Type = NotificationType.Information;
            var Duration = new TimeSpan(0, 0, 10);
            notifmanag.Show(notif, "", Duration, null, new Action(SaveNotification));
        }

        private async void AutoCheckUpdate()
        {
            if (InfoChecker.IsConnected())
            {
                var key = DbManager.GetByColumnName<Settings>("Name", "AppKey").FirstOrDefault().Data;
                if (await CheckUpdate(key))
                {
                    UpdateNotificationEvent();
                }
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
    }
}
