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
    /// Interaction logic for OperationListUC.xaml
    /// </summary>
    public partial class OperationListUC : UserControl
    {
        public List<OperationTuile> GetOperationTuiles { get; set; } = new List<OperationTuile>();
        public GestionDeCaisse GetCaisseHome { get; set; }
        public OperationListUC()
        {
            InitializeComponent();
        }
        public OperationListUC(GestionDeCaisse CaisseHome)
        {
            InitializeComponent();
            GetCaisseHome = CaisseHome;
        }

        private List<OperationTuile> CategorieTuiles()
        {
            //Liste des tuiles
            List<OperationTuile> categoriestuiple = new List<OperationTuile>();
            //Liste des Catégories
            List<EasyManagerLibrary.Operation> CatList = DbManager.GetAll<EasyManagerLibrary.Operation>();
            //Parcours la liste des Categories
            foreach (var item in CatList)
            {
                OperationTuile CatTuile = new OperationTuile(this);
                CatTuile.GetOperation = item;
                categoriestuiple.Add(CatTuile);
            }
            return categoriestuiple;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
            if (GetCaisseHome != null)
            {
                GetCaisseHome.Title = Properties.Resources.Operations;
                GetCaisseHome.MainTitle.Text = Properties.Resources.Operations;
            }
        }

        public void Load()
        {
            GetOperationTuiles.Clear();
            CategorieList.Children.Clear();
            GetOperationTuiles = CategorieTuiles();
            foreach (var item in GetOperationTuiles)
            {
                CategorieList.Children.Add(item);
            }
        }

        private List<OperationTuile> Search(string critere)
        {
            var rslt = from disc in GetOperationTuiles where disc.GetOperation.Libelle.Contains(critere) || disc.GetOperation.TypeOperation.ToString().Contains(critere) select disc;
            return rslt.ToList();
        }

        private void ResetList()
        {
            GetOperationTuiles = CategorieTuiles();
            SetData(GetOperationTuiles);
        }

        private void SetData(List<OperationTuile> lst)
        {
            CategorieList.Children.Clear();
            foreach (var item in lst)
            {
                CategorieList.Children.Add(item);
            }
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
                GetOperationTuiles.Clear();
                GetOperationTuiles = rslt;
                //rslt.Clear();
                SetData(GetOperationTuiles);
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

        private void Btnprint_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
