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
using ClassL = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for OperationCaisseListeUC.xaml
    /// </summary>
    public partial class OperationCaisseListeUC : UserControl
    {
        public List<OperationCaisseTuile> GetOperationCaisseTuiles { get; set; } = new List<OperationCaisseTuile>();
        public GestionDeCaisse GetCaisseHome { get; set; }
        public List<OperationCaisse> OperationCaisses { get; set; }
        public string SaveLocation { get; set; }
        public ShowDocument showDocument { get; set; }
        public OperationCaisseListeUC()
        {
            InitializeComponent();
        }
        public OperationCaisseListeUC(GestionDeCaisse CaisseHome)
        {
            InitializeComponent();
            GetCaisseHome = CaisseHome;
        }

        private List<OperationCaisseTuile> OperationCaisseTuiles()
        {
            //Liste des tuiles
            List<OperationCaisseTuile> operationcaissestuiple = new List<OperationCaisseTuile>();
            //Liste des Operations de caisse
            List<OperationCaisse> CatList = DbManager.GetAll<OperationCaisse>();
            OperationCaisses = new List<OperationCaisse>();
            //Parcours la liste des Operations de caisse
            foreach (var item in CatList)
            {
                item.SetOperation(GetOperation(item.OperationId));
                item.SetUser(GetUtilisateur(item.UtilisateurId));
                OperationCaisses.Add(item);
                OperationCaisseTuile CatTuile = new OperationCaisseTuile(this);
                CatTuile.GetOperationCaisse = item;
                operationcaissestuiple.Add(CatTuile);
            }
            return operationcaissestuiple;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
            if (GetCaisseHome != null)
            {
                GetCaisseHome.Title = Properties.Resources.CheckOutOperations;
                GetCaisseHome.MainTitle.Text = Properties.Resources.CheckOutOperations;
            }
        }

        public void Load()
        {
            GetOperationCaisseTuiles.Clear();
            CategorieList.Children.Clear();
            GetOperationCaisseTuiles = OperationCaisseTuiles();
            foreach (var item in GetOperationCaisseTuiles)
            {
                CategorieList.Children.Add(item);
            }
        }

        private List<OperationCaisseTuile> Search(string critere)
        {
            var rslt = from disc in GetOperationCaisseTuiles where disc.GetOperationCaisse.Date.ToShortDateString().Contains(critere) || disc.GetOperationCaisse.Montant.ToString().Contains(critere) || GetOperation(disc.GetOperationCaisse.OperationId).Libelle.Contains(critere) select disc;
            return rslt.ToList();
        }

        private void ResetList()
        {
            GetOperationCaisseTuiles = OperationCaisseTuiles();
            SetData(GetOperationCaisseTuiles);
        }

        private void SetData(List<OperationCaisseTuile> lst)
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
                GetOperationCaisseTuiles.Clear();
                GetOperationCaisseTuiles = rslt;
                //rslt.Clear();
                SetData(GetOperationCaisseTuiles);
            }
            else
            {
                //ResetList();
            }
        }

        private EasyManagerLibrary.Operation GetOperation(int id)
        {
            return DbManager.GetById<EasyManagerLibrary.Operation>(id);
        }

        private Utilisateur GetUtilisateur(int id)
        {
            return DbManager.GetById<Utilisateur>(id);
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            Research();
        }

        private async void Btnprint_Click(object sender, RoutedEventArgs e)
        {
            //var watch = System.Diagnostics.Stopwatch.StartNew();
            GetCaisseHome.viewModel.Progress = Visibility.Visible;
            if (await PrintAsync(OperationCaisses))
            {
                showDocument = new ShowDocument(SaveLocation);
                GetCaisseHome.viewModel.Progress = Visibility.Hidden;
                //watch.Stop();
                //Console.WriteLine($"premier arret: {watch.ElapsedMilliseconds}");
                showDocument.ShowDialog();
            }
            else
            {
                GetCaisseHome.viewModel.Progress = Visibility.Hidden;
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool GenOperationCaisseList(List<OperationCaisse> operationCaisses)
        {
            Office office = new Office();
            office.CompanyName = "DeLaFlor Corporation";
            office.CompanyTel = "Tel:+228 99 34 12 11";
            office.CompanyEmail = "Email:delaflor@flor.com";
            office.Operation = Properties.Resources.Operation;
            office.Montant = Properties.Resources.Montant;
            office.Utilisateur = Properties.Resources.User;
            office.Date = Properties.Resources.Date;
            office.OperationCaisseListe = Properties.Resources.CheckOutOperations;
            office.GetOperationCaisses = operationCaisses;
            //office.Periode = $"{InfoChecker.AjustDateWithDMY(InfoChecker.StringToDate(ViewModel.DateDebut))} - {InfoChecker.AjustDateWithDMY(InfoChecker.StringToDate(ViewModel.DateFin))}";

            SaveLocation = InfoChecker.SaveLoc(Properties.Resources.CheckoutOperationPrint, Properties.Resources.CheckoutOperationPrint);

            return office.GenListeOperationCaisse(System.IO.Path.GetFullPath("Files\\ListeOperationCaisse_Prototype.dotx"), SaveLocation);
        }

        private bool Print(List<ClassL.OperationCaisse> lst)
        {
            return GenOperationCaisseList(lst);
        }

        private async Task<bool> PrintAsync(List<OperationCaisse> lst)
        {
            return await Task.Run(() => Print(lst));
        }
    }
}
