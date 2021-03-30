using EasyManager.MenuItems;
using EasyManagerDb;
using EasyManagerLibrary;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ClassL = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for VenteListeUC.xaml
    /// </summary>
    public partial class VenteListeUC : UserControl
    {
        #region Variable declaration
        private List<ClassL.Vente> Ventes { get; set; }
        /// <summary>
        /// Liste des ventes à supprimer
        /// </summary>
        private List<DeleteRef> DeleteList { get; set; } = new List<DeleteRef>();
        /// <summary>
        /// Liste des ventes dans la base de données
        /// </summary>
        public List<ClassL.Vente> GetVentes { get; set; }
        /// <summary>
        /// Contient les données d'un row
        /// </summary>
        public ClassL.Vente Vente { get; set; }
        /// <summary>
        /// Id de l'élément dans le row selectionné
        /// </summary>
        private int RowIndex = 0;
        public bool IsFilter { get; set; }
        public Home GetHome { get; set; }
        public string DateDebut { get; set; }
        public string DateFin { get; set; }
        public string SaveLocation { get; set; }
        public ShowDocument showDocument { get; set; }
        public decimal Somme { get; set; } = 0;
        public VenteCredit GetVenteCredit { get; set; }
        public Tva Tva { get; set; } = new Tva();
        public CompanyInfo Company { get; set; } = new CompanyInfo();
        public decimal SommeTva { get; set; } = 0;
        public decimal SommeTtc { get; set; } = 0;
        public bool IsChecked { get; set; } = true;
        public bool Generated { get; set; }

        public bool IsCommand { get; set; }
        #endregion

        public VenteListeUC()
        {
            InitializeComponent();
            GetVentes = GetAllVente(DateDebut, DateFin);
            Ventes = GetAllVente(DateDebut, DateFin);
        }
        public VenteListeUC(Home h)
        {
            InitializeComponent();
            GetHome = h;
            Datagrid.Width = GetHome.ActualWidth - 50;

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



            var datacontext = new VenteListeUCViewModel();
            datacontext.VenteListeUC = this;
            datacontext.DatePicker = new DatePickerViewModel();
            DataContext = datacontext;

            ((VenteListeUCViewModel)DataContext).DatePicker.Date = Convert.ToDateTime(DateDebut);
            ((VenteListeUCViewModel)DataContext).DatePicker.DateF = Convert.ToDateTime(DateFin);

            GetVentes = GetAllVente(DateDebut, DateFin);
            Ventes = GetAllVente(DateDebut, DateFin);
            GetHome.SizeChanged += GetHome_SizeChanged;
        }

        private void GetHome_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Datagrid.Width = GetHome.ActualWidth - 50;
        }

        /// <summary>
        /// Retourne toutes les ventes de la base de données
        /// </summary>
        /// <returns></returns>
        private List<ClassL.Vente> GetAllVente()
        {
            var rslt = DbManager.GetAll<ClassL.Vente>();
            var lst = new List<ClassL.Vente>();
            string cmdclient;
            foreach (var item in rslt)
            {
                var user = GetUtilisateur(item.UtilisateurId);              
                item.SetUser(user);
                if (item.ClientId.HasValue && item.ClientId.Value>0)
                {
                    var client = DbManager.GetById<ClassL.Client>(item.ClientId.Value);
                    cmdclient = $"{client.Prenom} {client.Nom}";
                }
                else
                {
                    if (item.CommandeId.HasValue && item.CommandeId.Value>0)
                    {
                        var vc=DbManager.GetById<ClassL.VenteCredit>(item.CommandeId.Value);
                        cmdclient = InfoChecker.FormatIdent(vc.Id);
                    }
                    else
                        cmdclient = "-";
                }
                item.SetCmdClient(cmdclient);
                lst.Add(item);
            }
            return lst;
        }

        /// <summary>
        /// Retourne toutes les ventes de la base de données
        /// </summary>
        /// <returns></returns>
        private List<ClassL.Vente> GetAllVente(string datedebut,string datefin)
        {
            var rslt = DbManager.GetDataByDate<ClassL.Vente>("Date", datedebut, datefin);
            var lst = new List<ClassL.Vente>();
            lst.Clear();
            string cmdclient;
            Somme = 0;
            foreach (var item in rslt)
            {
                var user = GetUtilisateur(item.UtilisateurId);
                item.SetUser(user);
                Somme += item.Montant;
                if (item.ClientId.HasValue && item.ClientId.Value > 0)
                {
                    var client = DbManager.GetById<ClassL.Client>(item.ClientId.Value);
                    cmdclient = $"{client.Prenom} {client.Nom}";
                }
                else
                {
                    if (item.CommandeId.HasValue && item.CommandeId.Value > 0)
                    {
                        var vc = DbManager.GetById<ClassL.VenteCredit>(item.CommandeId.Value);
                        cmdclient = InfoChecker.FormatIdent(vc.Id);
                    }
                    else
                        cmdclient = "-";
                }
                item.SetCmdClient(cmdclient);
                lst.Add(item);
            }
            return lst;
        }

        private ClassL.Client GetClient(int id)
        {
            return DbManager.GetById<ClassL.Client>(id);
        }

        private ClassL.Utilisateur GetUtilisateur(int id)
        {
            return DbManager.GetById<Utilisateur>(id);
        }

        private void SetData(List<ClassL.Vente> vente)
        {
            Datagrid.ItemsSource = vente;
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            var datagridcell = sender as DataGridCell;
           // var chk = datagridcell.Content as CheckBox;
            //Checked

            DeleteRef delete = new DeleteRef();
            delete.CanBeDelete = true;

            var row = sender as DataGridRow;
            Vente = row.Item as ClassL.Vente;
            RowIndex = Vente.Id;

            delete.RowIndex = RowIndex;
            if (IsChecked == true)
            {
                if (!DeleteList.Contains(delete, new DeleteRefComparer()))
                    DeleteList.Add(delete);
            }
            IsChecked = true;
            //ManageGroupAction();
        }
        /// <summary>
        /// Affiche/Cache le menu des action de groupe
        /// </summary>

        private void OnUnChecked(object sender, RoutedEventArgs e)
        {
            DeleteRef delete = new DeleteRef();
            delete.CanBeDelete = true;

            var row = sender as DataGridRow;
            Vente = row.Item as ClassL.Vente;
            RowIndex = Vente.Id;

            delete.RowIndex = RowIndex;
            if (DeleteList.Count > 0)
            {
                try
                {
                    var d = (from del in DeleteList where del.RowIndex == delete.RowIndex select del).Single();
                    int deleteindex = DeleteList.IndexOf(d);
                    DeleteList.RemoveAt(deleteindex);
                }
                catch (Exception)
                {

                }
            }

            //ManageGroupAction();
            IsChecked = false;
        }

        private void ManageGroupAction()
        {
            if (DeleteList.Count > 1)
                GroupAction.Visibility = Visibility.Visible;
            else
                GroupAction.Visibility = Visibility.Hidden;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var row = sender as DataGridRow;
            //Vente = row.Item as ClassL.Vente;
            //RowIndex = Vente.Id;
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

        private void OnRowSelected(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;
            Vente = row.Item as ClassL.Vente;
            RowIndex = Vente.Id;
            //row.Foreground = new SolidColorBrush(Colors.Green);
        }

        private void OnCellSellected(object sender, RoutedEventArgs e)
        {
            //var cell = sender as DataGridCell;
            //cell.Background = "{DynamicResource SecondaryHueLightBrush}"
            //cell.Background= new SolidColorBrush(ColorZoneMode.Accent);
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            var pnl = sender as StackPanel;
            var btns = pnl.Children;
            foreach (var item in btns)
            {
                var btn = item as Button;
                btn.Visibility = Visibility.Visible;
            }
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            var pnl = sender as StackPanel;
            var btns = pnl.Children;
            foreach (var item in btns)
            {
                var btn = item as Button;
                btn.Visibility = Visibility.Hidden;
            }
        }

        private void DeleteVente()
        {
            foreach (var item in DeleteList)
            {
                var vente = DbManager.GetById<Vente>(item.RowIndex);
                if (!InfoChecker.IsMonthOld(Vente.Date))
                {
                    MessageBox.Show(Properties.Resources.CmdTimeError, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (Delete(item.RowIndex))
                    {
                        MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                        ManageGroupAction();
                    }
                    else
                        MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            DeleteList.Clear();
        }

        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            if (!InfoChecker.IsMonthOld(Vente.Date))
            {
                MessageBox.Show(Properties.Resources.CmdTimeError, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                if (Delete(RowIndex))
                {
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    DeleteList.Clear();
                    ManageGroupAction();
                }
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnedit_Click(object sender, RoutedEventArgs e)
        {
            //Client client = new Client(RowIndex);
            //client.ShowDialog();
            //Clients = GetClient();
            //SetData(Clients);
        }

        private List<ClassL.Vente> Search(string critere)
        {
            var rslt = from c in GetVentes where c.UserName.Contains(critere) || c.Id.ToString().Contains(critere) || c.Date.ToShortDateString().Contains(critere) || c.Montant.ToString().Contains(critere) || c.Montant.ToString().Contains(critere) select c;
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

        private void all_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void all_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void Research()
        {
            var rslt = Search(TxtSearch.Text);
            if (rslt == null)
                ResetList();
            if (rslt.Count > 0)
            {
                Ventes = null;
                Ventes = rslt;
                SetData(Ventes);
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
                ResetList();
                return;
            }

            Ventes = null;
            Ventes = (rslt);
            SetData(Ventes);

        }

        private List<ClassL.Vente> SingleSearch(string critere)
        {
            try
            {
                var rslt = from c in GetVentes where c.UserName.Contains(critere) || c.Id.ToString() == (critere) || c.Date.ToString() == (critere) || c.Montant.ToString() == critere select c;
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
            Ventes = GetAllVente(DateDebut,DateFin);
            SetData(Ventes);
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
        private bool Delete(int id)
        {
            var lst = DbManager.GetAllProductByCmd(id);
            foreach (var item in lst)
            {
                DeleteProduitsVendu(item.Id);
            }
            bool rslt = DbManager.Delete<ClassL.VenteCredit>(id);
            return rslt;
        }

        private bool DeleteProduitsVendu(int id)
        {
            bool rslt = DbManager.Delete<ProduitVendu>(id);
            return rslt;
        }

        private void GroupDelete(object sender, RoutedEventArgs e)
        {
            foreach (var item in DeleteList)
            {
                Delete(item.RowIndex);
            }
            MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            ResetList();
            DeleteList.Clear();
            ManageGroupAction();
        }

        private void btnINFO_Click(object sender, RoutedEventArgs e)
        {
            ListeProduitVendu listeProduit = new ListeProduitVendu(RowIndex);
            listeProduit.ShowDialog();

        }

        private void Allventecredit_OnClick(object sender, RoutedEventArgs e)
        {
            Ventes = null;
            Ventes = DbManager.GetAll<ClassL.Vente>();
            SetData(Ventes);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            

            SetData(GetAllVente(DateDebut,DateFin));
            btnSearch.IsEnabled = true;
            ChkFilter.IsChecked = false;
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.ModuleListeVente;
                GetHome.MainTitle.Text = Properties.Resources.ModuleListeVente;
            }

        }


        public void CalendarDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            Calendar.SelectedDate = ((VenteListeUCViewModel)DataContext).DatePicker.Date;
        }

        public void CalendarDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!Calendar.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

            ((VenteListeUCViewModel)DataContext).DatePicker.Date = Calendar.SelectedDate.Value;

            //update list on date change
            DateDebut = InfoChecker.AjustDate(((VenteListeUCViewModel)DataContext).DatePicker.Date);
            GetVentes = GetAllVente(DateDebut, DateFin);
            Ventes = GetAllVente(DateDebut, DateFin);
            SetData(Ventes);
        }

        public void CalendarDialogOpenedEventHandlerF(object sender, DialogOpenedEventArgs eventArgs)
        {
            CalendarF.SelectedDate = ((VenteListeUCViewModel)DataContext).DatePicker.DateF;
        }

        public void CalendarDialogClosingEventHandlerF(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!CalendarF.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

            ((VenteListeUCViewModel)DataContext).DatePicker.DateF = CalendarF.SelectedDate.Value;
            
            //update list on date change
            DateFin = InfoChecker.AjustDate(((VenteListeUCViewModel)DataContext).DatePicker.DateF);
            GetVentes = GetAllVente(DateDebut, DateFin);
            Ventes = GetAllVente(DateDebut, DateFin);
            SetData(Ventes);
        }

        private bool GenVenteList(List<ClassL.Vente> ventes)
        {
            GetCompany();
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
            
            office.Code = Properties.Resources.Code;
            office.CommandeClient = Properties.Resources.CommandeClient;
            office.Utilisateur = Properties.Resources.User;
            office.Date = Properties.Resources.Date;
            office.Montant = Properties.Resources.Montant;
            office.ListeVente = Properties.Resources.ModuleListeVente;
            office.ListVente = ventes;
            office.At = Properties.Resources.At;
            office.Somme = Somme;
            office.Total = Properties.Resources.Total;
            office.Periode = $"{Properties.Resources.FactureDU} {InfoChecker.AjustDateWithDMY(Convert.ToDateTime(DateDebut))}  {Properties.Resources.Au} {InfoChecker.AjustDateWithDMY(Convert.ToDateTime(DateFin))} ";
            SaveLocation = InfoChecker.SaveLoc(Properties.Resources.ProductList, Properties.Resources.ModuleVente);
            return office.GenListeVente(System.IO.Path.GetFullPath("Files\\ListeVente_Prototype.dotx"), SaveLocation);
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
            var rslt = GetFactureHeader();
            if (rslt == null)
            {
                Company = null;
            }
            else
            {
                var company = DbManager.GetAll<CompanyInfo>();
                if (company.Count > 0)
                {
                    if (rslt.Data == "InvoiceOne")
                    {
                        Company = company[0];
                    }
                    else if (rslt.Data == "InvoiceTwo")
                    {
                        Company = company[1];
                    }
                    else
                    {
                        Company = company[2];
                    }
                }
                else
                {
                    Company = null;
                }

            }


        }

        private Settings GetFactureHeader()
        {
            var query = "SELECT * FROM  Settings WHERE Name='Invoice'";
            var rslt = DbManager.CustumQuery<Settings>(query);

            if (rslt.Count == 0)
                return null;
            else
                return rslt.FirstOrDefault();
        }

        private async Task PrintAsync()
        {
            await Task.Run(() => PrintRecu());
        }

        private async Task<bool> GenVenteListAsync(List<ClassL.Vente> ventes)
        {
            return await Task.Run(() => GenVenteList(ventes));
        }

        private void PrintRecu() 
        {
            GetTva();
            GetCompany();
            //Gen facture
            var venteid = DeleteList[0].RowIndex;
            var ventee = vente(venteid);
            //Set tva
            if (Tva != null)
            {
                if (Tva.Apply)
                    SommeTva = (ventee.Montant * Tva.Taux) / 100;
            }
            SommeTtc = SommeTva + ventee.Montant;
            IsCommand = ventee.CommandeId.HasValue && ventee.CommandeId != 0;
            var client = new ClassL.Client();
            string clientname;
            if (ventee.ClientId.HasValue && ventee.ClientId != 0)
            {
                client = DbManager.GetClientById(ventee.ClientId.Value);
                if (client != null)
                    clientname = $"{client.Nom} {client.Prenom}";
                else
                {
                    if (IsCommand)
                    {
                        GetVenteCredit = DbManager.GetById<VenteCredit>(ventee.CommandeId.GetValueOrDefault());
                        client = DbManager.GetById<ClassL.Client>(GetVenteCredit.ClientId);
                        clientname = $"{client.Nom} {client.Prenom}";
                    }
                    else
                        clientname = $"Vente-{ventee.Id}";
                }
            }
            else
            {
                if (IsCommand)
                {
                    GetVenteCredit = DbManager.GetById<VenteCredit>(ventee.CommandeId.GetValueOrDefault());
                    client = DbManager.GetById<ClassL.Client>(GetVenteCredit.ClientId);
                    clientname = $"{client.Nom} {client.Prenom}";
                }
                else
                    clientname = $"Vente-{ventee.Id}";
            }
            

            SaveLocation = InfoChecker.SaveLoc(clientname, Properties.Resources.TypeFacture);
            //Genarate bill
            Generated = Recu(client, ventee, produitVendus(venteid));
        }

        private async void Btnprint_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            GetHome.DataContextt.Progress = Visibility.Visible;
            if (DeleteList.Count == 1)
            {
                await PrintAsync();
                GetHome.DataContextt.Progress = Visibility.Hidden;
                if (Generated)
                {               
                    showDocument = new ShowDocument(SaveLocation);
                    watch.Stop();
                    Console.WriteLine($"premier arret: {watch.ElapsedMilliseconds}");
                    showDocument.ShowDialog();
                }
                else
                {
                    GetHome.DataContextt.Progress = Visibility.Hidden;
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                //Gen liste des ventes
                if (Ventes.Count == 0)
                    Ventes = GetAllVente(DateDebut, DateFin);
                if (Ventes.Count == 0)
                {
                    GetHome.DataContextt.Progress = Visibility.Hidden;
                    MessageBox.Show(Properties.Resources.EmptyList, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (await GenVenteListAsync(Ventes))
                {
                    GetHome.DataContextt.Progress = Visibility.Hidden;
                    showDocument = new ShowDocument(SaveLocation);
                    watch.Stop();
                    Console.WriteLine($"premier arret: {watch.ElapsedMilliseconds}");
                    showDocument.ShowDialog();
                }
                else
                {
                    GetHome.DataContextt.Progress = Visibility.Hidden;
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        public bool Recu(ClassL.Client client, ClassL.Vente vente, List<ProduitVendu> vendu)
        {
            Office office = new Office();

            //Company info
            if (Company != null)
            {
                office.CompanyName = Company.Nom;
                office.CompanyTel = $"Tel: {Company.Contact}";
                office.CompanyEmail = $"Email: {Company.Email}";
                office.Consigne = $"NB: {Company.Consigne}";
            }
            else
            {
                office.CompanyName = "EasyManager";
                office.CompanyTel = "Tel: +228 00 00 00 00";
                office.CompanyEmail = "Email: elianosetekpo@gmail.com";
            }
            office.Code = Properties.Resources.Code;
            office.Designation = Properties.Resources.Designation;
            office.PrixUnitaire = Properties.Resources.Price;
            office.Montant = Properties.Resources.Montant;
            office.Discount = Properties.Resources.Discount;
            office.Quantite = Properties.Resources.Quantite;
            office.FactureDU = Properties.Resources.FactureDU;
            office.FactureNO = Properties.Resources.FactureNO;
            office.Client = Properties.Resources.Client;
            office.Total = Properties.Resources.Total;
            office.At = Properties.Resources.At;
            office.Rembourssement = Properties.Resources.Rembourssement;
            office.ResteAPayer = Properties.Resources.ResteAPayer;
            office.TypeFacture = Properties.Resources.TypeFacture;
            office.Arrete = Properties.Resources.Arrete;
            office.GetClient = client;
            office.GetVente = vente;
            office.ProduitVendus = vendu;
            office.GetVenteCredit = GetVenteCredit;
            office.tva = Properties.Resources.TVA;
            office.Total = Properties.Resources.HT;
            office.TotalTTC = Properties.Resources.TTC;
            office.DiscountValue = vente.ValueDiscount.ToString();
            office.IsRecall = true;
            office.IsCommand = IsCommand;

            var rslt = GetFactureHeader();
            if (rslt == null)
                office.LogoPath = GetShopLogo();
            else
            {
                if (rslt.Data == "InvoiceOne")
                {
                    office.LogoPath = GetShopLogo();
                }
                else if (rslt.Data == "InvoiceTwo")
                {
                    office.LogoPath = GetShopLogoTwo();
                }
                else
                {
                    office.LogoPath = GetShopLogoThree();
                }
            }

            //verifie si la tva doit-être appliquer
            if (Tva != null)
            {
                if (Tva.Apply == true)
                {
                    office.tauxtva = $"{Tva.Taux} %";
                }
            }
            office.SommeTva = SommeTva;
            office.SommeTtc = SommeTtc;
            office.MontantTotalCredit = Properties.Resources.MontantTotalCredit;

            var facturesettings = GetFactureStyle();
            string facture = "";

            if (facturesettings == null)
            {
                // not set
                facture = "FactureVert";
            }
            else
            {
                facture = facturesettings.Data;
            }

            if (office.GenFactureNew(System.IO.Path.GetFullPath("Files\\"+facture+".dotx"), SaveLocation))
            {
                //PerformClick(btndialogclose);
                return true;
            }
            else
                return false;
        }

        private Settings GetFactureStyle()
        {
            var query = "SELECT * FROM  Settings WHERE Name='FactureStyle'";
            var rslt = DbManager.CustumQuery<Settings>(query);

            if (rslt.Count == 0)
                return null;
            else
                return rslt.FirstOrDefault();
        }

        private string GetShopLogo()
        {
            List<ShopLogo> logos = new List<ShopLogo>();
            logos = DbManager.GetAll<ShopLogo>();
            if (logos.Count() == 0)
            {
                // there is not data in the table
                // set the default logo
                return ((InfoChecker.ShopLogoDefault()));
            }
            else
            {
                //get the last record
                var logo = logos.LastOrDefault();
                return ((InfoChecker.SetShopLogoPath(logo.Name)));
            }
        }
        private ClassL.Vente vente(int id)
        {
            return DbManager.GetById<ClassL.Vente>(id);
        }

        private List<ProduitVendu> produitVendus(int venteId)
        {
            var lst= DbManager.GetByColumnName<ProduitVendu>("VenteId", venteId.ToString());
            var rslt = new List<ProduitVendu>();
            foreach(var item in lst)
            {
                item.SetProduit(DbManager.GetById<ClassL.Produit>(item.ProduitId));
                rslt.Add(item);
            }
            return rslt;
        }

        private void btndeletechk_Click(object sender, RoutedEventArgs e)
        {
            DeleteVente();
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
                if (CSVManager.WriteDatas<ClassL.Vente>(SavePath, Ventes))
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetShopLogoTwo()
        {
            var settings = GetSecondLogo();
            if (settings == null)
            {
                // there is not data in the table
                // set the default logo
                return InfoChecker.ShopLogoDefault();
            }
            else
            {
                //get the last record
                return InfoChecker.SetShopLogoPath(settings.Data);
            }
        }

        private string GetShopLogoThree()
        {
            var settings = GetThirdLogo();
            if (settings == null)
            {
                // there is not data in the table
                // set the default logo
                return InfoChecker.ShopLogoDefault();
            }
            else
            {
                //get the last record
                return InfoChecker.SetShopLogoPath(settings.Data);
            }
        }

        private Settings GetSecondLogo()
        {
            var query = "SELECT * FROM  Settings WHERE Name='SecondLogo'";
            var rslt = DbManager.CustumQuery<Settings>(query);

            if (rslt.Count == 0)
                return null;
            else
                return rslt.FirstOrDefault();
        }

        private Settings GetThirdLogo()
        {
            var query = "SELECT * FROM  Settings WHERE Name='ThirdLogo'";
            var rslt = DbManager.CustumQuery<Settings>(query);

            if (rslt.Count == 0)
                return null;
            else
                return rslt.FirstOrDefault();
        }
    }

    public class VenteListeUCViewModel
    {
        public DatePickerViewModel DatePicker { get; set; }
        public VenteListeUC VenteListeUC { get; set; }
    }
}
