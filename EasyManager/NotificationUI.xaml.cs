using EasyManagerDb;
using EasyManagerLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for NotificationUI.xaml
    /// </summary>
    public partial class NotificationUI : Window, INotifyPropertyChanged
    {
        public List<NotificationUC> GetNotifications { get; set; } = new List<NotificationUC>();
        private Visibility _showemptyinfo=Visibility.Collapsed;

        public NotificationUI()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Visibility ShowEmptyInfo
        {
            get { return _showemptyinfo; }
            set { _showemptyinfo = value; OnPropertyChanged("ShowEmptyInfo"); }
        }
        private List<NotificationUC> NotificationUCs()
        {
            //Liste des tuiles
            List<NotificationUC> notifications = new List<NotificationUC>();
            //Liste des Notifications
            List<EasyManagerLibrary.Notifications> NotifList = DbManager.GetAll<EasyManagerLibrary.Notifications>("Date");
            //Parcours la liste des Notifications
            foreach (var item in NotifList)
            {
                NotificationUC NotifUC = new NotificationUC();
                NotifUC.ProductName = item.ProduitNom;
                NotifUC.Messages = item.Message;
                NotifUC.LeftQuanity = item.ProduitQuantiteRestante;
                NotifUC.Date = item.Date;
                NotifUC.StringDate = InfoChecker.AjustDateWithTimeDMYT(item.Date);
                NotifUC.NotificationId = item.Id;
                NotifUC.NotificationUI = this;
                NotifUC.Colors = item.Couleur;
                if (item.ProduitNom == Properties.Resources.Update || item.ProduitNom=="Mise à jour" || item.ProduitNom=="Update")
                    NotifUC.stkleft.Visibility = Visibility.Hidden;
                else
                    NotifUC.stkleft.Visibility = Visibility.Visible;
                notifications.Add(NotifUC);
            }
            return notifications;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        public void Reload()
        {
            NotifList.Children.Clear();
            GetNotifications = NotificationUCs();
            if (GetNotifications.Count == 0)
            {
                ShowEmptyInfo = Visibility.Visible;
            }
            else
            {
                ShowEmptyInfo = Visibility.Collapsed;
                foreach (var item in GetNotifications)
                {
                    NotifList.Children.Add(item);
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = sender as NotificationUI;
            var ah = window.ActualHeight;
            lstScroll.MaxHeight = ah - ah / 10;
        }
    }
}
