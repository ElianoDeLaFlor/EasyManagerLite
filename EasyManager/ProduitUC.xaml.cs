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
    /// Interaction logic for ProduitUC.xaml
    /// </summary>
    public partial class ProduitUC : UserControl
    {
        public EasyManagerLibrary.Produit Product { get; set; }
        public Home GetHome { get; set; }
        public int CatId { get; set; }
        public int SupplierId { get; set; } = 0;

        public ProduitUC()
        {
            InitializeComponent();
            ManagerTable("PrixGrossiste", "REAL", "0");
        }
        public ProduitUC(Home h)
        {
            InitializeComponent();
            GetHome = h;
            container.MinWidth = GetHome.ActualWidth - 500;
            GetHome.SizeChanged += GetHome_SizeChanged;
            ManagerTable("PrixGrossiste", "REAL", "0");
        }

        private void GetHome_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            container.MinWidth = Math.Abs(GetHome.ActualWidth - 500);
        }

        private void CbCatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count>0)
            {

                var item = e.AddedItems[0].ToString();
                if (item == Properties.Resources.MakeSelection)
                    CatId = 0;
                else
                {
                    var cat = DbManager.GetCategorieByLibelle(item);
                    CatId = cat.Id;
                }
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
                return;
            if (DbManager.Save(Product))
            {
                MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                Reset();
            }
            else
            {
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CheckFields()
        {
            string nom = txtNom.Text.ToLower();
            string description = txtDescription.Text.Length <= 3 ? txtNom.Text : txtDescription.Text;
            string prix = txtPrix.Text;
            string prixgrossiste = txtPrixgrossiste.Text;
            string quantite = txtQuantiteTotal.Text;
            string quantitealerte = txtQuantiteAlerte.Text;

            if (InfoChecker.IsEmpty(nom))
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (DbManager.GetProduitByName(nom) != null)
            {
                MessageBox.Show(Properties.Resources.ProductNameExist, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (InfoChecker.IsEmpty(prix))
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (InfoChecker.IsEmpty(prixgrossiste))
            {
                if (MessageBox.Show(Properties.Resources.WholeSaleWarning, Properties.Resources.MainTitle, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
                    return false;
                
            }

            if (InfoChecker.IsEmpty(quantite))
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (InfoChecker.IsEmpty(quantitealerte))
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (CatId == 0)
            {
                MessageBox.Show(Properties.Resources.CatSelectionError, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            Product = new EasyManagerLibrary.Produit();
            Product.Nom = nom;
            Product.PrixUnitaire = Convert.ToDecimal(prix);
            if(decimal.TryParse(prixgrossiste,out _))
                Product.PrixGrossiste = Convert.ToDecimal(prixgrossiste);

            if (Product.PrixGrossiste == 0)
                Product.PrixGrossiste = Product.PrixUnitaire;



            Product.QuantiteAlerte = double.Parse(quantitealerte);
            Product.QuantiteTotale = double.Parse(quantite);
            Product.Description = description;
            Product.CategorieId = CatId == 0 ? 1 : CatId;
            Product.QuantiteRestante = Product.QuantiteTotale;
            Product.SupplierId = SupplierId;

            return true;

        }

        public void ManagerTable(string columnName, string typ, string defaultvalue)
        {
            //check if the column already exist the table
            if (!DbManager.CustumQueryCheckColumn<EasyManagerLibrary.Produit>(columnName))
            {
                // the column is not in the table
                // so we have to create it
                DbManager.CreateNewColumn<EasyManagerLibrary.Produit>(columnName, typ, defaultvalue);
            }

        }

        private void FilledDropDown()
        {
            CbCatList.ItemsSource = CatList();
        }

        private void FilledSupplierDropDown()
        {
            CbSupList.ItemsSource = SupplierList();
        }

        private List<string> CatList()
        {
            List<string> catetorielst = new List<string>();
            var lst = DbManager.GetAll<EasyManagerLibrary.Categorie>();

            catetorielst.Add(Properties.Resources.MakeSelection);
            foreach (var item in lst)
            {
                catetorielst.Add(item.Libelle);
            }
            return catetorielst;
        }

        private List<string> SupplierList()
        {
            List<string> supplierlst = new List<string>();
            var lst = DbManager.GetAll<EasyManagerLibrary.Supplier>();

            supplierlst.Add(Properties.Resources.MakeSelection);
            foreach (var item in lst)
            {
                supplierlst.Add(item.Nom);
            }
            return supplierlst;
        }

        private void txtPrix_KeyUp(object sender, KeyEventArgs e)
        {
            OnlyNumber(sender as TextBox);
        }

        private void txtQuantiteTotal_KeyUp(object sender, KeyEventArgs e)
        {
            OnlyNumber(sender as TextBox);
        }

        private void txtQuantiteAlerte_KeyUp(object sender, KeyEventArgs e)
        {
            OnlyNumber(sender as TextBox);
        }
        private void OnlyNumber(TextBox tb)
        {
            tb.Text = InfoChecker.NumericDecOnlyRegExp(tb.Text);
            tb.SelectionStart = tb.Text.Length;
        }
        private void Reset()
        {
            txtDescription.Text = string.Empty;
            txtNom.Text = string.Empty;
            txtPrix.Text = string.Empty;
            txtPrixgrossiste.Text = string.Empty;
            txtQuantiteAlerte.Text = string.Empty;
            txtQuantiteTotal.Text = string.Empty;
            CbCatList.SelectedIndex = 0;
            CbSupList.SelectedIndex = 0;
        }

        private void btnNewCat_Click(object sender, RoutedEventArgs e)
        {
            Categorie categorie = new Categorie();
            categorie.ShowDialog();
            FilledDropDown();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            FilledDropDown();
            FilledSupplierDropDown();
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.SaveNewProduct;
                GetHome.MainTitle.Text = Properties.Resources.SaveNewProduct;
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            

        }

        private void btnNewSup_Click(object sender, RoutedEventArgs e)
        {
            var supplier = new Supplier(GetHome);
            supplier.ShowDialog();
            FilledSupplierDropDown();
        }

        private void CbSupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {

                var item = e.AddedItems[0].ToString();
                if (item == Properties.Resources.MakeSelection)
                    SupplierId = 0;
                else
                {
                    var sup = DbManager.GetByColumnName<EasyManagerLibrary.Supplier>("Nom", item);
                    SupplierId = sup[0].Id;
                }
            }
        }
    }
}
