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
using EasyManager.MenuItems;
using EasyManagerDb;
using EasyManagerLibrary;
using MaterialDesignThemes.Wpf;
using ClassL = EasyManagerLibrary;


namespace EasyManager
{
    /// <summary>
    /// Interaction logic for VenteCreditListeUC.xaml
    /// </summary>
    public partial class VenteCreditListeUC : UserControl
    {
        #region Variable declaration
        private List<VenteCredit> Commandes { get; set; }
        /// <summary>
        /// Liste des commandes à supprimer
        /// </summary>
        private List<DeleteRef> DeleteList { get; set; } = new List<DeleteRef>();
        /// <summary>
        /// Liste des commandes dans la base de données
        /// </summary>
        public List<VenteCredit> GetCommandes { get; set; }
        /// <summary>
        /// Contient les données d'un row
        /// </summary>
        public VenteCredit VenteCredit { get; set; }
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
        public decimal Reste { get; set; } = 0;
        public bool ApplyTVA { get; set; } = false;
        public decimal SommeTva { get; set; }
        public decimal SommeTtc { get; set; }
        public Tva Tva { get; set; } = new Tva();
        public CompanyInfo Company { get; set; } = new CompanyInfo();
        public bool Generated { get; set; }
        public bool IsChecked { get; set; } = true;


        #endregion
        public VenteCreditListeUC()
        {
            InitializeComponent();
            GetCommandes = GetAllCommande();
            Commandes = GetAllCommande();
        }


        public VenteCreditListeUC(Home h)
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



            var datacontext = new VenteCreditListeUCViewModel();
            datacontext.VenteCreditUC = this;
            datacontext.DatePicker = new DatePickerViewModel();
            DataContext = datacontext;

            ((VenteCreditListeUCViewModel)DataContext).DatePicker.Date = Convert.ToDateTime(DateDebut);
            ((VenteCreditListeUCViewModel)DataContext).DatePicker.DateF = Convert.ToDateTime(DateFin);

            GetCommandes = GetAllCommande(DateDebut, DateFin);
            Commandes = GetAllCommande(DateDebut, DateFin);
            GetHome.SizeChanged += GetHome_SizeChanged;
        }

        private void GetHome_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Datagrid.Width = GetHome.ActualWidth - 50;
        }

        /// <summary>
        /// Génère un objet <see cref="VenteCredit"/> pour des tests
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private ClassL.VenteCredit commande(int i)
        {
            ClassL.VenteCredit c = new ClassL.VenteCredit();
            c.Id = i;
            c.ClientId = i;
            //var client = Client(i);
            //c.ClientName = $"Prenom{i} Nom{i}";
            c.Montant = 123 * i;
            c.MontantRestant = i;
            c.UtilisateurId = i;
            //c.NomUtilisateur= $"Prenom{i} Nom{i}";
            c.Date = DateTime.UtcNow;
            return c;
        }

        private ClassL.Client GetClient(int id)
        {
            return DbManager.GetById<ClassL.Client>(id);
        }

        private ClassL.Utilisateur GetUtilisateur(int id)
        {
            return DbManager.GetById<Utilisateur>(id);
        }

        /// <summary>
        /// Génère une liste avec un nombre d'élément spécifié pour simuler un grand volume de données
        /// </summary>
        /// <returns></returns>

        private List<ClassL.VenteCredit> TestList()
        {
            List<ClassL.VenteCredit> l = new List<ClassL.VenteCredit>();
            for (int i = 1; i < 1000; i++)
            {
                l.Add(commande(i));
            }
            return l;
        }
        /// <summary>
        /// Retourne toutes les commandes de la base de données
        /// </summary>
        /// <returns></returns>

        private List<VenteCredit> GetAllCommande()
        {
            var rslt = DbManager.GetAll<VenteCredit>();
            var lst = new List<VenteCredit>();
            foreach (var item in rslt)
            {
                var client = GetClient(item.ClientId);
                var user = GetUtilisateur(item.UtilisateurId);
                item.SetClient(client);
                item.SetUser(user);
                lst.Add(item);
            }
            return lst;
        }

        private List<VenteCredit> GetAllCommande(string datedebut, string datefin)
        {
            var rslt = DbManager.GetDataByDate<VenteCredit>("Date", datedebut, datefin);
            var lst = new List<VenteCredit>();
            lst.Clear();
            Somme = 0;
            Reste = 0;
            foreach (var item in rslt)
            {
                var client = GetClient(item.ClientId);
                Somme += item.Montant;
                Reste += item.MontantRestant;
                var user = GetUtilisateur(item.UtilisateurId);
                item.SetClient(client);
                item.SetUser(user);
                lst.Add(item);
            }
            return lst;
        }
        /// <summary>
        /// Spécifie la source de données du <see cref="DataGrid"/>
        /// </summary>
        /// <param name="commande"></param>

        private void SetData(List<ClassL.VenteCredit> commande)
        {
            Datagrid.ItemsSource = commande;
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            /*var datagridcell = sender as DataGridCell;
            var chk = datagridcell.Content as CheckBox;*/
            //Checked

            DeleteRef delete = new DeleteRef();
            delete.CanBeDelete = true;

            var row = sender as DataGridRow;
            VenteCredit = row.Item as ClassL.VenteCredit;
            RowIndex = VenteCredit.Id;

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
            VenteCredit = row.Item as ClassL.VenteCredit;
            RowIndex = VenteCredit.Id;

            delete.RowIndex = RowIndex;
            if (DeleteList.Count > 0)
            {
                var d = (from del in DeleteList where del.RowIndex == delete.RowIndex select del).Single();
                int deleteindex = DeleteList.IndexOf(d);
                DeleteList.RemoveAt(deleteindex);
            }

            //ManageGroupAction();
            IsChecked = false;
        }

        private void ManageGroupAction()
        {
            //if (DeleteList.Count > 1)
            //    GroupAction.Visibility = Visibility.Visible;
            //else
            //    GroupAction.Visibility = Visibility.Hidden;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var row = sender as DataGridRow;
            VenteCredit = row.Item as ClassL.VenteCredit;
            RowIndex = VenteCredit.Id;
            row.Background =  new SolidColorBrush(Colors.LightGray);
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
            //var row = sender as DataGridRow;
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

        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            if (!InfoChecker.IsMonthOld(VenteCredit.Date))
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

        private List<VenteCredit> Search(string critere)
        {
            var rslt = from c in GetCommandes where c.NomClient.Contains(critere) || c.UserName.Contains(critere) || c.Id.ToString().Contains(critere) || c.Date.ToShortDateString().Contains(critere) || c.Montant.ToString().Contains(critere) || c.MontantRestant.ToString().Contains(critere) || c.GetSolde().Contains(critere) select c;
            var lst = new List<VenteCredit>();
            Somme = 0;
            Reste = 0;
            foreach (var item in rslt)
            {
                Somme += item.Montant;
                Reste += item.MontantRestant;
                lst.Add(item);
            }
            return lst;
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
                Commandes = null;
                Commandes = rslt;
                SetData(Commandes);
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

            Commandes = null;
            Commandes = (rslt);
            SetData(Commandes);

        }

        private List<VenteCredit> SingleSearch(string critere)
        {
            try
            {
                var rslt = from c in GetCommandes where c.UserName.Contains(critere) || c.NomClient.Contains(critere) || c.Id.ToString() == (critere) || c.Date.ToString() == (critere) || c.Montant.ToString() == critere || c.MontantRestant.ToString() == critere || c.GetSolde() == critere select c;
                Somme = 0;
                Reste = 0;
                var lst = new List<VenteCredit>();
                foreach (var item in rslt)
                {
                    Somme += item.Montant;
                    Reste += item.MontantRestant;
                    lst.Add(item);
                }
                return lst;
            }
            catch
            {
                return null;
            }
        }

        private void ResetList()
        {
            //Clients = GetClient();
            Commandes = GetAllCommande(DateDebut, DateFin);
            SetData(Commandes);
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
        /// <summary>
        /// Suprime une commande par son Id
        /// </summary>
        /// <param name="id">Id du commande</param>
        /// <returns></returns>
        private bool Delete(int id)
        {
            var lst = DbManager.GetAllProductByCmd(id);
            foreach (var item in lst)
            {
                DeleteProduitsCmd(item.Id);
            }
            bool rslt = DbManager.Delete<ClassL.VenteCredit>(id);
            return rslt;
        }

        private void DeleteVenteCredit()
        {
            foreach (var item in DeleteList)
            {
                var ventecredit = DbManager.GetById<VenteCredit>(item.RowIndex);
                if (!InfoChecker.IsMonthOld(ventecredit.Date))
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

        private bool DeleteProduitsCmd(int id)
        {
            bool rslt = DbManager.Delete<ProduitCredit>(id);
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
            ListeProduitCommende listeProduit = new ListeProduitCommende(RowIndex);
            listeProduit.ShowDialog();

        }

        private void Allventecredit_OnClick(object sender, RoutedEventArgs e)
        {
            Commandes = null;
            var rslt = DbManager.GetAll<VenteCredit>();
            var lst = new List<VenteCredit>();
            Somme = 0;
            Reste = 0;
            foreach (var item in rslt)
            {
                Somme += item.Montant;
                Reste += item.MontantRestant;
                var client = GetClient(item.ClientId);
                var user = GetUtilisateur(item.UtilisateurId);
                item.SetClient(client);
                item.SetUser(user);
                lst.Add(item);
            }
            Commandes = lst;
            SetData(Commandes);
        }

        private void Ventecreditsolde_OnClick(object sender, RoutedEventArgs e)
        {
            Commandes = null;
            var rslt = Servante.VenteCreditSolde();
            var lst = new List<VenteCredit>();
            Somme = 0;
            Reste = 0;
            foreach (var item in rslt)
            {
                Somme += item.Montant;
                Reste += item.MontantRestant;
                var client = GetClient(item.ClientId);
                var user = GetUtilisateur(item.UtilisateurId);
                item.SetClient(client);
                item.SetUser(user);
                lst.Add(item);
            }
            Commandes = lst;
            SetData(Commandes);
        }

        private void Ventecreditnonsolde_OnClick(object sender, RoutedEventArgs e)
        {

            Commandes = null;
            var rslt = Servante.VenteCreditNonSolde();
            var lst = new List<VenteCredit>();
            Somme = 0;
            Reste = 0;
            foreach (var item in rslt)
            {
                Somme += item.Montant;
                Reste += item.MontantRestant;
                var client = GetClient(item.ClientId);
                var user = GetUtilisateur(item.UtilisateurId);
                item.SetClient(client);
                item.SetUser(user);
                lst.Add(item);
            }
            Commandes = lst;
            SetData(Commandes);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetData(GetAllCommande(DateDebut, DateFin));
            btnSearch.IsEnabled = true;
            ChkFilter.IsChecked = false;
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.ListVenteCredit;
                GetHome.MainTitle.Text = Properties.Resources.ListVenteCredit;
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = sender as VenteCreditListeUC;
            var ah = window.ActualHeight;
            Datagrid.Width = (GetHome.ActualWidth) - 27;
            Datagrid.Height = ah - ah / 3;
        }

        public void CalendarDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            Calendar.SelectedDate = ((VenteCreditListeUCViewModel)DataContext).DatePicker.Date;
        }

        public void CalendarDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!Calendar.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

            ((VenteCreditListeUCViewModel)DataContext).DatePicker.Date = Calendar.SelectedDate.Value;

            //update list on date change
            DateDebut = InfoChecker.AjustDate(((VenteCreditListeUCViewModel)DataContext).DatePicker.Date);
            Commandes = GetAllCommande(DateDebut, DateFin);
            SetData(Commandes);
        }

        public void CalendarDialogOpenedEventHandlerF(object sender, DialogOpenedEventArgs eventArgs)
        {
            CalendarF.SelectedDate = ((VenteCreditListeUCViewModel)DataContext).DatePicker.DateF;
        }

        public void CalendarDialogClosingEventHandlerF(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!CalendarF.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

            ((VenteCreditListeUCViewModel)DataContext).DatePicker.DateF = CalendarF.SelectedDate.Value;

            //update list on date change
            DateFin = InfoChecker.AjustDate(((VenteCreditListeUCViewModel)DataContext).DatePicker.DateF);
            Commandes = GetAllCommande(DateDebut, DateFin);
            SetData(Commandes);
        }

        private bool GenVenteCreditList(List<VenteCredit> ventecredits)
        {
            Office office = new Office();
            office.CompanyName = "DeLaFlor Corporation";
            office.CompanyTel = "Tel:+228 99 34 12 11";
            office.CompanyEmail = "Email:delaflor@flor.com";
            office.Code = Properties.Resources.Code;
            office.Client = Properties.Resources.Client;
            office.Utilisateur = Properties.Resources.User;
            office.Date = Properties.Resources.Date;
            office.Montant = Properties.Resources.Montant;
            office.ListeVenteCredit = Properties.Resources.ListVenteCredit;
            office.ListVenteCredit = ventecredits;
            office.At = Properties.Resources.At;
            office.Somme = Somme;
            office.SommeRestant = Reste;
            office.Montant = Properties.Resources.Montant;
            office.MontantRestante = Properties.Resources.MontantRestant;
            office.Solde = Properties.Resources.Soldee;
            office.Total = Properties.Resources.Total;
            office.Periode = $"{Properties.Resources.FactureDU} {InfoChecker.AjustDateWithDMY(Convert.ToDateTime(DateDebut))}  {Properties.Resources.Au} {InfoChecker.AjustDateWithDMY(Convert.ToDateTime(DateFin))} ";
            SaveLocation = InfoChecker.SaveLoc(Properties.Resources.VenteCredit + Properties.Resources.Liste, Properties.Resources.VenteCredit);
            return office.GenListeVenteCredit(System.IO.Path.GetFullPath("Files\\ListeVenteCredit_Prototype.dotx"), SaveLocation);
        }

        private async Task PrintAsync()
        {
            await Task.Run(() => PrintRecu());
        }

        private async void btnprint_Click(object sender, RoutedEventArgs e)
        {
            GetHome.DataContextt.Progress = Visibility.Visible;
            if (DeleteList.Count == 1)
            {
                await PrintAsync();
                GetHome.DataContextt.Progress = Visibility.Hidden;
                if (Generated)
                {
                    showDocument = new ShowDocument(SaveLocation);
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
                //Gen liste des vente à credits
                if (Commandes.Count == 0)
                    Commandes = GetAllCommande(DateDebut, DateFin);
                if (Commandes.Count == 0)
                {
                    GetHome.DataContextt.Progress = Visibility.Hidden;
                    MessageBox.Show(Properties.Resources.EmptyList, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (await GenVenteCreditListAsync(Commandes))
                {
                    GetHome.DataContextt.Progress = Visibility.Hidden;
                    showDocument = new ShowDocument(SaveLocation);
                    showDocument.ShowDialog();
                }
                else
                {
                    GetHome.DataContextt.Progress = Visibility.Hidden;
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private async Task<bool> GenVenteCreditListAsync(List<VenteCredit> ventecredits)
        {
            return await Task.Run(() => GenVenteCreditList(ventecredits));
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

        private VenteCredit venteCredit(int id)
        {
            return DbManager.GetById<ClassL.VenteCredit>(id);
        }

        private List<ProduitCredit> produitcredit(int ventecreditId)
        {
            var lst = DbManager.GetByColumnName<ProduitCredit>("CommandeId", ventecreditId.ToString());
            var rslt = new List<ProduitCredit>();
            foreach (var item in lst)
            {
                item.SetProduit(DbManager.GetById<ClassL.Produit>(item.ProduitId));
                rslt.Add(item);
            }
            return rslt;
        }

        private void PrintRecu()
        {
            //Gen facture


            GetTva();
            GetCompany();

            var ventecreditid = DeleteList[0].RowIndex;
            var ventecredit = venteCredit(ventecreditid);
            //Set tva
            if (Tva != null)
            {
                if (Tva.Apply)
                    SommeTva = (ventecredit.Montant * Tva.Taux) / 100;
            }
            SommeTtc = SommeTva + ventecredit.Montant;
            var client = new ClassL.Client();
            string clientname;
            if (ventecredit.ClientId != 0)
            {
                client = DbManager.GetClientById(ventecredit.ClientId);
                clientname = $"{client.Nom} {client.Prenom}";
            }
            else
            {
                client = null;
                clientname = $"{Properties.Resources.VenteCredit}-{ventecredit.Id}";
            }

            SaveLocation = InfoChecker.SaveLoc(clientname, Properties.Resources.TypeCommande);
            //Genarate bill
            Generated = Recu(client, ventecredit, produitcredit(ventecreditid));
        }

        public bool Recu(ClassL.Client client, VenteCredit venteCredit, List<ProduitCredit> vendu)
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
            office.TypeFacture = Properties.Resources.TypeCommande;
            office.Arrete = Properties.Resources.ArreteCmd;
            office.GetClient = client;
            office.GetVenteCredit = venteCredit;
            office.ProduitCommandes = vendu;
            office.tva = Properties.Resources.TVA;
            office.Total = Properties.Resources.HT;
            office.TotalTTC = Properties.Resources.TTC;
            office.DiscountValue = venteCredit.ValueDiscount.ToString();
            office.IsRecall = true;
            office.LogoPath = GetShopLogo();

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

            if (office.GenFactureNew(System.IO.Path.GetFullPath("Files\\Facture.dotx"), SaveLocation, 'C'))
            {
                //PerformClick(btndialogclose);
                return true;
            }
            else
                return false;
        }

        private void btnDelLst_Click(object sender, RoutedEventArgs e)
        {
            DeleteVenteCredit();
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
                if (CSVManager.WriteDatas<ClassL.VenteCredit>(SavePath, Commandes))
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }



    public class VenteCreditListeUCViewModel
    {
        public DatePickerViewModel DatePicker { get; set; }
        public VenteCreditListeUC VenteCreditUC { get; set; }
    }
}
