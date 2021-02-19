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
    /// Interaction logic for RoleListUC.xaml
    /// </summary>
    public partial class RoleListUC : UserControl
    {
        public List<RoleTile> GetRoleTiles { get; set; } = new List<RoleTile>();
        public Home GetHome { get; set; }
        public RoleListUC()
        {
            InitializeComponent();
        }

        public RoleListUC(Home home)
        {
            InitializeComponent();
            GetHome = home;
        }

        private List<RoleTile> RoleTiles()
        {
            //Liste des tuiles
            List<RoleTile> roleTiles = new List<RoleTile>();
            //Liste des roles
            List<EasyManagerLibrary.Role> RoleList = DbManager.GetAll<EasyManagerLibrary.Role>();
            //Parcours la liste des roles
            foreach (var item in RoleList)
            {
                RoleTile roleTile = new RoleTile(GetHome,this);
                roleTile.Role = item;
                roleTile.LibelleModules = libelleModules(item.Id);
                roleTiles.Add(roleTile);
            }
            return roleTiles;
        }
        private List<EasyManagerLibrary.Module> ListModules(int roleid)
        {
            var rolemodule = DbManager.GetByColumnName<EasyManagerLibrary.RoleModule>("RoleId", roleid.ToString());
            var lstmodule = new List<EasyManagerLibrary.Module>();
            foreach (var item in rolemodule)
            {
                lstmodule.Add(DbManager.GetById<EasyManagerLibrary.Module>(item.ModuleId));
            }
            return lstmodule;
        }

        private List<LibelleModule> libelleModules(int id)
        {
            List<EasyManagerLibrary.Module> modules = ListModules(id);
            List<LibelleModule> libelleModules = new List<LibelleModule>();
            int count = 1;
            LibelleModule libelleModule = new LibelleModule();
            foreach (var item in modules)
            {

                if (count % 2 != 0)
                {
                    libelleModule.ItemOne = item.Libelle;
                }
                else
                {
                    libelleModule.ItemTwo = item.Libelle;
                }
                if (count % 2 == 0 || count == modules.Count)
                {
                    libelleModules.Add(libelleModule);
                    libelleModule = new LibelleModule();
                }
                count++;
            }

            return libelleModules;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
            if (GetHome!=null)
            {
                GetHome.Title = Properties.Resources.ModuleListeRole;
                GetHome.MainTitle.Text = Properties.Resources.ModuleListeRole;
            }
            
        }

        public void Load()
        {
            GetRoleTiles.Clear();
            RoleList.Children.Clear();

            GetRoleTiles = RoleTiles();
            foreach (var item in GetRoleTiles)
            {
                RoleList.Children.Add(item);
            }
        }
    }
}
