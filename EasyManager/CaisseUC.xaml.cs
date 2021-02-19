using EasyManager.MenuItems;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClassL=EasyManagerLibrary;
using EasyManagerDb;
using EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for CaisseUC.xaml
    /// </summary>
    public partial class CaisseUC : UserControl
    {
        public CaisseViewModel ViewModel { get; set; } = new CaisseViewModel();
        List<OperationCaisse> listsorti = new List<OperationCaisse>();
        List<OperationCaisse> listentre = new List<OperationCaisse>();
        List<ProduitVendu> listvente = new List<ProduitVendu>();
        public string SaveLocation { get; set; }
        public ShowDocument showDocument { get; set; }
        public GestionDeCaisse GetCaisseHome { get; set; }
        public Home GetHome { get; set; }
        public decimal Somme { get; set; } = 0;
        public decimal SommeTva { get; set; } = 0;
        public decimal SommeTTC { get; set; } = 0;
        public Tva Tva { get; set; } = new Tva();
        public CaisseUC(GestionDeCaisse CaisseHome)
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

            ViewModel.DateDebut = InfoChecker.AjustDate(InfoChecker.DateArriere(day));
            ViewModel.DateFin = InfoChecker.AjustDate(InfoChecker.NextDate(DateTime.UtcNow, LeftDaysToTheEnd));
            ViewModel.PieChart = piechart;
            GetCaisseHome = CaisseHome;
            DataContext = ViewModel;
        }




        public void CalendarDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            Calendar.SelectedDate = ViewModel.DatePicker.Date;
        }

        public void CalendarDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!Calendar.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

            ViewModel.DatePicker.Date = Calendar.SelectedDate.Value;

            //update list on date change
            ViewModel.DateDebut = InfoChecker.AjustDate(ViewModel.DatePicker.Date);
            ListOperationCaisse(InfoChecker.StringToDate(ViewModel.DateDebut), InfoChecker.StringToDate(ViewModel.DateFin));
            ListVenteInfo(InfoChecker.StringToDate(ViewModel.DateDebut), InfoChecker.StringToDate(ViewModel.DateFin));
            SetDataInfo();

        }

        public void CalendarDialogOpenedEventHandlerF(object sender, DialogOpenedEventArgs eventArgs)
        {
            CalendarF.SelectedDate = ViewModel.DatePicker.DateF;
        }

        public void CalendarDialogClosingEventHandlerF(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!CalendarF.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

            ViewModel.DatePicker.DateF = CalendarF.SelectedDate.Value;

            //update list on date change
            ViewModel.DateFin = InfoChecker.AjustDate(ViewModel.DatePicker.DateF);

            ListOperationCaisse(InfoChecker.StringToDate(ViewModel.DateDebut), InfoChecker.StringToDate(ViewModel.DateFin));
            ListVenteInfo(InfoChecker.StringToDate(ViewModel.DateDebut), InfoChecker.StringToDate(ViewModel.DateFin));

            SetDataInfo();

        }

        private List<CaisseItem> TestData()
        {
            var lst = new List<CaisseItem>();
            CaisseItem item;
            ClassL.OperationCaisse operation;
            for (int i = 0; i < 7; i++)
            {
                item = new CaisseItem();
                operation = new ClassL.OperationCaisse();
                operation.Date = DateTime.UtcNow;
                operation.Montant = 123 * i + 1;
                
                item.GetOperationCaisse = operation;
                lst.Add(item);
            }
            return lst;
        }

        private ClassL.Operation GetOperation(int id)
        {
            return DbManager.GetById<ClassL.Operation>(id);
        }





        private void SetSortieData(List<CaisseItem> caisseItems)
        {
            PnlSortie.Children.Clear();
            foreach (var item in caisseItems)
            {
                PnlSortie.Children.Add(item);
            }
        }

        private void SetEntreData(List<CaisseItem> caisseItems)
        {
            PnlEntree.Children.Clear();
            foreach (var item in caisseItems)
            {
                PnlEntree.Children.Add(item);
            }
        }

        private void SetVenteData(List<CaisseVenteItem> caisseItems)
        {
            Pnlvente.Children.Clear();
            foreach (var item in caisseItems)
            {
                Pnlvente.Children.Add(item);
            }
        }

        private void SetResumeData()
        {
            CaisseItem caisseItem = new CaisseItem();
            CaisseItem caisseItem2 = new CaisseItem();

            caisseItem.libelle = Properties.Resources.Expenses;
            caisseItem2.libelle = Properties.Resources.Incomings;

            caisseItem.montant = ViewModel.SortieTotale;
            caisseItem2.montant = ViewModel.Recette;

            Pnlresume.Children.Clear();

            Pnlresume.Children.Add(caisseItem);
            Pnlresume.Children.Add(caisseItem2);
        }







        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            ListOperationCaisse(InfoChecker.StringToDate(ViewModel.DateDebut), InfoChecker.StringToDate(ViewModel.DateFin));
            ListVenteInfo(InfoChecker.StringToDate(ViewModel.DateDebut), InfoChecker.StringToDate(ViewModel.DateFin));
            SetDataInfo();
            if (GetCaisseHome != null)
            {
                GetCaisseHome.Title = Properties.Resources.Checkout;
                GetCaisseHome.MainTitle.Text = Properties.Resources.Checkout;
            }

        }

        private void SetDataInfo()
        {
            SetSortieData(SortiesCaisseItems(listsorti));

            SetEntreData(EntreeCaisseItems(listentre));

            SetVenteData(CaisseVenteItems(listvente));

            SetResumeData();
        }



        private List<CaisseItem> SortiesCaisseItems(List<OperationCaisse> lst)
        {
            List<CaisseItem> caisseItems = new List<CaisseItem>();
            CaisseItem caisseItem;
            Somme = 0;
            foreach (var item in lst)
            {
                Somme += item.Montant;
                caisseItem = new CaisseItem();
                caisseItem.GetOperationCaisse = item;
                caisseItems.Add(caisseItem);
            }
            ViewModel.SortieTotale = Somme;
            return caisseItems;
        }
        private List<CaisseItem> EntreeCaisseItems(List<OperationCaisse> lst)
        {
            List<CaisseItem> caisseItems = new List<CaisseItem>();
            CaisseItem caisseItem;
            Somme = 0;
            foreach (var item in lst)
            {
                Somme += item.Montant;
                caisseItem = new CaisseItem();
                caisseItem.GetOperationCaisse = item;
                caisseItems.Add(caisseItem);
            }
            ViewModel.EntreTotale = Somme;
            return caisseItems;
        }
        private List<CaisseVenteItem> CaisseVenteItems(List<ProduitVendu> lst)
        {
            List<CaisseVenteItem> caisseventeItems = new List<CaisseVenteItem>();
            CaisseVenteItem caisseventeItem;
            Somme = 0;
            foreach (var item in lst)
            {
                Somme += item.Montant;
                caisseventeItem = new CaisseVenteItem();
                caisseventeItem.GetProduitVendu = item;
                caisseventeItems.Add(caisseventeItem);
            }
            ViewModel.VenteTotale = Somme;
            return caisseventeItems;
        }









        private void ListOperationCaisse(DateTime dateTime, DateTime dateTime2)
        {
           
            var lst=DbManager.GetDataByDate_<ClassL.OperationCaisse>("Date", InfoChecker.AjustDate(dateTime), InfoChecker.AjustDate(dateTime2));
            var lstS = new List<OperationCaisse>();
            var lstE = new List<OperationCaisse>();

            if (lst == null)
                return;

            foreach (var item in lst)
            {
                var op = GetOperation(item.OperationId);
                item.SetOperation(op);
                if (op.TypeOperation == TypeOperation.Sortie)
                    lstS.Add(item);
                else
                    lstE.Add(item);
            }
            listsorti= OperationCaisse(lstS);
            listentre = OperationCaisse(lstE);
        }

        private void ListVenteInfo(DateTime dateTime, DateTime dateTime2)
        {
            //liste des ventes
            var lst = GetVentesByDateInterval(InfoChecker.AjustDate(dateTime), InfoChecker.AjustDate(dateTime2));
            listvente = ProduitVendu(lst);
        }

        private OperationCaisse CombineOperationCaisse(OperationCaisse one, OperationCaisse two)
        {
            OperationCaisse opcaisse = new OperationCaisse();
            opcaisse = one;
            opcaisse.Montant += two.Montant;
            return opcaisse;
        }

        private List<OperationCaisse> OperationCaisse(List<OperationCaisse> lst)
        {
            
            List<OperationCaisse> operationcaisses = new List<OperationCaisse>();
            foreach (var item in lst)
            {
                if (operationcaisses.Contains(item, new OperationCaisseComparer()))
                {
                    var rslt = (from p in operationcaisses where p.OperationId == item.OperationId select p).Single();
                    var prodindex = operationcaisses.IndexOf(rslt);
                    var prod = operationcaisses[prodindex];
                    var combine = CombineOperationCaisse(prod, item);
                    operationcaisses.RemoveAt(prodindex);
                    operationcaisses.Add(combine);
                }
                else
                {
                    operationcaisses.Add(item);
                }
            }
            return operationcaisses;
        }


        private List<ClassL.Vente> GetVentesByDateInterval(string debut, string fin)
        {
            return DbManager.GetDataByDate<ClassL.Vente>("Date", debut, fin);
        }

        

        private ProduitVendu CombineProduct(ProduitVendu one, ProduitVendu two)
        {
            ProduitVendu pv = new ProduitVendu();
            pv = one;
            pv.Montant += two.Montant;
            pv.Quantite += two.Quantite;
            return pv;
        }
        private List<ProduitVendu> ProduitVendu(List<ProduitVendu> lst)
        {

            List<ProduitVendu> produits = new List<ProduitVendu>();
            foreach (var item in lst)
            {
                if (produits.Contains(item, new ProduitSortiCompairer()))
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

        private ClassL.Client GetClient(int id)
        {
            return DbManager.GetClientById(id);
        }

        private List<ProduitVendu> ProduitVendu(List<ClassL.Vente> ventes)
        {
            List<ProduitVendu> produits = new List<ProduitVendu>();
            foreach (var item2 in ventes)
            {
                foreach (var item in GetProduitsVenduu(item2.Id))
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

        private void Btnprintsortie_Click(object sender, RoutedEventArgs e)
        {
            PrintFlux(listsorti, ViewModel.SortieTotale);
        }

        private void Btnprintentree_Click(object sender, RoutedEventArgs e)
        {
            PrintFlux(listentre, ViewModel.EntreTotale,'E');
        }

        private async void PrintFlux(List<OperationCaisse> lst, decimal sum, char S = 'S')
        {
            //var watch = System.Diagnostics.Stopwatch.StartNew();
            GetCaisseHome.viewModel.Progress = Visibility.Visible;
            if (lst.Count() > 0)
            {
                if (await PrintAsync(lst, sum, S))
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
            else
            {
                GetCaisseHome.viewModel.Progress = Visibility.Hidden;
                MessageBox.Show(Properties.Resources.EmptyList, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }

        #region Print Sortie
        private bool GenOperationCaisseFlux(List<OperationCaisse> operationCaisses,decimal sum,char S='S')
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
            office.SortieToale = sum;
            office.Total = Properties.Resources.Total;

            if (S == 'S')
            {
                office.OperationCaisseListe = Properties.Resources.PrintSortie;
                SaveLocation = InfoChecker.SaveLoc(Properties.Resources.Sortie, Properties.Resources.Sortie);
            }
            else if (S == 'R')
            {
                office.OperationCaisseListe = Properties.Resources.Checkout;
                SaveLocation = InfoChecker.SaveLoc(Properties.Resources.Sortie, Properties.Resources.Checkout);
            }
            else
            {
                office.OperationCaisseListe = Properties.Resources.PrintEntree;
                SaveLocation = InfoChecker.SaveLoc(Properties.Resources.Enter, Properties.Resources.Enter);
            }

            return office.GenListeFluxOperation(System.IO.Path.GetFullPath("Files\\Caisse_Prototype.dotx"), SaveLocation);
        }

        

        private bool Print(List<ClassL.OperationCaisse> lst, decimal sum, char S = 'S')
        {
            return GenOperationCaisseFlux(lst,sum,S);
        }

        private async Task<bool> PrintAsync(List<OperationCaisse> lst, decimal sum, char S = 'S')
        {
            return await Task.Run(() => Print(lst,sum,S));
        }

        #endregion

        #region Caisse Resume
        /// <summary>
        /// Generate a pdf
        /// </summary>
        /// <param name="operationCaisses"></param>
        /// <returns>2=>Ok<br/>1=>Error<br/>0=>the list is empty</returns>
        private int GenCaisseResume(List<OperationCaisse> operationCaisses)
        {
            if (operationCaisses.Count() > 0)
            {
                Office office = new Office();
                office.CompanyName = "DeLaFlor Corporation";
                office.CompanyTel = "Tel:+228 99 34 12 11";
                office.CompanyEmail = "Email:delaflor@flor.com";
                office.Operation = Properties.Resources.Operation;
                office.Montant = Properties.Resources.Montant;
                office.Utilisateur = Properties.Resources.User;
                office.Date = Properties.Resources.Date;
                office.OperationCaisseListe = Properties.Resources.Checkout;
                office.GetOperationCaisses = operationCaisses;
                office.DepenseTotal = ViewModel.SortieTotale;
                office.RecetteTotal = ViewModel.Recette;
                office.Depense = Properties.Resources.Expenses;
                office.Recette = Properties.Resources.Incomings;
                office.SommeEnCaisse = Properties.Resources.SumCheckout;
                office.SommeEnCaisseTotal = ViewModel.SommeEnCaisse;
                office.Periode = $"{InfoChecker.AjustDateWithDMY(InfoChecker.StringToDate(ViewModel.DateDebut))} - {InfoChecker.AjustDateWithDMY(InfoChecker.StringToDate(ViewModel.DateFin))}";

                SaveLocation = InfoChecker.SaveLoc(Properties.Resources.Checkout, Properties.Resources.Checkout);

                var rslt= office.GenCaisseResume(System.IO.Path.GetFullPath("Files\\Caisse_Prototype.dotx"), SaveLocation);
                return rslt == true ? 2 : 1;
            }
            else
            {
                return 0;
            }
            
        }
        private int Print(List<ClassL.OperationCaisse> lst)
        {
            return GenCaisseResume(lst);
        }

        private async Task<int> PrintAsync(List<OperationCaisse> lst)
        {
            return await Task.Run(() => Print(lst));
        }
        #endregion

        private void Btnvente_Click(object sender, RoutedEventArgs e)
        {
            Rapport rapport = new Rapport(GetCaisseHome.viewModel.GetHome);
            rapport.ShowDialog();
        }

        private async void BtnPrintAll_Click(object sender, RoutedEventArgs e)
        {
            var lst = listentre;
            lst.AddRange(listsorti);
            //var watch = System.Diagnostics.Stopwatch.StartNew();
            GetCaisseHome.viewModel.Progress = Visibility.Visible;
            var rslt = await PrintAsync(lst);
            if (rslt==2)
            {
                showDocument = new ShowDocument(SaveLocation);
                GetCaisseHome.viewModel.Progress = Visibility.Hidden;
                //watch.Stop();
                //Console.WriteLine($"premier arret: {watch.ElapsedMilliseconds}");
                showDocument.ShowDialog();
            }
            else if (rslt==1)
            {
                GetCaisseHome.viewModel.Progress = Visibility.Hidden;
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                GetCaisseHome.viewModel.Progress = Visibility.Hidden;
                MessageBox.Show(Properties.Resources.EmptyList, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
