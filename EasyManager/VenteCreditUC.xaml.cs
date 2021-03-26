using EasyManager.Commands;
using EasyManagerDb;
using EasyManagerLibrary;
using MaterialDesignThemes.Wpf;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for VenteCreditUC.xaml
    /// </summary>
    public partial class VenteCreditUC : UserControl, INotifyPropertyChanged
    {
        #region Variable declararion
        private string _montantt;
        private decimal _sommetva;
        private decimal _sommettc;

        List<ProduitCredit> lst;
        public List<string> ProduitList { get; set; } = new List<string>();
        public List<ProduitCredit> ListPC { get; set; } = new List<ProduitCredit>();
        public List<ProduitCredit> ListPCommandes { get; set; } = new List<ProduitCredit>();
        public EasyManagerLibrary.Client SelectedClient { get; set; }
        public EasyManagerLibrary.Produit SelectedProduct { get; set; }
        public bool CanBeDeleted { get; set; } = true;
        public ComboBox CbFocusedId { get; set; } = null;
        public TextBox TxtFocusedId { get; set; } = null;
        public bool QuantityError { get; set; }
        public Home GetHome { get; set; }
        public bool CanBePrint { get; set; } = true;
        public ShowDocument showDocument { get; set; }
        public string SaveLocation { get; set; }
        public bool Generated { get; set; }
        public bool ApplyTVA { get; set; } = false;
        public decimal SommeTva 
        { 
            get=>_sommetva;
            set
            {
                if (_sommetva == value)
                    return;
                _sommetva = value;
                OnPropertyChanged();
            } 
        }
        public decimal SommeTtc 
        { 
            get=>_sommettc;
            set
            {
                if (_sommettc == value)
                    return;
                _sommettc = value;
                OnPropertyChanged();
            }
        }
        public Tva Tva { get; set; } = new Tva();
        public CompanyInfo Company { get; set; } = new CompanyInfo();
        public EasyManagerLibrary.Notifications GetNotifications { get; set; } = new EasyManagerLibrary.Notifications();
        DicountValueCommand DicountValueCommand;
        public List<ProduitCredit> ListPVC
        {
            get => lst;
            set
            {
                if (lst == value)
                    return;
                lst.AddRange(value);
                OnPropertyChanged();
            }
        }
        public bool IsDiscountValue { get; set; } = false;
        #endregion

        public string Montant
        {
            get => _montantt;
            set
            {
                if (_montantt == value)
                    return;
                _montantt = value;
                //ValidateAmount();
                OnPropertyChanged();
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public VenteCreditUC()
        {
            InitializeComponent();
        }

        public VenteCreditUC(Home h)
        {
            InitializeComponent();
            GetHome = h;
            container.MinWidth = GetHome.ActualWidth - 500;
            GetHome.SizeChanged += GetHome_SizeChanged;
            //ManagerTable("ValueDiscount", "REAL", "0");
            //ManagerTable("Canceled", "INTEGER", "0");
            //ManagerTable("CanceledDate", "TEXT", "NULL");
            DicountValueCommand = new DicountValueCommand(this);
            DataContext = this;
        }

        public void ManagerTable(string columnName, string typ, string defaultvalue)
        {
            //check if the column already exist the table
            if (!DbManager.CustumQueryCheckColumn<EasyManagerLibrary.Vente>(columnName))
            {
                // the column is not in the table
                // so we have to create it
                DbManager.CreateNewColumn<EasyManagerLibrary.Vente>(columnName, typ, defaultvalue);
            }

        }

        private void GetHome_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            container.MinWidth =Math.Abs(GetHome.ActualWidth - 500);
        }

        private void AddNewLine()
        {
            var newrow = new RowDefinition();
            var rowcount = ProdGrid.RowDefinitions.Count;

            TextBox tb = new TextBox();
            tb.Name = $"txtprod{rowcount}";
            Style style = FindResource("MaterialDesignFloatingHintTextBox") as Style;
            tb.Style = style;
            tb.Margin = new Thickness(16);
            tb.FontSize = 20;
            tb.KeyUp += Tb_KeyUp;
            tb.LostKeyboardFocus += Tb_LostKeyboardFocus;
            tb.ToolTip = Properties.Resources.QuantiteVendu;
            HintAssist.SetHint(tb, Properties.Resources.QuantiteVendu);


            ComboBox cb = new ComboBox();
            cb.Name = $"cbprod{rowcount}";
            cb.ToolTip = Properties.Resources.SelectionProduit;
            cb.ItemsSource = ProduitList;
            cb.LostFocus += Cb_LostFocus;
            cb.IsEditable = true;
            HintAssist.SetHint(cb, Properties.Resources.MakeSelection);
            HintAssist.SetHintOpacity(cb, .26);
            cb.Margin = new Thickness(16);
            cb.FontSize = 20;
            cb.SelectionChanged += Cb_SelectionChanged;


            ProdGrid.RowDefinitions.Add(newrow);
            var lst = ProdGrid.RowDefinitions.Last();
            //rowcount = lst

            Grid.SetRow(cb, rowcount);
            Grid.SetColumn(cb, 0);
            Grid.SetRow(tb, rowcount);
            Grid.SetColumn(tb, 1);

            //var i = ProdGrid.Children.Count;
            ProdGrid.Children.Add(cb);
            ProdGrid.Children.Add(tb);
        }

        private void Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
            {
                SelectedProduct = null;
                return;
            }

            var item = e.AddedItems[0].ToString();
            if (item == Properties.Resources.MakeSelection)
                SelectedProduct = null;
            else
                SelectedProduct = SelectedProductId(item);
        }

        private void Cb_LostFocus(object sender, RoutedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            string cbid = cb.Name;
            if (cb.SelectedIndex == 0)
            {
                CbFocusedId = null;
                return;
            }

            CbFocusedId = cb;
            var prodname = cb.SelectedValue as string;
            if (prodname == null)
                return;
            // cb
            if (TxtFocusedId == null)
                return;
            var txt = TxtFocusedId.Text;
            if (string.IsNullOrWhiteSpace(txt))
                return;
            int qty = int.Parse(txt);
            if (!IsOkLeftQuantity(prodname, qty))
            {
                MessageBox.Show(Properties.Resources.QuantiteManquante, Properties.Resources.MainTitle,
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                TxtFocusedId.Focus();
                TxtFocusedId.SelectionStart = TxtFocusedId.Text.Length;
                //CbFocusedId = null;
            }
        }

        private void Tb_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string txtid = tb.Name;
            if (string.IsNullOrWhiteSpace(tb.Text))
                return;
            int qty = int.Parse(tb.Text);
            TxtFocusedId = tb;

            // txt
            if (CbFocusedId == null)
                return;
            var prodname = CbFocusedId.SelectedItem as string;
            if (prodname == null)
                return;
            if (!IsOkLeftQuantity(prodname, qty))
            {
                MessageBox.Show(Properties.Resources.QuantiteManquante, Properties.Resources.MainTitle,
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                //tb.Focus();
                //tb.SelectionStart = tb.Text.Length;
                TxtFocusedId = null;
            }
        }

        private void Tb_KeyUp(object sender, KeyEventArgs e)
        {
            OnlyNumber(sender as TextBox);
        }

        private void OnlyNumber(TextBox tb)
        {
            tb.Text = InfoChecker.NumericOnlyRegExp(tb.Text);
            tb.SelectionStart = tb.Text.Length;
        }

        private void RemoveLine()
        {
            var children = ProdGrid.Children;
            var childcnt = children.Count;
            if (childcnt == 2)
            {
                CanBeDeleted = false;
                return;
            }

            var txt = children[childcnt - 1];
            var cb = children[childcnt - 2];
            ProdGrid.Children.Remove(cb);
            ProdGrid.Children.Remove(txt);
            var last = ProdGrid.RowDefinitions.Last();
            ProdGrid.RowDefinitions.Remove(last);
        }

        private void btnadd_Click(object sender, RoutedEventArgs e)
        {
            AddNewLine();
        }

        private void btnminus_Click(object sender, RoutedEventArgs e)
        {
            RemoveLine();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
        }

        private async Task PrintAsync(EasyManagerLibrary.VenteCredit vente)
        {
            await Task.Run(() => Printer(vente));
        }

        private void SellResume()
        {
            ListPC.Clear();
            lst = new List<ProduitCredit>();
            ListPVC = new List<ProduitCredit>();
            lst.Clear();
            bool stop = false;
            CanBeDeleted = true;
            var cmd = new VenteCredit();
            cmd.Date = DateTime.UtcNow;
            if (SelectedClient == null)
            {
                //Une erreur est sur venue
                GetHome.DataContextt.Progress = Visibility.Hidden;
                MessageBox.Show(Properties.Resources.ForgetClient, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            cmd.ClientId = SelectedClient.Id;
            cmd.UtilisateurId = GetHome.DataContextt.ConnectedUser.Id;
            decimal sommevente = 0;
            SommeTva = 0;
            SommeTtc = 0;
            ProduitCredit vendu;
            List<bool> saverslt = new List<bool>();
            var children = ProdGrid.Children;
            for (int i = 0; i < children.Count; i += 2)
            {
                vendu = GetProduitCommandeSellResume(children[i], children[i + 1],cmd);
                if (vendu == null)
                {
                    stop = true;
                    break;
                }

                //Vérifie si le client bénéficie d'une réduction
                var clientdisc = Servante.ClientHasDiscount(cmd.ClientId);
                if (clientdisc.Item1 && clientdisc.Item3)
                {
                    //vérifie si le discount a expiré
                    if (InfoChecker.IsWithinDate(clientdisc.Item2.DateDebut, clientdisc.Item2.DateFin, DateTime.Now))
                    {
                        //le discount est valide
                        vendu.GetTotal(vendu.PrixUnitaire * (1 - clientdisc.Item2.Taux));
                        vendu.Discount = clientdisc.Item2.Taux;
                    }
                    else
                    {
                        //le discount a expiré

                        //Check if discount canceled staus must be update
                        if (InfoChecker.HasExpire(clientdisc.Item2.DateFin, DateTime.Now))
                        {
                            //update Discount canceled status
                            DbManager.UpdateDiscountCanceled(clientdisc.Item2.Id, true);
                        }
                        //Vérifie s'il y a un discount sur le produit
                        var disc = Servante.ProductHasDiscount(vendu.ProduitId);
                        if (disc.Item1 && disc.Item3)
                        {
                            vendu.GetTotal(vendu.PrixUnitaire * (1 - disc.Item2.Taux));
                            vendu.Discount = disc.Item2.Taux;
                        }
                        else
                        {
                            vendu.GetTotal();
                            vendu.Discount = 0;
                        }
                    }
                }
                else
                {
                    //si non le client n'en bénéficie pas
                    //Vérifie s'il y a un discount sur le produit
                    var disc = Servante.ProductHasDiscount(vendu.ProduitId);
                    if (disc.Item1 && disc.Item3)
                    {
                        vendu.GetTotal(vendu.PrixUnitaire * (1 - disc.Item2.Taux));
                        vendu.Discount = disc.Item2.Taux;
                    }
                    else
                    {
                        vendu.GetTotal();
                        vendu.Discount = 0;
                    }
                }


                sommevente += vendu.Montant;
                lst.Add(vendu);
            }
            ListPVC = lst;

            if (stop)
            {
                //Une erreur est sur venue
                if (QuantityError)
                    MessageBox.Show(Properties.Resources.QuantiteManquante, Properties.Resources.MainTitle,
                        MessageBoxButton.OK, MessageBoxImage.Stop);
                else
                    MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle,
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            //montant de la commande
            cmd.Montant = sommevente;
            //montant restant de la commande
            cmd.MontantRestant = sommevente;
            //montant tva
            if (Tva != null)
            {
                if (Tva.Apply)
                    SommeTva = (sommevente * Tva.Taux) / 100;
            }
            if (DicountValueCommand.CanExecute(Montant))
            {
                var disc = decimal.Parse(Montant);
                SommeTtc = (SommeTva + sommevente) - disc;
            }
            else
            {
                SommeTtc = SommeTva + sommevente;
            }

            lstitemsource.ItemsSource = ListPVC;
            PerformClick(btndialog);
        }

        private async void Save()
        {
            GetHome.DataContextt.Progress = Visibility.Visible;
            ListPC.Clear();
            bool stop = false;
            CanBeDeleted = true;
            var cmd = new VenteCredit();
            cmd.Date = DateTime.UtcNow;
            if (SelectedClient == null)
            {
                //Une erreur est sur venue
                GetHome.DataContextt.Progress = Visibility.Hidden;
                MessageBox.Show(Properties.Resources.ForgetClient, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            cmd.ClientId = SelectedClient.Id;
            cmd.UtilisateurId = GetHome.DataContextt.ConnectedUser.Id;
            decimal sommevente = 0;
            SommeTva = 0;
            SommeTtc = 0;
            ProduitCredit vendu;
            List<bool> saverslt = new List<bool>();
            var children = ProdGrid.Children;
            for (int i = 0; i < children.Count; i += 2)
            {
                vendu = GetProduitCommande(children[i], children[i + 1],cmd);
                if (vendu == null)
                {
                    stop = true;
                    break;
                }

                //Vérifie si le client bénéficie d'une réduction
                var clientdisc = Servante.ClientHasDiscount(cmd.ClientId);
                if (clientdisc.Item1 && clientdisc.Item3)
                {
                    //vérifie si le discount a expiré
                    if (InfoChecker.IsWithinDate(clientdisc.Item2.DateDebut, clientdisc.Item2.DateFin, DateTime.Now))
                    {
                        //le discount est valide
                        vendu.GetTotal(vendu.PrixUnitaire * (1 - clientdisc.Item2.Taux));
                        vendu.Discount = clientdisc.Item2.Taux;
                    }
                    else
                    {
                        //le discount a expiré

                        //Check if discount canceled staus must be update
                        if (InfoChecker.HasExpire(clientdisc.Item2.DateFin, DateTime.Now))
                        {
                            //update Discount canceled status
                            DbManager.UpdateDiscountCanceled(clientdisc.Item2.Id, true);
                        }
                        //Vérifie s'il y a un discount sur le produit
                        var disc = Servante.ProductHasDiscount(vendu.ProduitId);
                        if (disc.Item1 && disc.Item3)
                        {
                            vendu.GetTotal(vendu.PrixUnitaire * (1 - disc.Item2.Taux));
                            vendu.Discount = disc.Item2.Taux;
                        }
                        else
                        {
                            vendu.GetTotal();
                            vendu.Discount = 0;
                        }
                    }
                }
                else
                {
                    //si non le client n'en bénéficie pas
                    //Vérifie s'il y a un discount sur le produit
                    var disc = Servante.ProductHasDiscount(vendu.ProduitId);
                    if (disc.Item1 && disc.Item3)
                    {
                        vendu.GetTotal(vendu.PrixUnitaire * (1 - disc.Item2.Taux));
                        vendu.Discount = disc.Item2.Taux;
                    }
                    else
                    {
                        vendu.GetTotal();
                        vendu.Discount = 0;
                    }
                }


                sommevente += vendu.Montant;
                ListPC.Add(vendu);
            }

            if (stop)
            {
                //Une erreur est sur venue
                if (QuantityError)
                    MessageBox.Show(Properties.Resources.QuantiteManquante, Properties.Resources.MainTitle,
                        MessageBoxButton.OK, MessageBoxImage.Stop);
                else
                    MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle,
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            //montant de la commande
            cmd.Montant = sommevente;
            //montant restant de la commande
            cmd.MontantRestant = sommevente;
            //montant tva
            if (Tva!=null)
            {
                if(Tva.Apply)
                    SommeTva = (sommevente * Tva.Taux)/100;
            }
            if (DicountValueCommand.CanExecute(Montant))
            {
                var disc = decimal.Parse(Montant);
                cmd.ValueDiscount = disc;

                //montant de la commande
                cmd.Montant = sommevente - disc;
                //montant restant de la commande
                cmd.MontantRestant = sommevente - disc;

                SommeTtc = (SommeTva + sommevente) - disc;
            }
            else
            {
                SommeTtc = SommeTva + sommevente;
            }

            int rslt = DbManager.SaveData(cmd); // save commande
            cmd.Id = rslt;
            if (rslt > 0)
            {
                ListPCommandes.Clear();
                foreach (var item in ListPC)
                {
                    // ajout des produits commandés à la base de données
                    item.CommandeId = rslt;
                    //item.Tva = ApplyTVA == true ? 1 : 0;
                    item.SetProduit(GetProduit(item.ProduitId));
                    ListPCommandes.Add(item);
                    saverslt.Add(DbManager.Save(item));
                }

                if (!saverslt.Contains(false))
                {
                    await PrintAsync(cmd);
                    GetHome.DataContextt.Progress = Visibility.Hidden;
                    // tous les produits commandés ont bien été enregistrés
                    //MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK,
                        //MessageBoxImage.Information);
                    Reset();
                    if (Generated)
                    {
                        showDocument = new ShowDocument(SaveLocation);
                        showDocument.ShowDialog();
                    }
                }
                else
                {
                    //Une erreur est sur venue
                    GetHome.DataContextt.Progress = Visibility.Hidden;
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                GetHome.DataContextt.Progress = Visibility.Hidden;
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //Save();
            SellResume();
        }
        
        private void GetTva()
        {
           var tva=DbManager.GetAll<Tva>();
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
                    else
                    {
                        Company = company[1];
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

        private EasyManagerLibrary.Produit GetProduit(int id)
        {
            return DbManager.GetById<EasyManagerLibrary.Produit>(id);
        }

        private void ManageDiscount(ref ProduitCredit vendu)
        {
            //Vérifie s'il y a un discount
            var disc = Servante.ProductHasDiscount(vendu.ProduitId);
            if (disc.Item1 && disc.Item3)
                vendu.GetTotal(vendu.PrixUnitaire * disc.Item2.Taux);
        }

        private void Printer(VenteCredit venteCredit)
        {
            if (CanBePrint)
            {

                string clientname;
                var clientt = DbManager.GetClientById(venteCredit.ClientId);
                clientname = $"{clientt.Nom} {clientt.Prenom}";


                SaveLocation = InfoChecker.SaveLoc(clientname, Properties.Resources.TypeCommande);
                //Genarate bill
                Generated = Recu(GetClient(venteCredit.ClientId), venteCredit, ListPCommandes);
                    
            }
        }

        private EasyManagerLibrary.Client GetClient(int? id)
        {
            if (id.HasValue)
            {
                int clientid = id.Value;
                return DbManager.GetClientById(clientid);
            }
            else
            {
                return null;
            }
        }

        public bool Recu(EasyManagerLibrary.Client client, VenteCredit venteCredit, List<ProduitCredit> vendu)
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
            office.DiscountValue = Montant;
            office.IsRecall = false;
            var rslt = GetFactureHeader();
            if (rslt == null)
                office.LogoPath = GetShopLogo();
            else
                office.LogoPath = rslt.Data == "InvoiceOne" ? GetShopLogo() : GetShopLogoTwo();
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

            if (office.GenFactureNew(System.IO.Path.GetFullPath("Files\\Facture.dotx"), SaveLocation,'C'))
            {
                //PerformClick(btndialogclose);
                return true;
            }
            else
                return false;
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
        private void Reset()
        {
            CbCatList.SelectedIndex = 0;
            CbProdList.SelectedIndex = 0;
            txtprod.Text = string.Empty;
            CbFocusedId = null;
            TxtFocusedId = null;
            Montant = "0";
            DeleteProductList();
        }

        private void DeleteProductList()
        {
            while (CanBeDeleted)
                RemoveLine();
        }

        private void FilledDropDown()
        {
            CbProdList.ItemsSource = ProduitList;
        }

        /// <summary>
        /// Récupère la liste des produits depuis la base de données
        /// </summary>
        /// <returns></returns>
        private List<string> ProdList()
        {
            var lst = DbManager.GetAll<EasyManagerLibrary.Produit>();
            List<string> lstprod = new List<string>();
            lstprod.Add(Properties.Resources.MakeSelection);
            foreach (var item in lst)
            {
                lstprod.Add(item.Nom);
            }

            return lstprod;
        }

        /// <summary>
        /// contitue l'object ProduitCommandé basé sur les entrées de l'utilisateur
        /// </summary>
        /// <param name="cbox">Dropdownbox contenant la liste des produits</param>
        /// <param name="tb">Textbox contenant la quantité du produit commandé</param>
        /// <returns></returns>
        private ProduitCredit GetProduitCommande(UIElement cbox, UIElement tb, VenteCredit credit)
        {
            ProduitCredit pc = new ProduitCredit();
            ComboBox cb = cbox as ComboBox;
            if (cb.SelectedItem == null)
                return null;
            string selection = cb.SelectedItem.ToString();
            if (selection == "Selectionnez")
            {
                return null;
            }

            var produit = SelectedProductId(selection);
            int prodid = produit.Id;
            TextBox txt = tb as TextBox;
            if (txt.Text.Length == 0)
            {
                return null;
            }

            int quantite = int.Parse(txt.Text);
            if (quantite == 0)
            {
                return null;
            }

            if (!IsOkLeftQuantity(produit.Nom, quantite))
            {
                QuantityError = true;
                return null;
            }

            pc.ProduitId = produit.Id;
            pc.Quantite = quantite;
            pc.QuantiteRestante = quantite;
            decimal PU = produit.PrixUnitaire;
            if (credit.ClientId > 0)
            {
                var client = DbManager.GetClientById(credit.ClientId);
                PU = client.ClientType == ClientType.ClientSimple ? produit.PrixUnitaire : produit.PrixGrossiste;
            }
            pc.PrixUnitaire = PU;
            UpdateStockAfterSell(produit.Id, quantite);
            return pc;
        }

        private ProduitCredit GetProduitCommandeSellResume(UIElement cbox, UIElement tb, VenteCredit credit)
        {
            ProduitCredit pc = new ProduitCredit();
            ComboBox cb = cbox as ComboBox;
            if (cb.SelectedItem == null)
                return null;
            string selection = cb.SelectedItem.ToString();
            if (selection == "Selectionnez")
            {
                return null;
            }

            var produit = SelectedProductId(selection);
            int prodid = produit.Id;
            TextBox txt = tb as TextBox;
            if (txt.Text.Length == 0)
            {
                return null;
            }

            int quantite = int.Parse(txt.Text);
            if (quantite == 0)
            {
                return null;
            }

            if (!IsOkLeftQuantity(produit.Nom, quantite))
            {
                QuantityError = true;
                return null;
            }

            pc.ProduitId = produit.Id;
            pc.Quantite = quantite;
            pc.QuantiteRestante = quantite;
            decimal PU = produit.PrixUnitaire;
            if (credit.ClientId>0)
            {
                var client = DbManager.GetClientById(credit.ClientId);
                PU = client.ClientType == ClientType.ClientSimple ? produit.PrixUnitaire : produit.PrixGrossiste;
            }
            pc.PrixUnitaire = PU;
            pc.Product=produit;
            return pc;
        }

        /// <summary>
        /// Récupère l'id du produit selection basé sur le nom
        /// </summary>
        /// <param name="item">Nom du produit</param>
        /// <returns></returns>
        private EasyManagerLibrary.Produit SelectedProductId(string item)
        {
            var prod = DbManager.GetProduitByName(item);
            return prod;
        }

        /// <summary>
        /// Récupère l'id du client selection basé sur le nom
        /// </summary>
        /// <param name="item">Nom du client</param>
        /// <returns></returns>
        private EasyManagerLibrary.Client SelectedClientId(string item)
        {
            var client = DbManager.GetClientByName(item);
            return client;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ProduitList = ProdList();
            FilledDropDown();
            FilledDropDownClient();
            GetTva();
            GetCompany();
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.VenteCreditTitle;
                GetHome.MainTitle.Text = Properties.Resources.VenteCreditTitle;
            }
        }

        private void FilledDropDownClient()
        {
            CbCatList.ItemsSource = ClientList();
        }

        /// <summary>
        /// Récupère la liste des clients depuis la base de données
        /// </summary>
        /// <returns></returns>
        private List<string> ClientList()
        {
            var lst = DbManager.GetAll<EasyManagerLibrary.Client>();

            List<string> lstcat = new List<string>();
            lstcat.Add(Properties.Resources.MakeSelection); //index 0
            foreach (var item in lst)
            {
                lstcat.Add(item.Nom);
            }

            return lstcat;
        }

        private void PerformClick(Button btn)
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(btn);
            IInvokeProvider invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProvider.Invoke();
        }

        private bool IsOkLeftQuantity(string prodname, double quantite)
        {
            try
            {
                var Prod = DbManager.GetProduitByName(prodname);
                return Prod.QuantiteRestante >= quantite;
            }
            catch
            {
                return false;
            }
        }

        private bool UpdateStockAfterSell(int ProdId, double Quantitevendu)
        {
            try
            {
                var oldprod = DbManager.GetProduitById(ProdId);
                double newquantiterestante = oldprod.QuantiteRestante - Quantitevendu;
                // Update
                oldprod.AlertQuantityEvent += Oldprod_AlertQuantityEvent;
                oldprod.QuantiteRestante = newquantiterestante;
                return DbManager.UpDate(oldprod, ProdId);
            }
            catch
            {
                return false;
            }
        }

        private void Oldprod_AlertQuantityEvent(object sender, AlertQuantityEventArgs e)
        {
            if (e.Produit.QuantiteRestante <= e.Produit.QuantiteAlerte)
            {
                NotificationContent notif = new NotificationContent();
                notif.Title = Properties.Resources.MainTitle;

                NotificationManager notifmanag = new NotificationManager();

                EasyManagerLibrary.Notifications notifications = new EasyManagerLibrary.Notifications();
                notifications.Date = DateTime.Now;
                if (e.Produit.QuantiteRestante <= 3)
                {
                    notifications.Couleur = "Red";
                    notifications.Message = Properties.Resources.EmptyStock;
                }
                else
                {
                    notifications.Couleur = "Orange";
                    notifications.Message = Properties.Resources.QuantiteNotification;
                }
                notifications.IsApprovisionnement = false;
                notifications.ProduitNom = e.Produit.Nom;
                notifications.ProduitQuantiteRestante = e.Produit.QuantiteRestante;

                GetNotifications = notifications;


                notif.Message = $"{e.Produit.Nom}{Environment.NewLine}{Properties.Resources.QuantiteNotification}{Environment.NewLine}{Properties.Resources.LeftQuantity}:{e.Produit.QuantiteRestante}{Environment.NewLine}{DateTime.Now}";
                notif.Type = NotificationType.Warning;
                var Duration = new TimeSpan(0, 0, 10);
                notifmanag.Show(notif, "", Duration, null, new Action(SaveNotification));


            }
            else
            {
                //Console.WriteLine("Tout va bien");
            }
        }

        private void CbCatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
            {
                SelectedClient = null;
                return;
            }

            var item = e.AddedItems[0].ToString();
            if (item == Properties.Resources.MakeSelection)
                SelectedClient = null;
            else
                SelectedClient = SelectedClientId(item);
        }

        private void btnNewClient_Click(object sender, RoutedEventArgs e)
        {
            Client client = new Client();
            client.ShowDialog();
            FilledDropDownClient();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = sender as VenteCreditUC;
            var ah = window.ActualHeight;
            lstScroll.MaxHeight = ah - ah / 3;
        }

        private void SaveNotification()
        {
            var notif = DbManager.GetAll<EasyManagerLibrary.Notifications>();

            if (notif != null && notif.Count > 0)
            {
                var prodnotif = DbManager.GetNotificationByType(GetNotifications.ProduitNom, false);
                if (prodnotif != null)
                {
                    //update
                    var NotifToUpdate = prodnotif;
                    //Produit
                    var prod = DbManager.GetByColumnName<EasyManagerLibrary.Produit>("Nom", NotifToUpdate.ProduitNom)[0];
                    if (prod.QuantiteRestante <= 3)
                    {
                        NotifToUpdate.Message = Properties.Resources.EmptyStock;
                        NotifToUpdate.Couleur = "Red";
                    }
                    else
                    {
                        //NotifToUpdate.Message = Properties.Resources.EmptyStock;
                        //NotifToUpdate.Couleur = "Red";
                    }
                    NotifToUpdate.NoticationEvent += NotifToUpdate_NoticationEvent;
                    NotifToUpdate.ProduitQuantiteRestante = GetNotifications.ProduitQuantiteRestante;
                    NotifToUpdate.Date = GetNotifications.Date;
                    DbManager.UpDate(NotifToUpdate, NotifToUpdate.Id);
                }
                else
                {
                    //new record
                    GetNotifications.NoticationEvent += NotifToUpdate_NoticationEvent;
                    DbManager.Save(GetNotifications);
                    GetNotifications.Save = true;
                }
            }
            else
            {
                //new record
                GetNotifications.NoticationEvent += NotifToUpdate_NoticationEvent;
                DbManager.Save(GetNotifications);
                GetNotifications.Save = true;
            }

        }

        private void NotifToUpdate_NoticationEvent(object sender, NotificationEventArgs e)
        {
            var notificationcount = DbManager.GetAll<EasyManagerLibrary.Notifications>().Count();
            GetHome.DataContextt.NotificationCount = notificationcount;
        }

        private void Sample1_DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (IsDiscountValue)
                return;
            if (Equals(eventArgs.Parameter, true))
            {
                //save
                Save();
            }
            else
            {
                //Canceled
                //Console.WriteLine("bbbbbbbbbbbbbbb");
            }


        }

        private void DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            IsDiscountValue = true;

            if (DicountValueCommand.CanExecute(Montant))
            {
                if (Equals(eventArgs.Parameter, true))
                {
                    //save

                }
                else
                {
                    //Canceled

                    Montant = "0";
                }
            }
            else
            {
                Montant = "0";
                discountdialog.IsOpen = true;
            }

        }

        private void Sample3_DialogHost_OnDialogOpend(object sender, DialogOpenedEventArgs eventArgs)
        {
            IsDiscountValue = false;
        }

        private void DialogHost_OnDialogOpend(object sender, DialogOpenedEventArgs eventArgs)
        {
            IsDiscountValue = true;
        }

        private void btnvaluediscount_Click(object sender, RoutedEventArgs e)
        {
            PerformClick(btndialogdiscount);
        }

        private void CbCatList_DropDownOpened(object sender, EventArgs e)
        {
            FilledDropDownClient();
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

        private Settings GetSecondLogo()
        {
            var query = "SELECT * FROM  Settings WHERE Name='SecondLogo'";
            var rslt = DbManager.CustumQuery<Settings>(query);

            if (rslt.Count == 0)
                return null;
            else
                return rslt.FirstOrDefault();
        }
    }
}
