using EasyManagerDb;
using EasyManagerLibrary;
using System;
using System.Collections.Generic;
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
using ClassL = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for Produit.xaml
    /// </summary>
    public partial class Produit : Window
    {
        public ClassL.Produit Product { get; set; }
        public ClassL.Produit Old_Product { get; set; }
        public ClassL.Produit New_Product { get; set; }
        public int ProduitId { get; set; }
        public int CatId { get; set; }
        public int SupplierId { get; set; } = 0;
        public Produit()
        {
            InitializeComponent();
            ManagerTable("PrixGrossiste", "REAL", "0");
        }

        public Produit(int produitid)
        {
            InitializeComponent();
            ProduitId = produitid;
            ManagerTable("PrixGrossiste", "REAL", "0");
        }

        public Produit(ClassL.Produit produit)
        {
            InitializeComponent();
            Product = produit;
            Old_Product = produit;
            ManagerTable("PrixGrossiste", "REAL", "0");
        }

        private ClassL.Produit GetProduitById(int id)
        {
            var prod = DbManager.GetById<ClassL.Produit>(id);
            return prod;
        }

        private void SetData(ClassL.Produit prod)
        {
            txtNom.Text = prod.Nom;
            txtDescription.Text = prod.Description;
            txtPrix.Text = prod.PrixUnitaire.ToString();
            txtQuantiteAlerte.Text = prod.QuantiteAlerte.ToString();
            txtQuantiteTotal.Text = prod.QuantiteTotale.ToString();
            CbCatList.SelectedIndex = prod.CategorieId;
            CbSupList.SelectedIndex = prod.SupplierId;
            txtPrixgrossiste.Text = prod.PrixGrossiste.ToString();

        }

        private void CbCatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
                return;
            var rslt = NewProductInfo(Old_Product, New_Product);

            if (DbManager.UpDate(rslt,Old_Product.Id))
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

            New_Product = new ClassL.Produit();
            New_Product.Nom = nom;
            New_Product.PrixUnitaire = Convert.ToDecimal(prix);
            New_Product.PrixGrossiste = Convert.ToDecimal(prixgrossiste);

            New_Product.QuantiteAlerte = Convert.ToDouble(quantitealerte);
            New_Product.QuantiteTotale = Convert.ToDouble(quantite);
            New_Product.Description = description;
            New_Product.CategorieId = CatId == 0 ? 1 : CatId;
            New_Product.QuantiteRestante = Product.QuantiteTotale;
            New_Product.SupplierId = SupplierId;
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

        private void txtPrix_KeyUp(object sender, KeyEventArgs e)
        {
            OnlyNumber(sender as TextBox);
        }

        private void txtQuantiteTotal_KeyUp(object sender, KeyEventArgs e)
        {
            OnlyNumber(sender as TextBox);
        }

        private void OnlyNumber(TextBox tb)
        {
            tb.Text = InfoChecker.NumericDecOnlyRegExp(tb.Text);
            tb.SelectionStart = tb.Text.Length;
        }

        private void txtQuantiteAlerte_KeyUp(object sender, KeyEventArgs e)
        {
            txtQuantiteAlerte.Text = InfoChecker.NumericOnlyRegExp(txtQuantiteAlerte.Text);
            txtQuantiteAlerte.SelectionStart = txtQuantiteAlerte.Text.Length;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetData(Product);
            FilledDropDown();
            FilledSupplierDropDown();
        }

        private ClassL.Produit NewProductInfo(ClassL.Produit oldprod,ClassL.Produit newprod)
        {
            ClassL.Produit prod = new ClassL.Produit();
            prod.Nom = newprod.Nom;
            prod.PrixUnitaire = newprod.PrixUnitaire;
            prod.PrixGrossiste = newprod.PrixGrossiste;
            
            if (prod.PrixUnitaire == 0)
                prod.PrixUnitaire = oldprod.PrixUnitaire;
            
            if (prod.PrixGrossiste == 0)
                prod.PrixGrossiste = oldprod.PrixGrossiste;
            
            prod.QuantiteAlerte = newprod.QuantiteAlerte;
            double qtydiff = QuantityDifference(oldprod, newprod);
            prod.QuantiteTotale = newprod.QuantiteTotale;
            prod.Description = newprod.Description;
            prod.CategorieId = newprod.CategorieId;
            prod.SupplierId = newprod.SupplierId;
            
            if(oldprod.QuantiteRestante > 0)
                prod.QuantiteRestante = oldprod.QuantiteRestante + qtydiff;


            return prod;

        }

        private void btnNewCat_Click(object sender, RoutedEventArgs e)
        {
            Categorie categorie = new Categorie();
            categorie.ShowDialog();
            FilledDropDown();

        }

        private void Reset()
        {
            txtDescription.Text = string.Empty;
            txtNom.Text = string.Empty;
            txtPrix.Text = string.Empty;
            txtQuantiteAlerte.Text = string.Empty;
            txtQuantiteTotal.Text = string.Empty;
            CbCatList.SelectedIndex = 0;
            CbSupList.SelectedIndex = 0;
            txtPrixgrossiste.Text = string.Empty;
        }

        private void PerformClick(Button btn)
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(btn);
            IInvokeProvider invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProvider.Invoke();
        }

        private double QuantityDifference(ClassL.Produit oldprod,ClassL.Produit newprod)
        {
            return newprod.QuantiteTotale- oldprod.QuantiteTotale;
        }

        private void Messagee_Box(string msg, string actiontitle, SolidColorBrush colorBrush)
        {
            MsgContent.Content = msg;
            MsgContent.Foreground = colorBrush;
            MsgContent.FontSize = 20;
            btnOk.Content = actiontitle;
            PerformClick(btnmsgbox);
        }

        private void btnNewSup_Click(object sender, RoutedEventArgs e)
        {
            var supplier = new Supplier();
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
