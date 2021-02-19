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
    /// Interaction logic for ProductResumeTile.xaml
    /// </summary>
    public partial class ProductResumeTile : UserControl
    {
        public List<ProduitStateInfo> ProduitStateInfos { get; set; }
        public RapportProduit Rapports { get; set; }
        public CompanyInfo Company { get; set; } = new CompanyInfo();
        public string SaveLocation { get; set; }
        public string Title { get; set; }
        public ShowDocument showDocument { get; set; }
        public ProductResumeTile()
        {
            InitializeComponent();
            DataContext = this;
        }
        public string DateDebut { get; set; }
        public string DateFin { get; set; }

        private void SetInfo()
        {
            if (ProduitStateInfos == null || ProduitStateInfos.Count == 0)
                return;
            PnlInfo.Children.Clear();
            foreach (var item in ProduitStateInfos)
            {
                
                PnlInfo.Children.Add(item);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetInfo();
            
        }

        private void GetCompany()
        {
            var company = DbManager.GetAll<CompanyInfo>();
            if (company.Count > 0)
                Company = company[0];
            else
                Company = null;

        }

        private void card_MouseEnter(object sender, MouseEventArgs e)
        {
            PnlPrint.Visibility = Visibility.Visible;
        }

        private void card_MouseLeave(object sender, MouseEventArgs e)
        {
            PnlPrint.Visibility = Visibility.Hidden;
        }

        private async void btnprint_Click(object sender, RoutedEventArgs e)
        {
            ProgBar.Visibility = Visibility.Visible;
            var fakelist = new List<RapportProduit>();
            fakelist.Add(Rapports);
            if (await GenRapportAsync(fakelist))
            {
                ProgBar.Visibility = Visibility.Hidden;
                showDocument = new ShowDocument(SaveLocation);
                showDocument.ShowDialog();
            }
            else
            {
                ProgBar.Visibility = Visibility.Hidden;
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async Task<bool> GenRapportAsync(List<RapportProduit> rapports)
        {
            return await Task.Run(() => GenRapport(rapports));
        }

        private bool GenRapport(List<RapportProduit> Rapports)
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
            office.Periode = $"{Properties.Resources.FactureDU} {InfoChecker.AjustDateWithDMY(InfoChecker.StringToDate(DateDebut))} {Properties.Resources.Au} {InfoChecker.AjustDateWithDMY(InfoChecker.StringToDate(DateFin))}";
            office.Produit = Properties.Resources.ProduitTitle;
            office.QuantiteVendue = Properties.Resources.SellQuantity;
            office.Montant = Properties.Resources.Montant;
            office.QuantiteCreditPayer = Properties.Resources.PayedQuanity;
            office.MontantCreditPayer = Properties.Resources.PayedQuanityCost;
            office.QuantiteCredit = Properties.Resources.CreditSellQuantity;
            office.MontantTotalCredit = Properties.Resources.MontantTotalCredit;
            office.GetRapportProduits = Rapports;
            office.ListProduct = Properties.Resources.Rapport;//Rapport
            SaveLocation = InfoChecker.SaveLoc(Properties.Resources.Rapport, Properties.Resources.Rapport);
            return office.GenRapports(System.IO.Path.GetFullPath("Files\\Rapport_Prototype.dotx"), SaveLocation);
        }
    }
}
