using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EasyManagerDb;
using EasyManagerLibrary;
using ClassL = EasyManagerLibrary;


namespace EasyManager
{
    /// <summary>
    /// Interaction logic for ListeProduitCommende.xaml
    /// </summary>
    public partial class ListeProduitCommende : Window, INotifyPropertyChanged
    {
        public List<ProduitCredit> ProduitCommandes { get; set; }
        public List<ProduitCredit> GetProduitCommandes { get; set; }
        public int CommandeId { get; set; }
        int RowIndex;

        public Tva Tva { get; set; } = new Tva();
        public CompanyInfo Company { get; set; } = new CompanyInfo();
        public bool IsFilter { get; set; }
        /// <summary>
        /// Indique que les produits commendés sont récupérés par CommandeId
        /// </summary>
        public bool IsUnique { get; set; }
        public string SaveLocation { get; set; }
        public ShowDocument showDocument { get; set; }
        public decimal Somme { get; set; } = 0;
        public decimal SommeTva { get; set; } = 0;
        public decimal SommeTTC { get; set; } = 0;
        public decimal SommeRestante { get; set; } = 0;
        private Visibility _Progress; //{ get { return Progress; } set { Progress = value; OnPropertyChanged("Progress"); } }
        public ListeProduitCommende()
        {
            InitializeComponent();
            GetProduitCommandes = GetProduitsCommande();
            ProduitCommandes = GetProduitsCommande();
            IsUnique = false;
            DataContext = this;
        }

        public ListeProduitCommende(int cmdid)
        {
            InitializeComponent();
            GetProduitCommandes = GetProduitsCommande(cmdid);
            ProduitCommandes = GetProduitsCommandee(cmdid);
            Progress = Visibility.Hidden;
            IsUnique = true;
            CommandeId = 2;
            DataContext = this;
        }

        

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Visibility Progress
        {
            get { return _Progress; }
            set
            {
                _Progress = value;
                OnPropertyChanged("Progress");
            }
        }

        /// <summary>
        /// Retourne tous les produits commandés dans la base de données
        /// </summary>
        /// <returns></returns>
        private List<ProduitCredit> GetProduitsCommande()
        {
            var lst = DbManager.GetAll<ClassL.ProduitCredit>();
            var lp = new List<ProduitCredit>();
            foreach (var item in lst)
            {
                item.SetProduit(GetProduit(item.ProduitId));
                var credit = GetVenteCredit(item.CommandeId);
                item.SetVenteCredit(credit);
                item.SetClient(GetClient(credit.ClientId));
                lp.Add(item);
            }
            return lp;
        }
        private List<ProduitCredit> GetProduitsCommande(int cmdid)
        {
            var lst = DbManager.GetAllProductByCmd(cmdid);
            var lp = new List<ProduitCredit>();
            foreach (var item in lst)
            {
                item.SetProduit(GetProduit(item.ProduitId));
                var credit = GetVenteCredit(item.CommandeId);
                item.SetVenteCredit(credit);
                item.SetClient(GetClient(credit.ClientId));
                lp.Add(item);
            }

            return lp;
        }
        private List<ProduitCredit> GetProduitsCommandee(int cmdid)
        {
            var lst = DbManager.GetAllProductByCmd(cmdid);
            var lp = new List<ProduitCredit>();
            GetTva();
            foreach (var item in lst)
            {
                item.SetProduit(GetProduit(item.ProduitId));
                var credit = GetVenteCredit(item.CommandeId);
                Somme = credit.Montant;
                SommeRestante = credit.MontantRestant;
                item.SetVenteCredit(credit);
                item.SetClient(GetClient(credit.ClientId));
                //montant de la vente
                lp.Add(item);
            }
            
            //montant tva
            if (Tva != null)
            {
                if (Tva.Apply)
                    SommeTva = (Somme * Tva.Taux) / 100;
            }
            //Somme TTC
            SommeTTC = SommeTva + Somme;
            return lp;
        }

        private List<ProduitCredit> GetProduitsCommande(List<ProduitCredit> lst)
        {
            var lp = new List<ProduitCredit>();
            foreach (var item in lst)
            {
                item.SetProduit(GetProduit(item.ProduitId));
                lp.Add(item);
            }
            return lp;
        }

        private ClassL.Produit GetProduit(int id)
        {
            return DbManager.GetById<ClassL.Produit>(id);
        }

        private ClassL.Client GetClient(int id)
        {
            return DbManager.GetClientById(id);
        }

        private ClassL.VenteCredit GetVenteCredit(int id)
        {
            return DbManager.GetById<ClassL.VenteCredit>(id);
        }

        private void SetData(List<ProduitCredit> commande)
        {
            datagrid.ItemsSource = commande;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var row = sender as DataGridRow;
            var c = row.Item as ClassL.ProduitCredit;
            RowIndex = c.Id;
            row.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            var row = sender as DataGridRow;
            var c = row.Item as ClassL.Client;
            int rid = row.GetIndex();
            if (rid == 0)
                row.Background = new SolidColorBrush(Colors.White);
            else
            {
                var color = rid % 2 == 0 ? Colors.White : Colors.LightGreen;
                row.Background = new SolidColorBrush(color);
            }
        }


        private void Research()
        {
            var rslt = Search(TxtSearch.Text);
            if (rslt == null)
                ResetList();
            if (rslt.Count > 0)
            {
                ProduitCommandes = null;
                ProduitCommandes = rslt;
                SetData(ProduitCommandes);
            }
            else
            {
                //ResetList();
            }
        }
        
        private void Research(int k)
        {
            var rslt = SingleSearch(TxtSearch.Text);
            if (rslt == null || rslt.Count == 0)
            {
                MessageBox.Show(Properties.Resources.EmptyResult, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //Message_Box(Properties.Resources.EmptyResult, Properties.Resources.Accept, new SolidColorBrush(Colors.SteelBlue));
                ResetList();
                return;
            }

            ProduitCommandes = null;
            ProduitCommandes = (rslt);
            SetData(ProduitCommandes);

        }
        
        private List<ProduitCredit> SingleSearch(string critere)
        {
            try
            {
                var rslt = from c in ProduitCommandes where c.CommandeId.ToString() == critere || c.Id.ToString() == critere || c.Montant.ToString() == critere || c.PrixUnitaire.ToString() == critere || c.Quantite.ToString() == critere || c.QuantiteRestante.ToString() == critere || c.ProductName == critere select c;
                return rslt.ToList();
            }
            catch
            {
                return null;
            }
        }

        private void ResetList()
        {
            //Clients = GetClient();
            if(IsUnique)
                ProduitCommandes = GetProduitsCommande(CommandeId);
            else
                ProduitCommandes = GetProduitsCommande();
            SetData(ProduitCommandes);
        }

        private List<ProduitCredit> Search(string critere)
        {
            var rslt = from c in ProduitCommandes where c.CommandeId.ToString().Contains(critere) || c.Id.ToString().Contains(critere) || c.Montant.ToString().Contains(critere) || c.PrixUnitaire.ToString().Contains(critere) || c.Quantite.ToString().Contains(critere) || c.QuantiteRestante.ToString().Contains(critere) || c.ProductName.Contains(critere) select c;
            return rslt.ToList();
        }

        private void TxtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsFilter)
            {
                //Is a number?
                if (Converter.IsNumeric(TxtSearch.Text))
                {
                    Research();
                }
                else
                {
                    if (TxtSearch.Text.Length >= 3)
                    {
                        Research();
                    }
                    else
                    {
                        ResetList();
                    }
                }
            }

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                ResetList();
                return;
            }
            Research(1);
        }

        private void ChkFilter_Unchecked(object sender, RoutedEventArgs e)
        {
            IsFilter = false;
            btnSearch.IsEnabled = true;
        }

        private void ChkFilter_Checked(object sender, RoutedEventArgs e)
        {
            IsFilter = true;
            btnSearch.IsEnabled = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetCompany();
            GetTva();
            SetData(ProduitCommandes);
            btnSearch.IsEnabled = true;
            ChkFilter.IsChecked = false;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = sender as ListeProduitCommende;
            var ah = window.ActualHeight;
            datagrid.Height = ah - ah / 3;
        }

        private void btnlstproduitnonsolde_Click(object sender, RoutedEventArgs e)
        {
            ProduitCommandes = null;
            ProduitCommandes = GetProduitsCommande(DbManager.GetProduitCreditNonSolde(CommandeId));
            SetData(ProduitCommandes);
        }

        private void btnlstproduitsolde_Click(object sender, RoutedEventArgs e)
        {
            ProduitCommandes = null;
            ProduitCommandes = GetProduitsCommande(DbManager.GetProduitCreditSolde(CommandeId));
            SetData(ProduitCommandes);
        }

        private void btnlstproduit_Click(object sender, RoutedEventArgs e)
        {
            ProduitCommandes = null;
            ProduitCommandes = GetProduitsCommande(DbManager.GetProduitCredit(CommandeId));
            SetData(ProduitCommandes);
        }

        private void GetTva()
        {
            var tva = DbManager.GetAll<Tva>();
            if (tva.Count > 0)
                Tva = tva[0];
            else
                Tva = null;
        }
        private void GetCompany()
        {
            var company = DbManager.GetAll<CompanyInfo>();
            if (company.Count > 0)
                Company = company[0];
            else
                Company = null;

        }

        private bool GenProduitVenduList(List<ClassL.ProduitCredit> credits)
        {
            
            Office office = new Office();
            //Company info
            if (Company != null)
            {
                office.CompanyName = Company.Nom;
                office.CompanyTel = $"Tel:{Company.Contact}";
                office.CompanyEmail = $"Email:{Company.Email}";
            }
            else
            {
                office.CompanyName = "Easy manager";
                office.CompanyTel = "Tel:+228 99 34 12 11";
                office.CompanyEmail = "Email:delaflor@flor.com";
            }
            //verifie si la tva doit-être appliquer
            if (Tva != null)
            {
                if (Tva.Apply == true)
                {
                    office.tauxtva = $"{Tva.Taux} %";
                }
            }
            office.Code = Properties.Resources.Code;
            office.Produit = Properties.Resources.ProduitTitle;
            office.Quantite = Properties.Resources.Quantite;
            office.Client = Properties.Resources.Client;
            office.PrixUnitaire = Properties.Resources.Price;
            office.QuantiteRestante = Properties.Resources.LeftQuantity;
            office.ResteAPayer = Properties.Resources.ResteAPayer;
            office.Discount = Properties.Resources.Discount;
            office.CommandeClient = Properties.Resources.CommandeClient;
            office.Utilisateur = Properties.Resources.User;
            office.Date = Properties.Resources.Date;
            office.Montant = Properties.Resources.Montant;
            office.Total = Properties.Resources.Total;
            office.TotalTTC = Properties.Resources.TTC;
            office.tva = Properties.Resources.TVA;
            office.NomClient = $"{Properties.Resources.Client}: {credits[0].NomClient}";
            office.TotalReste = $"{Properties.Resources.Total} {Properties.Resources.Reste}";
            office.Somme = Somme;
            office.SommeRestant = SommeRestante;
            office.SommeTtc = SommeTTC;
            office.SommeTva = SommeTva;
            office.ProduitCommandes = credits;
            office.ListeProduitVendu = Properties.Resources.ListProduitVendu;

            SaveLocation = InfoChecker.SaveLoc(Properties.Resources.ProductList, $"{Properties.Resources.ProduitTitle}{Properties.Resources.VenteCredit}");
            return office.GenListeProduitCredit(System.IO.Path.GetFullPath("Files\\ListeProduitVenduCredit_Prototype.dotx"), SaveLocation);
        }

        private async void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            //Gen liste des ventes
            Progress= Visibility.Visible;
            if (await PrintAsync(ProduitCommandes))
            {
                showDocument = new ShowDocument(SaveLocation);
                Progress = Visibility.Hidden;
                showDocument.ShowDialog();
            }
            else
            {
                Progress = Visibility.Hidden;
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool Print(List<ProduitCredit> lst)
        {
            return GenProduitVenduList(lst);
        }

        private async Task<bool> PrintAsync(List<ProduitCredit> lst)
        {
            return await Task.Run(() => Print(lst));
        }
    }
}
