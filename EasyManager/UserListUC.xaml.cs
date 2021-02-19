using EasyManagerDb;
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
    /// Interaction logic for UserListUC.xaml
    /// </summary>
    public partial class UserListUC : UserControl
    {
        public List<UserTile> GetUserTiles { get; set; } = new List<UserTile>();
        public Home GetHome { get; set; }
        public UserListUC()
        {
            InitializeComponent();
        }
        public UserListUC(Home home)
        {
            InitializeComponent();
            GetHome = home;
        }

        public void Load()
        {
            GetUserTiles.Clear();
            UserList.Children.Clear();
            GetUserTiles = Usertuiles();
            foreach (var item in GetUserTiles)
            {
                UserList.Children.Add(item);
            }
        }

        private List<UserTile> Usertuiles()
        {
            //Liste des tuiles
            List<UserTile> userTiles = new List<UserTile>();
            //Liste des utilisateur
            List<EasyManagerLibrary.Utilisateur> ListeUtilisateur = DbManager.GetAll<EasyManagerLibrary.Utilisateur>();
            //Parcours la liste des utilisateurs
            foreach (var item in ListeUtilisateur)
            {
                if (item.Deleted == false)
                {
                    UserTile userTile = new UserTile(this);
                    userTile.GetHome = GetHome;
                    userTile.User = item;
                    userTiles.Add(userTile);
                }
            }
            return userTiles;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.ModuleListeUtilisateur;
                GetHome.MainTitle.Text = Properties.Resources.ModuleListeUtilisateur;
            }
        }

        private void SetData(List<UserTile> lst)
        {
            UserList.Children.Clear();
            foreach (var item in lst)
            {
                UserList.Children.Add(item);
            }
        }

        private List<UserTile> Search(string critere)
        {
            var rslt = from disc in GetUserTiles where disc.User.Nom.Contains(critere) || disc.User.Prenom.Contains(critere) || disc.User.RoleLibelle.Contains(critere) || disc.User.Id.ToString().Contains(critere) select disc;
            return rslt.ToList();
        }

        private void Research()
        {
            if (string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                ResetList();
                return;
            }
            var rslt = Search(TxtSearch.Text);
            if (rslt == null)
                ResetList();
            if (rslt.Count > 0)
            {
                GetUserTiles.Clear();
                GetUserTiles = rslt;
                //rslt.Clear();
                SetData(GetUserTiles);
            }
            else
            {
                //ResetList();
            }
        }

        private void ResetList()
        {
            GetUserTiles = Usertuiles();
            SetData(GetUserTiles);
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            Research();
        }
    }
}
