using EasyManager.MenuItems;
using EasyManagerDb;
using EasyManagerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for GestionDeCaisse.xaml
    /// </summary>
    public partial class GestionDeCaisse : Window, IAccessControl
    {
        public GestionCaisseViewModel viewModel;
        private bool IsClicked { get; set; } = true;
        public Home GetHome { get; set; }
        public Module GetModule { get => CaisseModule(); set { } }

        public GestionDeCaisse()
        {
            InitializeComponent();
            viewModel = new GestionCaisseViewModel(this);
            DataContext = viewModel;

        }

        public GestionDeCaisse(Home home)
        {
            InitializeComponent();
            viewModel = new GestionCaisseViewModel(this);
            GetHome = home;
            viewModel.GetHome = home;
            DataContext = viewModel;

        }

        private void Btnshowtext_Click(object sender, RoutedEventArgs e)
        {
            if (IsClicked)
            {
                //clicked => hide text

                var lstmenu = viewModel.MenuContents;
                foreach (var item in lstmenu)
                {
                    item.ShowText = Visibility.Visible;
                }
                viewModel.MenuContents = lstmenu;
                IsClicked = false;
            }
            else
            {
                //uncliked => show text
                var lstmenu = viewModel.MenuContents;
                foreach (var item in lstmenu)
                {
                    item.ShowText = Visibility.Collapsed;
                }
                viewModel.MenuContents = lstmenu;
                IsClicked = true;
            }
        }

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

        public bool CanAccess(Utilisateur utilisateur)
        {
            //UserRole
            var userrole = DbManager.GetByColumnName<Role>("Libelle", utilisateur.RoleLibelle)[0];
            //Role module
            var rolemodule = DbManager.GetByColumnName<RoleModule>("RoleId", userrole.Id.ToString());

            List<int> usermoduleid = new List<int>();
            foreach (var item in rolemodule)
                usermoduleid.Add(item.ModuleId);

            return usermoduleid.Contains(GetModule.Id);
        }

        private Module CaisseModule()
        {
            return DbManager.GetByColumnName<Module>("Libelle", Properties.Resources.ModuleCaisse)[0];
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CanAccess(GetHome.DataContextt.ConnectedUser)) return;
            IsEnabled = false;
            MessageBox.Show(Properties.Resources.CanAccess, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
