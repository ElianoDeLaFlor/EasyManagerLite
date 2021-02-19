using System;
using System.Collections.Generic;
using System.IO;
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
using CsvHelper;
using EasyManagerDb;
using EasyManagerLibrary;
using ClassL = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for ProduitListUC.xaml
    /// </summary>
    public partial class ProduitListUC : UserControl
    {
        #region Variable declaration
        private List<ClassL.Produit> Produits { get; set; }
        public List<ClassL.Produit> GetProduits { get; set; }
        public ClassL.Produit Produit { get; set; }
        public bool IsFilter { get; set; }
        public Home GetHome { get; set; }
        public string SaveLocation { get; set; }
        public ShowDocument showDocument { get; set; }
        public CompanyInfo Company { get; set; } = new CompanyInfo();
        public ClassL.Supplier Supplier { get; set; } = new ClassL.Supplier();
        #endregion
        public ProduitListUC()
        {
            InitializeComponent();
        }

        public ProduitListUC(Home h)
        {
            InitializeComponent();
            GetHome = h;
            Datagrid.Width = GetHome.ActualWidth - 30;
            GetProduits = GetAllProduit();
            Produits = GetAllProduit();
            GetHome.SizeChanged += GetHome_SizeChanged;
            DataContext = this;
        }

        public string SavePath { get; set; }

        private void GetHome_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Datagrid.Width = GetHome.ActualWidth - 30;
        }

        private List<ClassL.Produit> GetAllProduit()
        {
            var rslt = DbManager.GetAll<ClassL.Produit>();
            var lst = new List<ClassL.Produit>();
            ClassL.Supplier sup;
            foreach (var item in rslt)
            {
                sup = new ClassL.Supplier();
                sup = DbManager.GetById<ClassL.Supplier>(item.SupplierId);
                var cat = DbManager.GetById<ClassL.Categorie>(item.CategorieId);
                item.SetCategorieNom(cat.Libelle);
                item.SetSupplier(sup);
                lst.Add(item);
            }
            return lst;
        }

        private void SetData(List<ClassL.Produit> produits)
        {
            Datagrid.ItemsSource = produits;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var row = sender as DataGridRow;
            Produit = row.Item as ClassL.Produit;
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

        private List<ClassL.Produit> Search(string critere)
        {
            var rslt = from c in GetAllProduit() where c.Nom.Contains(critere) || c.Id.ToString().Contains(critere) || c.PrixUnitaire.ToString().Contains(critere) || c.QuantiteAlerte.ToString().Contains(critere) || c.QuantiteRestante.ToString().Contains(critere) || c.QuantiteTotale.ToString().Contains(critere) || c.GetCategorieNom.Contains(critere) select c;
            return rslt.ToList();
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                ResetList();
                return;
            }
            Research(1);
        }

        private void ResetList()
        {
            Produits = GetAllProduit();
            SetData(Produits);
        }

        private void Research(int k)
        {
            var rslt = SingleSearch(TxtSearch.Text);
            if (rslt == null || rslt.Count == 0)
            {
                MessageBox.Show(Properties.Resources.EmptyResult, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ResetList();
                return;
            }
            Produits = null;
            Produits = (rslt);
            SetData(Produits);

        }

        private List<ClassL.Produit> SingleSearch(string critere)
        {
            try
            {
                var rslt = from c in GetAllProduit() where c.Nom.Contains(critere) || c.Id.ToString().Contains(critere) || c.PrixUnitaire.ToString().Contains(critere) || c.QuantiteAlerte.ToString().Contains(critere) || c.QuantiteRestante.ToString().Contains(critere) || c.QuantiteTotale.ToString().Contains(critere) || c.GetCategorieNom.Contains(critere) select c;
                return rslt.ToList();
            }
            catch
            {
                return null;
            }
        }

        private void Research()
        {
            var rslt = Search(TxtSearch.Text);
            if (rslt == null)
                ResetList();
            if (rslt.Count > 0)
            {
                Produits = null;
                Produits = rslt;
                SetData(Produits);
            }
            else
            {
                //ResetList();
            }
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
                    if (TxtSearch.Text.Length > 3)
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetData(GetAllProduit());
            
            btnSearch.IsEnabled = true;
            ChkFilter.IsChecked = false;
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.ModuleListeProduits;
                GetHome.MainTitle.Text = Properties.Resources.ModuleListeProduits;
            }
        }

        private void GetCompany()
        {
            var company = DbManager.GetAll<CompanyInfo>();
            if (company.Count > 0)
                Company = company[0];
            else
                Company = null;

        }

        private bool GenProductList(List<ClassL.Produit> produits)
        {
            GetCompany();
            Office office = new Office();
            if (Company != null)
            {
                office.CompanyName = Company.Nom;
                office.CompanyTel = $"Tel: {Company.Contact}";
                office.CompanyEmail = $"Email: {Company.Email}";
            }
            else
            {
                office.CompanyName = "EasyManager";
                office.CompanyTel = "Tel: +228 00 00 00 00";
                office.CompanyEmail = "Email: elianosetekpo@gmail.com";
            }
            office.Code = Properties.Resources.Code;
            office.Produit = Properties.Resources.ProduitTitle;
            office.PrixUnitaire = Properties.Resources.Price;
            office.Quantite = Properties.Resources.Quantite;
            office.QuantiteRestante = Properties.Resources.LeftQuantity;
            office.QuantiteVendue = Properties.Resources.SellQuantity;
            office.Categorie = Properties.Resources.Category;
            office.ListProduct = Properties.Resources.ModuleListeProduits;
            office.GetProduits = produits;
            office.At = Properties.Resources.At;
            SaveLocation = InfoChecker.SaveLoc(Properties.Resources.ProductList, Properties.Resources.ProduitTitle);
            return office.GenListeProduit(System.IO.Path.GetFullPath("Files\\ListeProduit_Prototype.dotx"), SaveLocation);
        }

        private async Task<bool> GenProductListAsync(List<ClassL.Produit> produits)
        {
            return await Task.Run(() => GenProductList(produits));
        }

        private async void Btnprint_Click(object sender, RoutedEventArgs e)
        {
            GetHome.DataContextt.Progress = Visibility.Visible;

            if (await GenProductListAsync(GetProduits))
            {
                GetHome.DataContextt.Progress = Visibility.Hidden;
                showDocument = new ShowDocument(SaveLocation);
                showDocument.ShowDialog();
            }
            else
            {
                GetHome.DataContextt.Progress = Visibility.Hidden;
                MessageBox.Show(Properties.Resources.Error,Properties.Resources.MainTitle,MessageBoxButton.OK,MessageBoxImage.Error);
            }
            /*
            CsvHelper.Configuration.CsvConfiguration csvConfiguration = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);
            using(var sw=new StreamWriter(@"C:\Users\setek\OneDrive\Documents\EasyManager\test.csv"))
            {
                var writer = new CsvWriter(sw,csvConfiguration);
                writer.WriteHeader(typeof(ClassL.Produit));
                foreach (var item in Produits)
                {
                    writer.WriteRecord(item);
                }
            }
            MessageBox.Show("ok");*/
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            var pnl = sender as StackPanel;
            var btns = pnl.Children;
            foreach (var item in btns)
            {
                var btn = item as Button;
                if(btn!=null)
                    btn.Visibility = Visibility.Visible;
                else
                {
                    var btnn = item as MaterialDesignThemes.Wpf.PopupBox;
                    btnn.Visibility = Visibility.Visible;
                }

            }
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            var pnl = sender as StackPanel;
            var btns = pnl.Children;
            foreach (var item in btns)
            {
                var btn = item as Button;
                if (btn != null)
                    btn.Visibility = Visibility.Hidden;
                else
                {
                    var btnn = item as MaterialDesignThemes.Wpf.PopupBox;
                    btnn.Visibility = Visibility.Hidden;
                }

            }
        }

        private void PopupBox_Opened(object sender, RoutedEventArgs e)
        {
            //var supplir = new ClassL.Supplier();
            //Supplier = DbManager.GetById<ClassL.Supplier>(Produit.SupplierId);
        }

        private void btnedit_Click(object sender, RoutedEventArgs e)
        {
            Produit produit = new Produit(Produit);
            produit.ShowDialog();
            ResetList();
        }

        private void btnCSV_Click(object sender, RoutedEventArgs e)
        {
            FileBrowserDialog();
        }

        public void FileBrowserDialog()
        {

            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Title = Properties.Resources.MainTitle;
            //saveFileDialog.CheckFileExists = true;
            //saveFileDialog.CheckPathExists = true;
            saveFileDialog.DefaultExt = "csv";
            saveFileDialog.Filter = "Csv fules (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SavePath = saveFileDialog.FileName;
                if(CSVManager.WriteDatas<ClassL.Produit>(SavePath, Produits))
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
