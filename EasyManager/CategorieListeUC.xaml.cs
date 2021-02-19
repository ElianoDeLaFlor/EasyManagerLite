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
    /// Interaction logic for CategorieListeUC.xaml
    /// </summary>
    public partial class CategorieListeUC : UserControl
    {
        public List<CategorieTuile> GetCategorieTuiles { get; set; } = new List<CategorieTuile>();
        public Home GetHome { get; set; }
        public CategorieListeUC()
        {
            InitializeComponent();
        }
        public CategorieListeUC(Home home)
        {
            InitializeComponent();
            GetHome = home;
        }

        private List<CategorieTuile> CategorieTuiles()
        {
            //Liste des tuiles
            List<CategorieTuile> categoriestuiple = new List<CategorieTuile>();
            //Liste des Catégories
            List<EasyManagerLibrary.Categorie> CatList = DbManager.GetAll<EasyManagerLibrary.Categorie>();
            //Parcours la liste des Categories
            foreach (var item in CatList)
            {
                CategorieTuile CatTuile = new CategorieTuile(this);
                CatTuile.GetCategorie = item;
                categoriestuiple.Add(CatTuile);
            }
            return categoriestuiple;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.ModuleListeCategorie;
                GetHome.MainTitle.Text = Properties.Resources.ModuleListeCategorie;
            }
        }

        public void Load()
        {
            GetCategorieTuiles.Clear();
            CategorieList.Children.Clear();
            GetCategorieTuiles = CategorieTuiles();
            foreach (var item in GetCategorieTuiles)
            {
                CategorieList.Children.Add(item);
            }
        }

        private List<CategorieTuile> Search(string critere)
        {
            var rslt = from disc in GetCategorieTuiles where disc.GetCategorie.Libelle.Contains(critere) || disc.GetCategorie.Description.Contains(critere) select disc;
            return rslt.ToList();
        }

        private void ResetList()
        {
            GetCategorieTuiles = CategorieTuiles();
            SetData(GetCategorieTuiles);
        }

        private void SetData(List<CategorieTuile> lst)
        {
            CategorieList.Children.Clear();
            foreach (var item in lst)
            {
                CategorieList.Children.Add(item);
            }
        }

        private void Research()
        {
            if(string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                ResetList();
                return;
            }
            var rslt = Search(TxtSearch.Text);
            if (rslt == null)
                ResetList();
            if (rslt.Count > 0)
            {
                GetCategorieTuiles.Clear();
                GetCategorieTuiles = rslt;
                //rslt.Clear();
                SetData(GetCategorieTuiles);
            }
            else
            {
                //ResetList();
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            Research();
        }
    }
}
