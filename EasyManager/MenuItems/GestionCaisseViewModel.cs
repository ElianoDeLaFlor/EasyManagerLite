using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EasyManagerDb;
using EasyManagerLibrary;
using EasyLib=EasyManagerLibrary;

namespace EasyManager.MenuItems
{
    public class GestionCaisseViewModel: INotifyPropertyChanged
    {
        private Visibility _progress = Visibility.Hidden;
        public GestionCaisseViewModel()
        {
            SetMenu();
            foreach (var item in MenuContentsLst)
            {
                MenuContents.Add(item);

            }
        }
        public GestionCaisseViewModel(GestionDeCaisse caisse)
        {
            GetCaisseHome = caisse;
            SetMenu();
            foreach (var item in MenuContentsLst)
            {
                MenuContents.Add(item);

            }
        }
        private MenuContent _menuContent;
        public Home _getHome;
        public GestionDeCaisse GetCaisseHome { get; set; }


        public List<MenuContent> MenuContents { get; set; } = new List<MenuContent>();
        public List<MenuContent> MenuContentsLst { get; set; } = new List<MenuContent>();
        

        public MenuContent MenuContent
        {
            get { return _menuContent; }
            set
            {
                _menuContent = value;
                MenuContents.Add(_menuContent);
                OnPropertyChanged("MenuContent");
                MenuContentsLst.Add(_menuContent);
            }
        }

        public Visibility Progress
        {
            get => _progress;
            set
            {
                if (_progress == value)
                    return;
                _progress = value;
                OnPropertyChanged("Progress");
            }
        }

        public Home GetHome
        {
            get => _getHome;
            set
            {
                if (_getHome == value)
                    return;
                _getHome = value;
                OnPropertyChanged("GetHome");
            }
        }

        private void SetMenu()
        {
            MenuContentsLst = new List<MenuContent>()
            {
                new MenuContent("ViewDashboardVariant",Properties.Resources.MenuHome,Properties.Resources.MenuHome,new CheckoutHome(GetCaisseHome)),
                new MenuContent("Briefcase", Properties.Resources.AddOperation,Properties.Resources.AddOperation,new OperationUC(GetCaisseHome)),
                new MenuContent("CreditCard", Properties.Resources.AddCheckOutOperation,Properties.Resources.AddCheckOutOperation,new OperationCaisseUC(GetCaisseHome)),
                new MenuContent("Buffer", Properties.Resources.Operations,Properties.Resources.Operations,new OperationListUC(GetCaisseHome)),
                new MenuContent("FormatListCheckbox", Properties.Resources.CheckOutOperations,Properties.Resources.CheckOutOperations,new OperationCaisseListeUC(GetCaisseHome)),
                new MenuContent("Cup", Properties.Resources.Checkout,Properties.Resources.Checkout,new CaisseUC(GetCaisseHome)),
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        

    }
}
