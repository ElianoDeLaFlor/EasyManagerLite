using EasyManagerLibrary;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace EasyManager.MenuItems
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Vararible declaration
        public MainWindowViewModel() { }
        public List<MenuContent> MenuContents { get; set; } = new List<MenuContent>();
        public List<MenuContent> MenuContentsLst { get; set; } = new List<MenuContent>();
        private int _discountId { get; set; }
        private MenuContent _menuContent;
        public Home GetHome { get; set; }
        public DiscountUC Discount;
        private int _menuIndex = 1;
        private Visibility _isConnected;
        private Visibility _showtext = Visibility.Hidden;
        private Visibility _progress = Visibility.Hidden;
        private int _notificationcount = 0;
        private Utilisateur _Utilisateur { get; set; }        
        #endregion


        public MainWindowViewModel(ISnackbarMessageQueue snackbarMessageQueue, Home h)
        {
            GetHome = h;
            //Discount = new DiscountUC(GetHome, DiscountId);
            if (snackbarMessageQueue == null) throw new ArgumentNullException(nameof(snackbarMessageQueue));
            SetMenu();
            foreach (var item in MenuContentsLst)
            {
                if (!item.Visible)
                    continue;
                else
                    MenuContents.Add(item);
            }

        }

        private void SetMenu()
        {
            MenuContentsLst = new List<MenuContent>()
            {
                new MenuContent("ViewDashboardVariant",Properties.Resources.MenuHome,Properties.Resources.MenuHome,new Accueil(GetHome)),
                new MenuContent("UserCircle", Properties.Resources.MenuConnexion,Properties.Resources.MenuConnexion,new ConnexionUC(GetHome)),
            };
        }

        public int MenuIndex
        {
            get=>_menuIndex;
            set
            {
                if (_menuIndex == value)
                    return;
                _menuIndex = value;

                OnPropertyChanged("MenuIndex");
            }
        }

        public MenuContent MenuContent
        {
            get { return _menuContent; }
            set
            {
                _menuContent = value;
                MenuContents.Add(_menuContent);
                MenuContentsLst.Add(_menuContent);
                OnPropertyChanged("MenuContent");
            }
        }

        public int DiscountId
        {
            get { return _discountId; }
            set { _discountId = value; OnPropertyChanged("DiscountId"); }
        }

        public Utilisateur ConnectedUser
        {
            get => _Utilisateur;
            set
            {
                _Utilisateur = value;
                InfoChecker.ConnectedUser = _Utilisateur;
                OnPropertyChanged("ConnectedUser");
            }
        }

        public System.Windows.Visibility IsConnected
        {
            get => _isConnected;
            set { _isConnected = value; OnPropertyChanged("IsConnected"); }
        }

        public System.Windows.Visibility ShowText
        {
            get => _showtext;
            set { _showtext = value; OnPropertyChanged("ShowText"); }
        }

        public int NotificationCount
        {
            get { return _notificationcount; }
            set
            {
                _notificationcount = value;
                OnPropertyChanged("NotificationCount");
            }
        }

        public System.Windows.Visibility Progress
        {
            get => _progress;
            set { _progress = value; OnPropertyChanged("Progress"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
