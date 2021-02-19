using EasyManager.MenuItems;
using EasyManagerDb;
using EasyManagerLibrary;
using LiveCharts;
using MaterialDesignThemes.Wpf;
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
using ClassL = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for Rapport.xaml
    /// </summary>
    public partial class Rapport : Window
    {
        public string DateDebut { get; set; } = "07-02-2020";
        public string DateFin { get; set; }= "2020-02-14";
        public string XData { get; set; }
        public List<ClassL.Vente> Ventes { get; set; }
        public List<VenteCredit> VenteCredits { get; set; }
        public List<double> LstColl { get; set; } = new List<double>();
        public Tva Tva { get; set; } = new Tva();
        public decimal Somme { get; set; } = 0;
        public decimal SommeTva { get; set; } = 0;
        public decimal SommeCreditPayer { get; set; } = 0;
        public decimal SommeTTC { get; set; } = 0;
        public List<RapportProduit> GetRapportProduits { get; set; } = new List<RapportProduit>();
        public CompanyInfo Company { get; set; } = new CompanyInfo();
        public string SaveLocation { get; set; }
        public ShowDocument showDocument { get; set; }
        public Home GetHome { get; set; }
        public Rapport()
        {
            InitializeComponent();
            int day = (DateTime.Now.Day) - 1;
            DateDebut = InfoChecker.AjustDate(InfoChecker.DateArriere(day));
            DateFin = InfoChecker.AjustDate(DateTime.Now);



            var datacontext = new RapportViewModel();
            datacontext.Rapport = this;
            datacontext.DatePicker = new DatePickerViewModel();
            DataContext = datacontext;

            ((RapportViewModel)DataContext).DatePicker.Date = Convert.ToDateTime(DateDebut);
            ((RapportViewModel)DataContext).DatePicker.DateF = Convert.ToDateTime(DateFin);
            Ventes = GetVentesByDateInterval(DateDebut, DateFin);
        }

        public Rapport(Home home)
        {
            InitializeComponent();
            int day = (DateTime.Now.Day) - 1;
            int LeftDaysToTheEnd;
            if (DateTime.UtcNow.Month == 2)
            {
                //febrary
                //total day of the month => 28
                LeftDaysToTheEnd = 28 - (DateTime.Now.Day);
            }
            else
            {
                //total day of the month=>30
                LeftDaysToTheEnd = 30 - (DateTime.Now.Day);
            }

            DateDebut = InfoChecker.AjustDate(InfoChecker.DateArriere(day));
            DateFin = InfoChecker.AjustDate(InfoChecker.NextDate(DateTime.UtcNow, LeftDaysToTheEnd));

            GetHome = home;

            var datacontext = new RapportViewModel();
            datacontext.Rapport = this;
            datacontext.DatePicker = new DatePickerViewModel();
            DataContext = datacontext;

            ((RapportViewModel)DataContext).DatePicker.Date = Convert.ToDateTime(DateDebut);
            ((RapportViewModel)DataContext).DatePicker.DateF = Convert.ToDateTime(DateFin);
            Ventes = GetVentesByDateInterval(DateDebut, DateFin);
            UpdateVenteChart();
        }

        private List<ClassL.Vente> GetVentesByDateInterval(string debut,string fin)
        {
            return DbManager.GetDataByDate<ClassL.Vente>("Date", debut, fin);
        }

        private List<VenteCredit> GetVenteCreditsByDateInterval(string debut, string fin)
        {
            return DbManager.GetDataByDate<VenteCredit>("Date", debut, fin);
        }

        private double VenteTotalItem(ClassL.Vente vente)
        {
            var produitvendu = DbManager.GetByColumnName<ProduitVendu>("VenteId", vente.Id.ToString());
            double total = 0;
            foreach (var item in produitvendu)
            {
                total += item.Quantite;
            }
            return total;
        }

        private double VenteCreditTotalItem(VenteCredit credits)
        {
            var produitvendu = DbManager.GetByColumnName<ProduitCredit>("CommandeId", credits.Id.ToString());
            double total = 0;
            foreach (var item in produitvendu)
            {
                total += item.Quantite;
            }
            return total;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!CanAccess(GetHome.DataContextt.ConnectedUser))
            {
                IsEnabled = false;
                MessageBox.Show(Properties.Resources.CanAccess, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                LstColl.Clear();
                foreach (var item in Ventes)
                {
                    LstColl.Add(VenteTotalItem(item));
                }
                ChartValues<double> lstchartvalue = new ChartValues<double>(LstColl);
                rapportDiagram.XData = lstchartvalue;
            }        
        }


        public void CalendarDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            Calendar.SelectedDate = ((RapportViewModel)DataContext).DatePicker.Date;
        }

        public void CalendarDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!Calendar.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

            ((RapportViewModel)DataContext).DatePicker.Date = Calendar.SelectedDate.Value;

            //update list on date change
            DateDebut = InfoChecker.AjustDate(((RapportViewModel)DataContext).DatePicker.Date);
            UpdateVenteChart();
        }

        public void CalendarDialogOpenedEventHandlerF(object sender, DialogOpenedEventArgs eventArgs)
        {
            CalendarF.SelectedDate = ((RapportViewModel)DataContext).DatePicker.DateF;
        }

        public void CalendarDialogClosingEventHandlerF(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!CalendarF.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

            ((RapportViewModel)DataContext).DatePicker.DateF = CalendarF.SelectedDate.Value;

            //update list on date change
            DateFin = InfoChecker.AjustDate(((RapportViewModel)DataContext).DatePicker.DateF);
            //UpdateVentecreditChart();
            UpdateVenteChart();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = (sender as ComboBox).SelectedIndex;
            if (index == 0)
            {
                //vente
                VenteChart();
            }
            else
            {
                //VenteCredit
                UpdateVentecreditChart();
            }
        }

        private void VenteChart()
        {
            Ventes = GetVentesByDateInterval(DateDebut, DateFin);
            LstColl.Clear();
            foreach (var item in Ventes)
            {
                LstColl.Add(VenteTotalItem(item));
            }
            ChartValues<double> lstchartvalue = new ChartValues<double>(LstColl);
            rapportDiagram.XData = lstchartvalue;
            rapportDiagram.XSection = 2;
            rapportDiagram.YSection = 2;
        }

        private void UpdateVenteChart()
        {

            Ventes = GetVentesByDateInterval(DateDebut, DateFin);
            List<ProduitVendu> pv = new List<ProduitVendu>();
            List<ProduitVendu> CreditP = new List<ProduitVendu>();
            pv = ProduitVendu(Ventes);
            CreditP = CreditPayers(Ventes);
            //SetTiles(GenProductResume(pv));


            //
            VenteCredits = GetVenteCreditsByDateInterval(DateDebut, DateFin);
            List<ProduitCredit> pc = new List<ProduitCredit>();
            pc = ProduitCredit(VenteCredits);
            //SetTiles(GenProductResume(pv,pc));
            SetTiles(GenProductResume(pv, pc,CreditP));
        }

        private void UpdateVentecreditChart()
        {
            VenteCredits = GetVenteCreditsByDateInterval(DateDebut, DateFin);
            LstColl.Clear();
            foreach (var item in VenteCredits)
            {
                LstColl.Add(VenteCreditTotalItem(item));
            }
            ChartValues<double> lstchartvalue = new ChartValues<double>(LstColl);
            rapportDiagram.XData = lstchartvalue;
            rapportDiagram.XSection = 2;
            rapportDiagram.YSection = 2;
        }

        private void GetTva()
        {
            var tva = DbManager.GetAll<Tva>();
            if (tva.Count > 0)
                Tva = tva[0];
            else
                Tva = null;
        }

        private ClassL.Produit GetProduit(int id)
        {
            return DbManager.GetById<ClassL.Produit>(id);
        }

        private ClassL.Vente GetVente(int id)
        {
            return DbManager.GetById<ClassL.Vente>(id);
        }

        private VenteCredit GetVenteCredit(int id)
        {
            return DbManager.GetById<VenteCredit>(id);
        }

        private ClassL.Client GetClient(int id)
        {
            return DbManager.GetClientById(id);
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
                else if (vente.CommandeId.HasValue && vente.CommandeId.Value != 0)
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

        private List<ProduitVendu> GetCreditPayer(int venteid)
        {
            var lst = DbManager.GetAllProductByVnt(venteid);
            var lp = new List<ProduitVendu>();
            GetTva();
            foreach (var item in lst)
            {
                item.SetProduit(GetProduit(item.ProduitId));
                var vente = GetVente(item.VenteId);
                
                if (vente.CommandeId.HasValue && vente.CommandeId.Value != 0)
                {
                    Somme = vente.Montant;
                    item.SetVente(vente);
                    var ventecredit = DbManager.GetById<VenteCredit>(vente.CommandeId.Value);
                    item.SetClient(GetClient(ventecredit.ClientId));
                    lp.Add(item);
                }
                
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

        private List<ProduitCredit> GetProduitsCredit(int creditid)
        {
            var lst = DbManager.GetAllProductByCmd(creditid);
            var lp = new List<ProduitCredit>();
            GetTva();
            foreach (var item in lst)
            {
                item.SetProduit(GetProduit(item.ProduitId));
                var ventecredit = GetVenteCredit(item.CommandeId);
                Somme = ventecredit.Montant;
                item.SetVenteCredit(ventecredit);
                if (ventecredit.ClientId != 0)
                    item.SetClient(GetClient(ventecredit.ClientId));

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

        private List<ProduitVendu> ProduitVendu(List<ProduitVendu> lst)
        {

            List<ProduitVendu> produits = new List<ProduitVendu>();
            foreach (var item in lst)
            {
                if(produits.Contains(item,new ProduitSortiCompairer()))
                {
                    var prodindex = produits.IndexOf(item);
                    var prod = produits[prodindex];
                    var combine = CombineProduct(prod, item);
                    produits.Insert(prodindex, combine);
                }
                else
                {
                    produits.Add(item);
                }
            }
            return produits;
        }

        /// <summary>
        /// Gets the list of product selled
        /// </summary>
        /// <param name="ventes"></param>
        /// <returns></returns>
        private List<ProduitVendu> ProduitVendu(List<ClassL.Vente> ventes)
        {
            ProduitVendu produitVendu = new ProduitVendu();

            List<ProduitVendu> produits = new List<ProduitVendu>();
            foreach (var item2 in ventes)
            {
                foreach (var item in GetProduitsVenduu(item2.Id))
                {
                    if (produits.Contains(item, new ProduitSortiCompairer()))
                    {
                        var rslt= (from p in produits where p.ProduitId == item.ProduitId select p).Single();
                        var prodindex = produits.IndexOf(rslt);
                        var combine = CombineProduct(rslt, item);
                        produits.RemoveAt(prodindex);
                        produits.Add(combine);
                    }
                    else
                    {
                        produits.Add(item);
                    }
                }
            }
            
            return produits;
        }

        private List<ProduitVendu> CreditPayers(List<ClassL.Vente> ventes)
        {
            ProduitVendu produitVendu = new ProduitVendu();

            List<ProduitVendu> produits = new List<ProduitVendu>();
            foreach (var item2 in ventes)
            {
                foreach (var item in GetCreditPayer(item2.Id))
                {
                    if (produits.Contains(item, new ProduitSortiCompairer()))
                    {
                        var rslt = (from p in produits where p.ProduitId == item.ProduitId select p).Single();
                        var prodindex = produits.IndexOf(rslt);
                        var combine = CombineProduct(rslt, item);
                        produits.RemoveAt(prodindex);
                        produits.Add(combine);
                    }
                    else
                    {
                        produits.Add(item);
                    }
                }
            }

            return produits;
        }

        /// <summary>
        /// Gets the list of credit product
        /// </summary>
        /// <param name="credits"></param>
        /// <returns></returns>
        private List<ProduitCredit> ProduitCredit(List<VenteCredit> credits)
        {
            ProduitCredit produitCredit = new ProduitCredit();

            List<ProduitCredit> produits = new List<ProduitCredit>();
            foreach (var item2 in credits)
            {
                foreach (var item in GetProduitsCredit(item2.Id))
                {
                    if (produits.Contains(item, new ProduitSortiCompairer()))
                    {
                        var rslt = (from p in produits where p.ProduitId == item.ProduitId select p).Single();
                        var prodindex = produits.IndexOf(rslt);
                        var combine = CombineProduct(rslt, item);
                        produits.RemoveAt(prodindex);
                        produits.Add(combine);
                    }
                    else
                    {
                        produits.Add(item);
                    }
                }
            }

            return produits;
        }

        private ProduitVendu CombineProduct(ProduitVendu one,ProduitVendu two)
        {
            ProduitVendu pv = new ProduitVendu();
            pv = one;
            pv.Montant += two.Montant;
            pv.Quantite += two.Quantite;
            return pv;
        }

        private ProduitCredit CombineProduct(ProduitCredit one, ProduitCredit two)
        {
            ProduitCredit pv = new ProduitCredit();
            pv = one;
            pv.Montant += two.Montant;
            pv.Quantite += two.Quantite;
            return pv;
        }

        private ProduitCredit CombineVenteCredit(ProduitCredit one, ProduitCredit two)
        {
            ProduitCredit pv = new ProduitCredit();
            pv = one;
            pv.Montant += two.Montant;
            pv.Quantite += two.Quantite;
            return pv;
        }

        private List<ProductResumeTile> GenProductResume(List<ProduitVendu> lst)
        {
            List<ProductResumeTile> productResumes = new List<ProductResumeTile>();
            List<ProduitStateInfo> produitStateInfos;
            ProduitStateInfo PSI;
            ProductResumeTile PR;
            foreach (var item in lst)
            {
                PR = new ProductResumeTile();
                produitStateInfos = new List<ProduitStateInfo>();
                PR.Title = GetProduit(item.ProduitId).Nom;
                //
                PSI = new ProduitStateInfo();
                PSI.Title = Properties.Resources.SellQuantity;
                PSI.Quantity = item.Quantite.ToString();
                produitStateInfos.Add(PSI);
                //
                PSI = new ProduitStateInfo();
                PSI.Title = Properties.Resources.Montant;
                PSI.Quantity = InfoChecker.CurrencyFormat(item.Montant);
                produitStateInfos.Add(PSI);
                //
                PR.ProduitStateInfos = produitStateInfos;
                //
                productResumes.Add(PR);
            }
            return productResumes;
        }

        private List<ProductResumeTile> GenProductResume(List<ProduitCredit> lst)
        {
            List<ProductResumeTile> productResumes = new List<ProductResumeTile>();
            List<ProduitStateInfo> produitStateInfos;
            ProduitStateInfo PSI;
            ProductResumeTile PR;
            foreach (var item in lst)
            {
                PR = new ProductResumeTile();
                produitStateInfos = new List<ProduitStateInfo>();
                PR.Title = GetProduit(item.ProduitId).Nom;
                //
                PSI = new ProduitStateInfo();
                PSI.Title = Properties.Resources.SellQuantity;
                PSI.Quantity = item.Quantite.ToString();
                produitStateInfos.Add(PSI);
                //
                PSI = new ProduitStateInfo();
                PSI.Title = Properties.Resources.Montant;
                PSI.Quantity = InfoChecker.CurrencyFormat(item.Montant);
                produitStateInfos.Add(PSI);
                //
                PR.ProduitStateInfos = produitStateInfos;
                //
                productResumes.Add(PR);
            }
            return productResumes;
        }

        private List<ProductResumeTile> GenProductResume(List<ProduitVendu> lstvendu,List<ProduitCredit> lstcredit)
        {
            List<ProductResumeTile> productResumes = new List<ProductResumeTile>();
            List<ProduitStateInfo> produitStateInfos;
            ProduitStateInfo PSI;
            ProductResumeTile PR;
            foreach (var item in lstvendu)
            {
                PR = new ProductResumeTile();
                produitStateInfos = new List<ProduitStateInfo>();
                PR.Title = GetProduit(item.ProduitId).Nom;
                //
                PSI = new ProduitStateInfo();
                PSI.Title = Properties.Resources.SellQuantity;
                PSI.Quantity = item.Quantite.ToString();
                produitStateInfos.Add(PSI);
                //
                PSI = new ProduitStateInfo();
                PSI.Title = Properties.Resources.Montant;
                PSI.Quantity = InfoChecker.CurrencyFormat(item.Montant);
                produitStateInfos.Add(PSI);

                //Ventecredit status
                if(lstcredit.Contains(item as ProduitSorti,new ProduitSortiCompairer()))
                {
                    var rslt = (from p in lstcredit where p.ProduitId == item.ProduitId select p).Single();
                    var prodindex = lstcredit.IndexOf(rslt);
                    

                    //
                    PSI = new ProduitStateInfo();
                    PSI.Title = Properties.Resources.VenteCredit;
                    PSI.Quantity = rslt.Quantite.ToString();
                    PSI.Couleur = "Salmon";
                    produitStateInfos.Add(PSI);
                    //
                    PSI = new ProduitStateInfo();
                    PSI.Title = Properties.Resources.MontantTotalCredit;
                    PSI.Quantity = InfoChecker.CurrencyFormat(rslt.Montant);
                    PSI.Couleur = "Salmon";
                    produitStateInfos.Add(PSI);

                    lstcredit.RemoveAt(prodindex);
                }

                

                //
                PR.ProduitStateInfos = produitStateInfos;
                //
                productResumes.Add(PR);
            }

            foreach (var item2 in lstcredit)
            {
                PR = new ProductResumeTile();
                produitStateInfos = new List<ProduitStateInfo>();
                PR.Title = GetProduit(item2.ProduitId).Nom;
                //
                PSI = new ProduitStateInfo();
                PSI.Title = Properties.Resources.VenteCredit;
                PSI.Couleur = "Salmon";
                PSI.Quantity = item2.Quantite.ToString();
                produitStateInfos.Add(PSI);
                //
                PSI = new ProduitStateInfo();
                PSI.Title = Properties.Resources.MontantTotalCredit;
                PSI.Quantity = InfoChecker.CurrencyFormat(item2.Montant);
                PSI.Couleur = "Salmon";
                produitStateInfos.Add(PSI);
                PR.ProduitStateInfos = produitStateInfos;
                productResumes.Add(PR);
            }
            return productResumes;
        }

        private List<ProductResumeTile> GenProductResume(List<ProduitVendu> lstvendu, List<ProduitCredit> lstcredit, List<ProduitVendu> lstcreditpayer)
        {
            List<ProductResumeTile> productResumes = new List<ProductResumeTile>();
            
            List<ProduitStateInfo> produitStateInfos;
            ProduitStateInfo PSI;
            ProductResumeTile PR;
            foreach (var item in lstvendu)
            {
                PR = new ProductResumeTile();
                RapportProduit rapportProduits = new RapportProduit();
                produitStateInfos = new List<ProduitStateInfo>();
                var name= GetProduit(item.ProduitId).Nom;
                PR.Title = name;
                PR.DateFin = DateDebut;
                PR.DateFin = DateFin;
                rapportProduits.Name = name;
                //
                PSI = new ProduitStateInfo();
                PSI.Title = Properties.Resources.SellQuantity;
                PSI.Quantity = item.Quantite.ToString();
                rapportProduits.QuantiteVendue = item.Quantite.ToString();
                produitStateInfos.Add(PSI);
                //
                PSI = new ProduitStateInfo();
                PSI.Title = Properties.Resources.Montant;
                PSI.Quantity = InfoChecker.CurrencyFormat(item.Montant);
                rapportProduits.Montant= InfoChecker.CurrencyFormat(item.Montant);
                produitStateInfos.Add(PSI);

                //Credit payé
                if (lstcreditpayer.Contains(item as ProduitSorti, new ProduitSortiCompairer()))
                {
                    var rslt = (from p in lstcreditpayer where p.ProduitId == item.ProduitId select p).Single();
                    var prodindex = lstcreditpayer.IndexOf(rslt);
                    //
                    PSI = new ProduitStateInfo();
                    PSI.Title = Properties.Resources.PayedQuanity;
                    PSI.Quantity = rslt.Quantite.ToString();
                    PSI.Couleur = "LightBlue";
                    rapportProduits.QuantiteCreditPayer = rslt.Quantite.ToString();
                    produitStateInfos.Add(PSI);
                    //
                    PSI = new ProduitStateInfo();
                    PSI.Title = Properties.Resources.PayedQuanityCost;
                    PSI.Quantity = InfoChecker.CurrencyFormat(rslt.Montant);
                    PSI.Couleur = "LightBlue";
                    rapportProduits.MontantCreditPayer = InfoChecker.CurrencyFormat(rslt.Montant);
                    produitStateInfos.Add(PSI);

                    lstcreditpayer.RemoveAt(prodindex);
                }

                //Ventecredit status
                if (lstcredit.Contains(item as ProduitSorti, new ProduitSortiCompairer()))
                {
                    var rslt = (from p in lstcredit where p.ProduitId == item.ProduitId select p).Single();
                    var prodindex = lstcredit.IndexOf(rslt);


                    //
                    PSI = new ProduitStateInfo();
                    PSI.Title = Properties.Resources.VenteCredit;
                    PSI.Quantity = rslt.Quantite.ToString();
                    PSI.Couleur = "Salmon";
                    rapportProduits.QuaniteCredit= rslt.Quantite.ToString();
                    produitStateInfos.Add(PSI);
                    //
                    PSI = new ProduitStateInfo();
                    PSI.Title = Properties.Resources.MontantTotalCredit;
                    PSI.Quantity = InfoChecker.CurrencyFormat(rslt.Montant);
                    rapportProduits.MontantCredit= InfoChecker.CurrencyFormat(rslt.Montant);
                    PSI.Couleur = "Salmon";
                    produitStateInfos.Add(PSI);

                    lstcredit.RemoveAt(prodindex);
                }

                //
                PR.ProduitStateInfos = produitStateInfos;
                PR.Rapports = rapportProduits;
                //
                GetRapportProduits.Add(rapportProduits);
                productResumes.Add(PR);
            }

            //Credit
            foreach (var item2 in lstcredit)
            {
                PR = new ProductResumeTile();
                produitStateInfos = new List<ProduitStateInfo>();
                RapportProduit rapportProduits = new RapportProduit();
                //
                var name = GetProduit(item2.ProduitId).Nom;
                PR.Title = name;
                PR.DateFin = DateDebut;
                PR.DateFin = DateFin;
                rapportProduits.Name = name;
                PR.Title = name;
                //
                PSI = new ProduitStateInfo();
                PSI.Title = Properties.Resources.VenteCredit;
                PSI.Couleur = "Salmon";
                PSI.Quantity = item2.Quantite.ToString();
                rapportProduits.QuaniteCredit = item2.Quantite.ToString();
                produitStateInfos.Add(PSI);
                //
                PSI = new ProduitStateInfo();
                PSI.Title = Properties.Resources.MontantTotalCredit;
                PSI.Quantity = InfoChecker.CurrencyFormat(item2.Montant);
                PSI.Couleur = "Salmon";
                rapportProduits.MontantCredit = InfoChecker.CurrencyFormat(item2.Montant);
                produitStateInfos.Add(PSI);
                //
                PR.ProduitStateInfos = produitStateInfos;
                PR.Rapports = rapportProduits;
                //
                GetRapportProduits.Add(rapportProduits);
                productResumes.Add(PR);
            }
            return productResumes;
        }

        private void SetTiles(List<ProductResumeTile> lst)
        {
            if (testpnl == null)
                return;
            testpnl.Children.Clear();
            foreach (var item in lst)
            {
                testpnl.Children.Add(item);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = sender as Rapport;
            var ah = window.ActualHeight;
            //mainscroll.MaxHeight = ah - (ah /6);
            scroll.MaxHeight = ah - (ah /3);
        }

        private void scroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollbar = sender as ScrollViewer;
            if (scrollbar.VerticalScrollBarVisibility == ScrollBarVisibility.Visible)
                Console.WriteLine("showing");
            else
                Console.WriteLine("hide");
        }



        private async void BtnPrintAll_Click(object sender, RoutedEventArgs e)
        {
            ProgBarAll.Visibility = Visibility.Visible;
            if (await GenRapportAsync(GetRapportProduits))
            {
                ProgBarAll.Visibility = Visibility.Hidden;
                showDocument = new ShowDocument(SaveLocation);
                showDocument.ShowDialog();
            }
            else
            {
                ProgBarAll.Visibility = Visibility.Hidden;
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
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

        private Module RapportModule()
        {
            return DbManager.GetByColumnName<Module>("Libelle", Properties.Resources.ModuleRapport)[0];
        }

        public Module GetModule { get => RapportModule(); set { } }

        public bool CanAccess(Utilisateur utilisateur)
        {
            //UserRole
            var userrole = DbManager.GetByColumnName<Role>("Libelle", utilisateur.RoleLibelle)[0];
            //Role module
            var rolemodule = DbManager.GetByColumnName<RoleModule>("RoleId", userrole.Id.ToString());

            List<int> usermoduleid = new List<int>();
            foreach (var item in rolemodule)
                usermoduleid.Add(item.ModuleId);

            return usermoduleid.Contains(GetModule.Id);
        }

        private void btncsv_Click(object sender, RoutedEventArgs e)
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
                string SavePath = saveFileDialog.FileName;
                if (CSVManager.WriteDatas<ClassL.RapportProduit>(SavePath, GetRapportProduits))
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }


    public class RapportViewModel
    {
        public DatePickerViewModel DatePicker { get; set; }
        public Rapport Rapport { get; set; }
    }
}
