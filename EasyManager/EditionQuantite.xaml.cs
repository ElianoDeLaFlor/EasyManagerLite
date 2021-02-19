using EasyManagerDb;
using EasyManagerLibrary;
using MaterialDesignThemes.Wpf;
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

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for EditionProduit.xaml
    /// </summary>
    public partial class EditionProduit : Window
    {
        public EditionProduit()
        {
            InitializeComponent();
        }
        public int ProdId { get; set; }
        public double NewQuantite { get; set; } = 0;
        public decimal UnitPrice { get; set; } = 0;
        public QuantiteEdition Quantite { get; set; }
        
        private void FilledDropDown()
        {
            CbCatList.ItemsSource = ProdList();
        }
        private void CbCatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems[0].ToString();

            var cat = DbManager.GetProduitByName(item);
            ProdId = cat.Id;

        }
        private List<string> ProdList()
        {
            var lst = DbManager.GetAll<EasyManagerLibrary.Produit>();

            List<string> lstprod = new List<string>();
            foreach (var item in lst)
            {
                lstprod.Add(item.Nom);
            }
            return lstprod;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FilledDropDown();
        }

        private bool CheckFields()
        {
            string quantite = txtqantite.Text;
            string prixunitaire = txtprixunitaire.Text;

            if (InfoChecker.IsEmpty(quantite))
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (InfoChecker.IsEmpty(prixunitaire))
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (ProdId == 0)
            {
                MessageBox.Show(Properties.Resources.CatSelectionError, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            NewQuantite = double.Parse(quantite);
            UnitPrice = decimal.Parse(prixunitaire);
            
            Quantite = new QuantiteEdition();
            Quantite.ProduitId = ProdId;
            Quantite.UtilisateurId = 1; // Connected user id
            Quantite.Quantite = NewQuantite;
            Quantite.PrixUnitaire = UnitPrice;
            Quantite.DateEdition = DateTime.UtcNow;
            
            return true;

        }

        private void txtqantite_KeyUp(object sender, KeyEventArgs e)
        {
            OnlyNumber(sender as TextBox);
        }

        private void txtprixunitaire_KeyUp(object sender, KeyEventArgs e)
        {
            OnlyNumber(sender as TextBox);
        }

        private void btncreate_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
                return;
            if (DbManager.Save(Quantite))
            {
                MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateProduct(NewQuantite);
                Reset();
            }
            else
            {
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool UpdateProduct(double addquantite)
        {
            var oldprod = DbManager.GetProduitById(ProdId);
            double newquantitetotal = oldprod.QuantiteTotale + addquantite;
            double newquantiterestante = oldprod.QuantiteRestante + addquantite;
            // Update
            oldprod.QuantiteTotale = newquantitetotal;
            oldprod.QuantiteRestante = newquantiterestante;
            oldprod.PrixUnitaire = UnitPrice;
            return DbManager.UpDate(oldprod,ProdId);
        }

        private void OnlyNumber(TextBox tb)
        {
            tb.Text = InfoChecker.NumericDecOnlyRegExp(tb.Text);
            tb.SelectionStart = tb.Text.Length;
        }

        private void Reset()
        {
            txtprixunitaire.Text = string.Empty;
            txtqantite.Text = string.Empty;
            CbCatList.SelectedIndex = 0;
        }

        private void btncancel_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void PerformClick(Button btn)
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(btn);
            IInvokeProvider invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProvider.Invoke();
        }

        private void Messagee_Box(string msg, string actiontitle, SolidColorBrush colorBrush)
        {
            //MsgContent.Content = msg;
            //MsgContent.Foreground = colorBrush;
            //MsgContent.FontSize = 20;
            //btnOk.Content = actiontitle;
            //PerformClick(btnmsgbox);
        }

    }
}
