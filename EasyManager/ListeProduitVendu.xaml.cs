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
using System.Windows.Shapes;
using ClassL= EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for ListeProduitVendu.xaml
    /// </summary>
    public partial class ListeProduitVendu : Window
    {
        #region Variable declaration
        public List<ProduitVendu> ProduitVendus { get; set; }
        public List<ProduitVendu> GetProduitVendus { get; set; }
        public int VenteId { get; set; }
        public bool IsFilter { get; set; }
        /// <summary>
        /// Indique que les produits vendus sont récupérés par VenteId
        /// </summary>
        public bool IsUnique { get; set; }
        public string SaveLocation { get; set; }
        public ShowDocument showDocument { get; set; }
        public Tva Tva { get; set; } = new Tva();
        public CompanyInfo Company { get; set; } = new CompanyInfo();
        public decimal Somme { get; set; } = 0;
        public decimal SommeTva { get; set; } = 0;
        public decimal SommeTTC { get; set; } = 0;
        public decimal SommeRestante { get; set; } = 0;
        #endregion

        public ListeProduitVendu()
        {
            InitializeComponent();
            GetProduitVendus = GetProduitsVendu();
            ProduitVendus = GetProduitsVendu();
            IsUnique = false;
        }

        public ListeProduitVendu(int venteid)
        {
            InitializeComponent();
            VenteId = venteid;
            GetProduitVendus = GetProduitsVenduu(VenteId);
            ProduitVendus = GetProduitsVendu(VenteId);
            IsUnique = false;
        }

        private List<ProduitVendu> GetProduitsVendu()
        {
            var lst = DbManager.GetAll<ProduitVendu>();
            var lp = new List<ProduitVendu>();
            foreach (var item in lst)
            {
                item.SetProduit(GetProduit(item.ProduitId));
                lp.Add(item);
            }
            return lp;
        }

        private List<ProduitVendu> GetProduitsVendu(int venteid)
        {
            var lst = DbManager.GetAllProductByVnt(venteid);
            var lp = new List<ProduitVendu>();
            foreach (var item in lst)
            {
                item.SetProduit(GetProduit(item.ProduitId));
                var vente = GetVente(item.VenteId);
                Somme = vente.Montant;
                item.SetVente(vente);
                if(vente.ClientId.HasValue && vente.ClientId.Value!=0)
                    item.SetClient(GetClient(vente.ClientId.Value));
                lp.Add(item);
            }
            return lp;
        }

        private List<ProduitVendu> GetProduitsVenduu(int venteid)
        {
            var lst = DbManager.GetAllProductByVnt(venteid);
            var lp = new List<ProduitVendu>();
            GetTva();
            foreach (var item in lst)
            {
                item.SetProduit(GetProduit(item.ProduitId));
                var vente = GetVente(item.VenteId);
                Somme = vente.Montant;
                item.SetVente(vente);
                if (vente.ClientId.HasValue && vente.ClientId.Value != 0)
                    item.SetClient(GetClient(vente.ClientId.Value));
                else if(vente.CommandeId.HasValue && vente.CommandeId.Value!=0)
                {
                    var ventecredit = DbManager.GetById<VenteCredit>(vente.CommandeId.Value);
                    item.SetClient(GetClient(ventecredit.ClientId));
                }
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



        private ClassL.Client GetClient(int id)
        {
            return DbManager.GetClientById(id);
        }

        private ClassL.Vente GetVente(int id)
        {
            return DbManager.GetById<ClassL.Vente>(id);
        }

        private List<ProduitVendu> GetProduitsVendu(List<ProduitVendu> lst)
        {
            var lp = new List<ProduitVendu>();
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

        private void SetData(List<ProduitVendu> vente)
        {
            datagrid.ItemsSource = vente;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var row = sender as DataGridRow;
            var c = row.Item as ProduitVendu;
            row.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            var row = sender as DataGridRow;
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
                ProduitVendus = null;
                ProduitVendus = rslt;
                SetData(ProduitVendus);
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

            ProduitVendus = null;
            ProduitVendus = (rslt);
            SetData(ProduitVendus);

        }

        private List<ProduitVendu> SingleSearch(string critere)
        {
            try
            {
                var rslt = from c in ProduitVendus where c.Id.ToString() == critere || c.VenteId.ToString() == critere || c.Montant.ToString() == critere || c.PrixUnitaire.ToString() == critere || c.Quantite.ToString() == critere || c.Discount.ToString() == critere || c.GetProduit().Nom == critere select c;
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
            if (IsUnique)
                ProduitVendus = GetProduitsVendu(VenteId);
            else
                ProduitVendus = GetProduitsVendu(VenteId);
           SetData(ProduitVendus);
        }

        private List<ProduitVendu> Search(string critere)
        {
            var rslt = from c in ProduitVendus where c.VenteId.ToString().Contains(critere) || c.Id.ToString().Contains(critere) || c.Montant.ToString().Contains(critere) || c.PrixUnitaire.ToString().Contains(critere) || c.Quantite.ToString().Contains(critere) || c.GetProduit().Nom.ToString().Contains(critere) || c.Discount.ToString().Contains(critere) select c;
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
            SetData(GetProduitsVendu(VenteId));
            btnSearch.IsEnabled = true;
            ChkFilter.IsChecked = false;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = sender as ListeProduitVendu;
            var ah = window.ActualHeight;
            datagrid.Height = ah - ah / 3;
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

        private async Task<bool> GenProduitVenduListAsync(List<ClassL.ProduitVendu> vendus)
        {
            return await Task.Run(() => GenProduitVenduList(vendus));
        }

        private bool GenProduitVenduList(List<ClassL.ProduitVendu> vendus)
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
            office.Discount = Properties.Resources.Discount;
            office.Montant = Properties.Resources.Montant;
            office.Total = Properties.Resources.Total;
            office.TotalTTC = Properties.Resources.TTC;
            office.tva = Properties.Resources.TVA;
            office.NomClient = $"{Properties.Resources.Client}: {vendus[0].NomClient}";
            office.Somme = Somme;
            office.SommeTtc = SommeTTC;
            office.SommeTva = SommeTva;
            office.ProduitVendus = vendus;
            office.ListeProduitVendu = Properties.Resources.ListProduitVendu;
            SaveLocation = InfoChecker.SaveLoc(Properties.Resources.ProductList, $"{Properties.Resources.ProduitTitle}{Properties.Resources.VenteCredit}");
            return office.GenListeProduitVendu(System.IO.Path.GetFullPath("Files\\ListeProduitVendu_Prototype.dotx"), SaveLocation);
        }

        private async void btnprint_Click(object sender, RoutedEventArgs e)
        {
            //Gen liste des produits vendu à un client
            Progress.Visibility = Visibility.Visible;
            if (await GenProduitVenduListAsync(GetProduitVendus))
            {
                Progress.Visibility = Visibility.Hidden;
                showDocument = new ShowDocument(SaveLocation);
                showDocument.ShowDialog();
            }
            else
            {
                Progress.Visibility = Visibility.Hidden;
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
