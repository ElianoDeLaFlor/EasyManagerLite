using EasyManagerDb;
using EasyManagerLibrary;
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
    /// Interaction logic for RoleTile.xaml
    /// </summary>
    public partial class RoleTile : UserControl
    {
        public EasyManagerLibrary.Role Role { get; set; }
        public List<LibelleModule> LibelleModules { get; set; }
        public Home GetHome { get; set; }
        public RoleListUC RoleList { get; set; }
        public RoleTile(Home home,RoleListUC rl)
        {
            InitializeComponent();
            GetHome = home;
            Datagrid.Width = GetHome.ActualWidth - 75;
            GetHome.SizeChanged += GetHome_SizeChanged;
            RoleList = rl;
            DataContext = this;
        }

        private void GetHome_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Datagrid.Width = GetHome.ActualWidth - 75;
        }

        public RoleTile()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void btnedit_Click(object sender, RoutedEventArgs e)
        {
            Parametre parametre = new Parametre(GetHome,true,Role.Libelle);
            parametre.ShowDialog();
        }

        private void Btndelete_Click(object sender, RoutedEventArgs e)
        {
            if (Utilisateurs(Role).Count > 0)
            {
                //a user has this role
                //it can't be deleted
                MessageBox.Show(Properties.Resources.RoleCanNotBeDeleted, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Delete(Role))
            {
                MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                RoleList.Load();
            }
            else
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private bool Delete(Role role)
        {   
                // no one has this role
                // it can be deleted
                bool deleteRslt = DbManager.DeleteRoleModuleByRole(role.Id);
                if (deleteRslt)
                    return DbManager.Delete<Role>(Role.Id);
                return deleteRslt;
            
        }

        private List<Utilisateur> Utilisateurs(Role role)
        {
            return DbManager.GetByColumnName<Utilisateur>("RoleLibelle", role.Libelle);
        }
    }

}
