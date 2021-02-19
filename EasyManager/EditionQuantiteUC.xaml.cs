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

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for EditionQuantiteUC.xaml
    /// </summary>
    public partial class EditionQuantiteUC : UserControl
    {
        public int ProdId { get; set; }
        public double NewQuantite { get; set; } = 0;
        public decimal UnitPrice { get; set; } = 0;
        public QuantiteEdition Quantite { get; set; }
        public Home GetHome { get; set; }
        public EasyManagerLibrary.Notifications GetNotifications { get; set; } = new EasyManagerLibrary.Notifications();

        public EditionQuantiteUC()
        {
            InitializeComponent();
        }

        public EditionQuantiteUC(Home h)
        {
            InitializeComponent();
            GetHome = h;
        }

        private void FilledDropDown()
        {
            CbCatList.ItemsSource = ProdList();
        }
        private void CbCatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems[0].ToString();

            var cat = DbManager.GetProduitByName(item);
            ProdId = cat.Id;

        }
        private List<string> ProdList()
        {
            var lst = DbManager.GetAll<EasyManagerLibrary.Produit>();

            List<string> lstprod = new List<string>();
            foreach (var item in lst)
            {
                lstprod.Add(item.Nom);
            }
            return lstprod;
        }

        private bool CheckFields()
        {
            string quantite = txtqantite.Text;
            string prixunitaire = txtprixunitaire.Text;

            if (InfoChecker.IsEmpty(quantite))
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (InfoChecker.IsEmpty(prixunitaire))
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (ProdId == 0)
            {
                MessageBox.Show(Properties.Resources.CatSelectionError, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            NewQuantite = double.Parse(quantite);
            UnitPrice = decimal.Parse(prixunitaire);

            Quantite = new QuantiteEdition();
            Quantite.ProduitId = ProdId;
            Quantite.UtilisateurId = GetHome.DataContextt.ConnectedUser.Id; // Connected user id
            Quantite.Quantite = NewQuantite;
            Quantite.PrixUnitaire = UnitPrice;
            Quantite.DateEdition = DateTime.UtcNow;

            return true;

        }

        private void txtqantite_KeyUp(object sender, KeyEventArgs e)
        {
            OnlyNumber(sender as TextBox);
        }

        private void txtprixunitaire_KeyUp(object sender, KeyEventArgs e)
        {
            OnlyNumber(sender as TextBox);
        }

        private void btncreate_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
                return;
            if (DbManager.Save(Quantite))
            {
                MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateProduct(NewQuantite);
                Reset();
            }
            else
            {
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool UpdateProduct(double addquantite)
        {
            var oldprod = DbManager.GetProduitById(ProdId);
            oldprod.AlertQuantityEvent += Oldprod_AlertQuantityEvent;
            double newquantitetotal = oldprod.QuantiteTotale + addquantite;
            double newquantiterestante = oldprod.QuantiteRestante + addquantite;
            // Update
            oldprod.QuantiteTotale = newquantitetotal;
            oldprod.QuantiteRestante = newquantiterestante;
            oldprod.PrixUnitaire = UnitPrice;
            return DbManager.UpDate(oldprod, ProdId);
        }
        private void SaveNotification()
        {
            //var NotifToUpdate = DbManager.GetNotificationByType(GetNotifications.ProduitNom, true);

            //if (NotifToUpdate != null)
            //{
            //    //update
                
            //    //produit
            //    var prod = DbManager.GetProduitByName(NotifToUpdate.ProduitNom);   
                
            //    NotifToUpdate.NoticationEvent += GetNotifications_NoticationEvent;
            //    NotifToUpdate.ProduitQuantiteRestante = prod.QuantiteRestante;
            //    DbManager.UpDate(NotifToUpdate, NotifToUpdate.Id);


            //}
            //else
            {
                //new record
                GetNotifications.NoticationEvent += GetNotifications_NoticationEvent; ;
                DbManager.Save(GetNotifications);
                GetNotifications.Save = true;
            }

        }
        private void OnlyNumber(TextBox tb)
        {
            tb.Text = InfoChecker.NumericDecOnlyRegExp(tb.Text);
            tb.SelectionStart = tb.Text.Length;
        }
        private void GetNotifications_NoticationEvent(object sender, NotificationEventArgs e)
        {
            var notificationcount = DbManager.GetAll<EasyManagerLibrary.Notifications>().Count();
            GetHome.DataContextt.NotificationCount = notificationcount;
        }

        private void Oldprod_AlertQuantityEvent(object sender, AlertQuantityEventArgs e)
        {
            NotificationContent notif = new NotificationContent();
            notif.Title = Properties.Resources.MainTitle;

            NotificationManager notifmanag = new NotificationManager();

            EasyManagerLibrary.Notifications notifications = new EasyManagerLibrary.Notifications();
            notifications.Date = DateTime.Now;

            if (e.Produit.QuantiteRestante <= e.Produit.QuantiteAlerte)
            {
                // l'ajout ne dépasse pas la quantité d'allert
                notifications.Couleur = "YellowGreen";
            }
            else
            {
                // l'ajout dépasse pas la quantité d'allert
                notifications.Couleur = "Green";
            }

            notifications.Message = Properties.Resources.ApprovisionnementNotif;
            notifications.IsApprovisionnement = true;
            notifications.ProduitNom = e.Produit.Nom;
            notifications.ProduitQuantiteRestante = e.Produit.QuantiteRestante;

            GetNotifications = notifications;


            notif.Message = $"{e.Produit.Nom}{Environment.NewLine}{Properties.Resources.ApprovisionnementNotif}{Environment.NewLine}{Properties.Resources.LeftQuantity}:{e.Produit.QuantiteRestante}{Environment.NewLine}{DateTime.Now}";
            notif.Type = NotificationType.Success;
            var Duration = new TimeSpan(0, 0, 10);
            notifmanag.Show(notif, "", Duration, null, new Action(SaveNotification));
        }

        private void Reset()
        {
            txtprixunitaire.Text = string.Empty;
            txtqantite.Text = string.Empty;
            CbCatList.SelectedIndex = 0;
        }

        private void btncancel_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.EditionQuantite;
                GetHome.MainTitle.Text = Properties.Resources.EditionQuantite;
            }
            FilledDropDown();
        }
    }
}
