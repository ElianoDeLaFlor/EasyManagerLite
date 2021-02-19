using EasyManagerDb;
using EasyManagerLibrary;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Notifications.Wpf;
using Notifications.Wpf.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections;
using EasyManager.Commands;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for VenteUC.xaml
    /// </summary>
    public partial class VenteUC : UserControl, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region Déclaration des variables
        List<ProduitVendu> lst;
        private string _montantt;
        private decimal _sommeTtc;
        private decimal _sommeTva;
        private string _clientname;

        private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();
        public bool HasErrors => _errorsByPropertyName.Any();
        public List<ProduitVendu> ListPV { get; set; } = new List<ProduitVendu>();
        public List<ProduitVendu> ListProduitVendu { get; set; } = new List<ProduitVendu>();
        public List<ProduitVendu> ListPVR
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
        public string ClientName 
        { 
            get=>_clientname;
            set
            {
                if (_clientname == value)
                    return;
                _clientname = value;
                OnPropertyChanged();
            }
        }
        public List<string> ProduitList { get; set; }
        public List<string> Commande { get; set; } = new List<string>();
        public int SelectedCommandId { get; set; }
        public int SelectedClientId { get; set; }
        public ComboBox CbFocusedId { get; set; } = null;
        public TextBox TxtFocusedId { get; set; } = null;
        private List<string> ListClient { get; set; } = new List<string>();
        public bool CanBeDeleted { get; set; } = true;
        public bool _canBePrint { get; set; } = true;
        public ShowDocument showDocument { get; set; }
        public Await wait { get; set; }
        public VenteCredit SelectedVenteCredit { get; set; }
        public VenteCredit GetVenteCredit { get; set; }
        public EasyManagerLibrary.Produit SelectedProduct { get; set; }
        public bool QuantityError { get; set; }
        public bool IsCommand { get; set; } = false;
        public EasyManagerLibrary.Client Client { get; set; } = new EasyManagerLibrary.Client();
        /// <summary>
        /// Liste du nom des produits
        /// </summary>
        public List<EasyManagerLibrary.Produit> ProdName { get; set; } = new List<EasyManagerLibrary.Produit>();

        /// <summary>
        /// Liste des quantités vendues
        /// </summary>
        public List<int> ProdQty { get; set; } = new List<int>();
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
        public bool IsOkToContinue { get; set; } = true;
        public int MaxRowCount { get; set; }
        public bool IsOkRowCount { get; set; } = false;
        public Home GetHome { get; set; }
        public string SaveLocation { get; set; }
        public bool Generated { get; set; } = false;
        public Tva Tva { get; set; } = new Tva();
        public CompanyInfo Company { get; set; } = new CompanyInfo();
        public decimal SommeTva 
        {
            get => _sommeTva;
            set
            {
                if (_sommeTva == value)
                    return;
                _sommeTva = value;
                OnPropertyChanged();
            }
        }
        public decimal SommeTtc 
        { 
            get=>_sommeTtc;
            set
            {
                if (_sommeTtc == value)
                    return;
                _sommeTtc = value;
                OnPropertyChanged();
            }
        }
        public EasyManagerLibrary.Notifications GetNotifications { get; set; } = new EasyManagerLibrary.Notifications();
        DicountValueCommand DicountValueCommand;
        public bool IsDiscountValue { get; set; } = false;
        #endregion


        public VenteUC()
        {
            InitializeComponent();
        }

        public VenteUC(Home h)
        {
            InitializeComponent();
            GetHome = h;
            //container.MinWidth = GetHome.ActualWidth - 500;
            //ManagerTable("ValueDiscount", "REAL","0");
            //ManagerTable("Canceled", "INTEGER","0");
            //ManagerTable("CanceledDate", "TEXT", "NULL");
            GetHome.SizeChanged += GetHome_SizeChanged;
            DicountValueCommand = new DicountValueCommand(this);
            DataContext = this;
        }

        public void ManagerTable(string columnName,string typ,string defaultvalue)
        {
            //check if the column already exist the table
            if (!DbManager.CustumQueryCheckColumn<EasyManagerLibrary.Vente>(columnName))
            {
                // the column is not in the table
                // so we have to create it
                DbManager.CreateNewColumn<EasyManagerLibrary.Vente>(columnName, typ, defaultvalue);
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsByPropertyName.ContainsKey(propertyName) ? _errorsByPropertyName[propertyName] : null;
        }
        private void OnErrorChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
                _errorsByPropertyName[propertyName] = new List<string>();

            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorChanged(propertyName);
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorChanged(propertyName);
            }
        }

        private void ValidateAmount()
        {
            ClearErrors(nameof(Montant));
            if (!double.TryParse(Montant, out _))
                AddError(nameof(Montant), Properties.Resources.AmountError);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool CanBePrint
        {
            get => _canBePrint;
            set
            {
                if (_canBePrint == value)
                    return;
                _canBePrint = value;
                OnPropertyChanged();
            }
        }


        private void GetHome_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //container.MinWidth = Math.Abs(GetHome.ActualWidth - 500);
        }

        private void AddNewLine()
        {
            var newrow = new RowDefinition();
            var rowcount = ProdGrid.RowDefinitions.Count;
            if (IsCommand)
            {
                //Le nombre de ligne généré ne doit pas dépasser le nombre de produit vendu à credit
                if (rowcount == MaxRowCount)
                {
                    IsOkRowCount = true;
                    return;
                }
            }

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
            HintAssist.SetHelperText(tb, Properties.Resources.QuantiteVendu);



            ComboBox cb = new ComboBox();
            cb.Name = $"cbprod{rowcount}";
            //Style cbstyle = FindResource("CbStyle") as Style;
            //cb.Style = cbstyle;
            cb.ToolTip = Properties.Resources.SelectionProduit;
            //cb.Text = Properties.Resources.MakeSelection;
            if (IsCommand)
                cb.ItemsSource = CommandedProduct(SelectedCommandId);
            else
                cb.ItemsSource = ProduitList;
            cb.LostFocus += Cb_LostFocus;
            cb.IsEditable = true;
            HintAssist.SetHint(cb, Properties.Resources.MakeSelection);
            HintAssist.SetHintOpacity(cb, .26);
            HintAssist.SetHelperText(cb, Properties.Resources.SelectionProduit);
            cb.Margin = new Thickness(16);
            cb.FontSize = 20;


            ProdGrid.RowDefinitions.Add(newrow);
            var lst = ProdGrid.RowDefinitions.Last();
            //rowcount = lst

            Grid.SetRow(cb, rowcount);
            Grid.SetColumn(cb, 0);
            Grid.SetRow(tb, rowcount);
            Grid.SetColumn(tb, 1);

            var i = ProdGrid.Children.Count;
            ProdGrid.Children.Add(cb);
            ProdGrid.Children.Add(tb);
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
            if (IsCommand)
            {
                if (!IsOkQuantityCmd(SelectedCommandId, prodname, qty))
                {
                    MessageBox.Show(Properties.Resources.VenteCreditQuantityCheck, Properties.Resources.MainTitle,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    CbFocusedId = null;
                }
            }
            else
            {
                if (!IsOkLeftQuantity(prodname, qty))
                {
                    MessageBox.Show(Properties.Resources.QuantiteManquante, Properties.Resources.MainTitle,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    CbFocusedId = null;
                }
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

            if (IsCommand)
            {
                if (!IsOkQuantityCmd(SelectedCommandId, prodname, qty))
                {
                    MessageBox.Show(Properties.Resources.VenteCreditQuantityCheck, Properties.Resources.MainTitle,
                        MessageBoxButton.OK, MessageBoxImage.Warning);

                    //TxtFocusedId = null;
                }
            }
            else
            {
                if (!IsOkLeftQuantity(prodname, qty))
                {
                    MessageBox.Show(Properties.Resources.QuantiteManquante, Properties.Resources.MainTitle,
                        MessageBoxButton.OK, MessageBoxImage.Warning);

                    //TxtFocusedId = null;
                }
            }
        }

        /// <summary>
        /// Vérifie si tout est ok avant d'ajouter un nouveau champs
        /// </summary>
        /// <param name="tb">TextBox contenant la quantité vendu</param>
        private bool OnFocusLost(TextBox tb)
        {
            if (tb == null)
                return true;
            if (IsOkRowCount)
                return false;
            string txtid = tb.Name;
            if (string.IsNullOrWhiteSpace(tb.Text))
                return false;
            int qty = int.Parse(tb.Text);
            TxtFocusedId = tb;

            // txt
            if (CbFocusedId == null)
                return false;
            var prodname = CbFocusedId.SelectedItem as string;
            if (prodname == null)
                return false;

            if (IsCommand)
            {
                if (!IsOkQuantityCmd(SelectedCommandId, prodname, qty))
                {
                    MessageBox.Show(Properties.Resources.VenteCreditQuantityCheck, Properties.Resources.MainTitle,
                        MessageBoxButton.OK, MessageBoxImage.Warning);

                    //TxtFocusedId = null;
                    return false;
                }

                return true;
            }
            else
            {
                if (!IsOkLeftQuantity(prodname, qty))
                {
                    MessageBox.Show(Properties.Resources.QuantiteManquante, Properties.Resources.MainTitle,
                        MessageBoxButton.OK, MessageBoxImage.Warning);

                    //TxtFocusedId = null;
                    return false;
                }

                return true;
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

            IsOkRowCount = false;
            var txt = children[childcnt - 1];
            var cb = children[childcnt - 2];
            ProdGrid.Children.Remove(cb);
            ProdGrid.Children.Remove(txt);
            var last = ProdGrid.RowDefinitions.Last();
            ProdGrid.RowDefinitions.Remove(last);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
        }

        private async Task PrintAsync(EasyManagerLibrary.Vente vente)
        {
            await Task.Run(() => printer(vente));
        }

        private void VenteInfo(ProduitVendu vendu, EasyManagerLibrary.Vente vente)
        {
            //vérifie si c'est un credit qui sera remboursée(venteCredit)
            if (SelectedCommandId > 0)
            {
                //Si oui récupère les information sur le credit
                var credits = DbManager.CustumQuery<ProduitCredit>(
                    $"SELECT * FROM ProduitCredit WHERE ProduitId={vendu.ProduitId} AND CommandeId={SelectedCommandId}");
                //Récupère le taux de discount sur le credit
                var tauxDiscount = credits[0].Discount;
                //Applique le taux de discount sur le prix unitaire
                vendu.GetTotal(vendu.PrixUnitaire * (1 - tauxDiscount));
                //ajout du taux au prouit
                vendu.Discount = tauxDiscount;
            }
            //Non -> récupère le information du client
            else
            {
                //Vérifie s'il y a un client pour la vente
                if (SelectedClientId > 0)
                {
                    //Vérifie si le client a une réduction
                    var clientdisc = Servante.ClientHasDiscount(vente.ClientId.Value);
                    if (clientdisc.Item1)
                    {
                        //vérifie si le discount a expiré
                        if (InfoChecker.IsWithinDate(clientdisc.Item2.DateDebut, clientdisc.Item2.DateFin, DateTime.Now))
                        {
                            //Le discount est valide
                            vendu.GetTotal(vendu.PrixUnitaire * (1 - clientdisc.Item2.Taux));
                            vendu.Discount = clientdisc.Item2.Taux;
                        }
                        else
                        {
                            //Le discount a expiré

                            //Check if discount canceled staus must be update
                            if (InfoChecker.HasExpire(clientdisc.Item2.DateFin, DateTime.Now))
                            {
                                //update Discount canceled status
                                DbManager.UpdateDiscountCanceled(clientdisc.Item2.Id, true);
                            }
                            // vérifie si le produit a une réduction
                            ManageDiscount(ref vendu);
                        }
                    }
                    else
                    {
                        //Non-> vérifie si le produit a une réduction
                        ManageDiscount(ref vendu);
                    }

                }
                // Non -> une vente sans client
                else
                {
                    ManageDiscount(ref vendu);
                }
            }
        }



        private void SellResume()
        {
            ListPV.Clear();
            ProdQty.Clear();
            lst = new List<ProduitVendu>();
            ListPVR = new List<ProduitVendu>();
            lst.Clear();
            CanBeDeleted = true;
            bool stop = false;
            var vente = new EasyManagerLibrary.Vente();
            vente.Date = DateTime.UtcNow;
            vente.UtilisateurId = GetHome.DataContextt.ConnectedUser.Id;
            vente.ClientId = SelectedClientId;
            vente.CommandeId = SelectedCommandId;
            decimal sommevente = 0;
            SommeTva = 0;
            SommeTtc = 0;
            ProduitVendu vendu;
            List<bool> SaveResult = new List<bool>();
            var children = ProdGrid.Children;
            for (int i = 0; i < children.Count; i += 2)
            {
                vendu = GetProduitVenduSellResume(children[i], children[i + 1],vente);
                if (vendu == null)
                {
                    stop = true;
                    break;
                }
                VenteInfo(vendu, vente);
                sommevente += vendu.Montant;
                lst.Add(vendu);
            }
            ListPVR = lst;



            if (stop)
            {
                //Une erreur est sur venue
                GetHome.DataContextt.Progress = Visibility.Hidden;
                if (QuantityError)
                    MessageBox.Show(Properties.Resources.QuantiteManquante, Properties.Resources.MainTitle,
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle,
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //montant de la vente
            vente.Montant = sommevente;
            //montant tva
            if (Tva != null)
            {
                if (Tva.Apply)
                    SommeTva = (sommevente * Tva.Taux) / 100;
            }
            if (DicountValueCommand.CanExecute(Montant))
            {
                var disc=decimal.Parse(Montant);
                SommeTtc = (SommeTva + sommevente)- disc;
            }
            else
            {
                SommeTtc = SommeTva + sommevente;
            }

            
            if (vente.ClientId.HasValue)
            {
                Client = DbManager.GetClientById(vente.ClientId.Value);
                if (Client != null)
                    ClientName = $"{Client.Nom} {Client.Prenom}";
                else
                {
                    if (IsCommand)
                    {
                        GetVenteCredit = DbManager.GetById<VenteCredit>(SelectedCommandId);
                        Client = DbManager.GetById<EasyManagerLibrary.Client>(GetVenteCredit.ClientId);
                        ClientName = $"{Client.Nom} {Client.Prenom}";
                    }
                    else
                        ClientName = $"Vente-{vente.Id}";
                }
            }
            else
            {
                if (IsCommand)
                {
                    GetVenteCredit = DbManager.GetById<VenteCredit>(SelectedCommandId);
                    Client = DbManager.GetById<EasyManagerLibrary.Client>(GetVenteCredit.ClientId);
                    ClientName = $"{Client.Nom} {Client.Prenom}";
                }
                else
                    ClientName = $"Vente-{vente.Id}";
            }


            lstitemsource.ItemsSource = ListPVR;
            PerformClick(btndialog);
        }

        

        private async void Save()
        {
            GetHome.DataContextt.Progress = Visibility.Visible;
            ListPV.Clear();
            ProdQty.Clear();
            ProdName.Clear();
            CanBeDeleted = true;
            bool stop = false;
            var vente = new EasyManagerLibrary.Vente();
            vente.Date = DateTime.UtcNow;
            vente.UtilisateurId = GetHome.DataContextt.ConnectedUser.Id;
            vente.ClientId = SelectedClientId;
            vente.CommandeId = SelectedCommandId;
            decimal sommevente = 0;
            SommeTva = 0;
            SommeTtc = 0;
            ProduitVendu vendu;
            List<bool> SaveResult = new List<bool>();
            var children = ProdGrid.Children;
            for (int i = 0; i < children.Count; i += 2)
            {
                vendu = GetProduitVendu(children[i], children[i + 1],vente);
                if (vendu == null)
                {
                    stop = true;
                    break;
                }
                VenteInfo(vendu, vente);
                sommevente += vendu.Montant;
                ListPV.Add(vendu);
            }

            if (stop)
            {
                //Une erreur est sur venue
                GetHome.DataContextt.Progress = Visibility.Hidden;
                if (QuantityError)
                    MessageBox.Show(Properties.Resources.QuantiteManquante, Properties.Resources.MainTitle,
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle,
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //montant de la vente
            vente.Montant = sommevente;
            //montant tva
            if (Tva != null)
            {
                if (Tva.Apply)
                    SommeTva = (sommevente * Tva.Taux) / 100;
            }
            if (DicountValueCommand.CanExecute(Montant))
            {
                var disc = decimal.Parse(Montant);
                // add discount to vente
                vente.ValueDiscount = disc;
                //montant de la vente
                vente.Montant = sommevente-disc;

                SommeTtc = (SommeTva + sommevente) - disc;
            }
            else
            {
                SommeTtc = SommeTva + sommevente;
            }

            

            //Save vente
            int Result = DbManager.SaveData(vente); // save vente
            vente.Id = Result;
            if (Result > 0)
            {
                ListProduitVendu.Clear();
                foreach (var item in ListPV)
                {
                    // ajout des produits vendus à la base de données
                    item.VenteId = Result;
                    item.SetProduit(GetProduit(item.ProduitId));
                    ListProduitVendu.Add(item);
                    Console.WriteLine(item);
                    SaveResult.Add(DbManager.Save(item));
                }

                if (!SaveResult.Contains(false))
                {
                    //Updates
                    if (IsCommand)
                    {

                        // Mettre à jour la somme restante au niveau de la VenteCredit
                        Servante.UpdateVenteCreditRemainingCost(SelectedCommandId, sommevente);
                        // Mettre à jour la quantité restante du produit commandé au niveau de la VenteCredit
                        int cnt = 0;
                        foreach (var item in ProdName)
                        {
                            Servante.UpdateProductRemainingQtyInCmd(SelectedCommandId, item, ProdQty[cnt]);
                            //item.AlertQuantityEvent += Item_AlertQuantityEvent;
                            cnt++;
                        }
                        GetVenteCredit = DbManager.GetById<VenteCredit>(SelectedCommandId);
                    }
                    await PrintAsync(vente);
                    GetHome.DataContextt.Progress = Visibility.Hidden;
                    // tous les produits vendus ont bien été enregistré
                    //MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK,
                       // MessageBoxImage.Information);
                    //Check if a bill must be generated
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

        private void printer(EasyManagerLibrary.Vente vente)
        {
            if (CanBePrint)
            {

                string clientname;
                if (vente.ClientId.HasValue)
                {
                    Client = DbManager.GetClientById(vente.ClientId.Value);
                    if (Client != null)
                        clientname = $"{Client.Nom} {Client.Prenom}";
                    else
                    {
                        if (IsCommand)
                        {
                            Client = DbManager.GetById<EasyManagerLibrary.Client>(GetVenteCredit.ClientId);
                            clientname = $"{Client.Nom} {Client.Prenom}";
                        }
                        else
                            clientname = $"Vente-{vente.Id}";
                    }
                }
                else
                {
                    if (IsCommand)
                    {
                        Client = DbManager.GetById<EasyManagerLibrary.Client>(GetVenteCredit.ClientId);
                        clientname = $"{Client.Nom} {Client.Prenom}";
                    }
                    else
                        clientname = $"Vente-{vente.Id}";
                }

                SaveLocation = InfoChecker.SaveLoc(clientname, Properties.Resources.TypeFacture);
                //Genarate bill
                Generated = Recu(Client, vente, ListProduitVendu);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //Save();
            SellResume();
        }

        private EasyManagerLibrary.Produit GetProduit(int id)
        {
            return DbManager.GetById<EasyManagerLibrary.Produit>(id);
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

        /// <summary>
        /// Réinitialise la valeur des champs
        /// </summary>
        private void ResetData()
        {
            CbFocusedId = null;
            TxtFocusedId = null;
            QuantityError = false;
        }

        private void ManageDiscount(ref EasyManagerLibrary.ProduitVendu vendu)
        {
            //Vérifie s'il y a un discount sur le produit
            var disc = Servante.ProductHasDiscount(vendu.ProduitId);
            if (disc.Item1)
            {
                //vérifie si le discount a expiré
                if (InfoChecker.IsWithinDate(disc.Item2.DateDebut, disc.Item2.DateFin, DateTime.Now))
                {
                    //le discount est valide
                    vendu.GetTotal(vendu.PrixUnitaire * (1 - disc.Item2.Taux));
                    vendu.Discount = disc.Item2.Taux;
                }
                else
                {
                    //Le discount a expiré

                    //Check if discount canceled staus must be update
                    if (InfoChecker.HasExpire(disc.Item2.DateFin, DateTime.Now))
                    {
                        //update Discount canceled status
                        DbManager.UpdateDiscountCanceled(disc.Item2.Id, true);
                    }
                    //le produit n'en bénéficie pas
                    vendu.GetTotal();
                    vendu.Discount = 0;
                }
            }
            else
            {
                //si non le produit n'en bénéficie pas
                vendu.GetTotal();
                vendu.Discount = 0;
            }

        }

        private void ManageDiscountResume(ref EasyManagerLibrary.ProduitVenduResume vendu)
        {
            //Vérifie s'il y a un discount sur le produit
            var disc = Servante.ProductHasDiscount(vendu.ProduitId);
            if (disc.Item1)
            {
                //vérifie si le discount a expiré
                if (InfoChecker.IsWithinDate(disc.Item2.DateDebut, disc.Item2.DateFin, DateTime.Now))
                {
                    //le discount est valide
                    if (disc.Item2.Taux > 0 && disc.Item2.Taux <= 1)
                    {
                        // reducion par pourcentage
                        vendu.GetTotal(vendu.PrixUnitaire * (1 - disc.Item2.Taux));
                        vendu.Discount = disc.Item2.Taux;
                    }
                    else
                    {
                        // reduction par valeur
                        vendu.Montant = (vendu.PrixUnitaire * (decimal)vendu.Quantite) - vendu.Discount;
                        vendu.Discount = disc.Item2.Taux;
                    }

                }
                else
                {
                    //Le discount a expiré

                    //Check if discount canceled staus must be update
                    if (InfoChecker.HasExpire(disc.Item2.DateFin, DateTime.Now))
                    {
                        //update Discount canceled status
                        DbManager.UpdateDiscountCanceled(disc.Item2.Id, true);
                    }
                    //le produit n'en bénéficie pas
                    vendu.GetTotal();
                    vendu.Discount = 0;
                }
            }
            else
            {
                //si non le produit n'en bénéficie pas
                vendu.GetTotal();
                vendu.Discount = 0;
            }

        }

        public bool Recu(EasyManagerLibrary.Client client, EasyManagerLibrary.Vente vente, List<ProduitVendu> vendu)
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
            office.DiscountValue = Montant;
            office.IsRecall = false;
            office.IsCommand = IsCommand;
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
            office.MontantTotalCredit = Properties.Resources.MontantTotalCredit;

            if (office.GenFactureNew(System.IO.Path.GetFullPath("Files\\Facture.dotx"), SaveLocation))
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
        private void btnadd_Click(object sender, RoutedEventArgs e)
        {
            if (!OnFocusLost(TxtFocusedId))
                return;
            ResetData();
            AddNewLine();
        }

        private void btnminus_Click(object sender, RoutedEventArgs e)
        {
            ResetData();
            RemoveLine();
        }

        private ProduitVendu GetProduitVendu(UIElement cbox, UIElement tb,EasyManagerLibrary.Vente vente)
        {
            ProduitVendu pv = new ProduitVendu();
            ComboBox cb = cbox as ComboBox;

            if (cb.SelectedItem == null)
                return null;
            string selection = cb.SelectedItem.ToString();
            if (selection == "Selectionnez")
            {
                return null;
            }

            var produit = SelectedProductId(selection);
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

            if (IsCommand)
            {
                if (!IsOkQuantityCmd(SelectedCommandId, produit.Nom, quantite))
                {
                    QuantityError = true;
                    return null;
                }
            }
            else
            {
                if (!IsOkLeftQuantity(produit.Nom, quantite))
                {
                    QuantityError = true;
                    return null;
                }
            }

            pv.ProduitId = produit.Id;
            pv.Quantite = quantite;
            decimal PU = produit.PrixUnitaire;
            if (IsCommand)
            {
                var credit = DbManager.GetById<VenteCredit>(SelectedCommandId);
                var client = DbManager.GetById<EasyManagerLibrary.Client>(credit.ClientId);
                PU = client.ClientType == ClientType.ClientSimple ? produit.PrixUnitaire : produit.PrixGrossiste;
            }
            else
            {
                if (vente.ClientId.HasValue)
                {
                    var client = DbManager.GetClientById(vente.ClientId.Value);
                    if (client == null)
                        PU = produit.PrixUnitaire;
                    else
                        PU = client.ClientType == ClientType.ClientSimple ? produit.PrixUnitaire : produit.PrixGrossiste;
                }
            }
            pv.PrixUnitaire = PU;
            ProdName.Add(produit);
            ProdQty.Add(quantite);
            pv.Product = produit;
            if (!IsCommand)
                UpdateStockAfterSell(produit.Id, quantite); //// Mettre à jour la quantité des produits
            return pv;
        }

        private ProduitVendu GetProduitVenduSellResume(UIElement cbox, UIElement tb,EasyManagerLibrary.Vente vente)
        {
            ProduitVendu pv = new ProduitVendu();
            ComboBox cb = cbox as ComboBox;

            if (cb.SelectedItem == null)
                return null;
            string selection = cb.SelectedItem.ToString();
            if (selection == "Selectionnez")
            {
                return null;
            }

            var produit = SelectedProductId(selection);
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

            if (IsCommand)
            {
                if (!IsOkQuantityCmd(SelectedCommandId, produit.Nom, quantite))
                {
                    QuantityError = true;
                    return null;
                }
            }
            else
            {
                if (!IsOkLeftQuantity(produit.Nom, quantite))
                {
                    QuantityError = true;
                    return null;
                }
            }

            pv.ProduitId = produit.Id;
            pv.Quantite = quantite;
            decimal PU = produit.PrixUnitaire;
            if (IsCommand)
            {
                var credit = DbManager.GetById<VenteCredit>(SelectedCommandId);
                var client = DbManager.GetById<EasyManagerLibrary.Client>(credit.ClientId);
                PU = client.ClientType == ClientType.ClientSimple ? produit.PrixUnitaire : produit.PrixGrossiste;
            }
            else
            {
                if (vente.ClientId.HasValue)
                {
                    var client = DbManager.GetClientById(vente.ClientId.Value);
                    if (client == null)
                        PU = produit.PrixUnitaire;
                    else
                        PU = client.ClientType == ClientType.ClientSimple ? produit.PrixUnitaire : produit.PrixGrossiste;
                }
            }
            pv.PrixUnitaire = PU;
            ProdName.Add(produit);
            ProdQty.Add(quantite);
            pv.Product = produit;

            return pv;
        }

        private ProduitVenduResume GetProduitVenduResume(UIElement cbox, UIElement tb)
        {
            ProduitVenduResume pv = new ProduitVenduResume();
            ComboBox cb = cbox as ComboBox;

            if (cb.SelectedItem == null)
                return null;
            string selection = cb.SelectedItem.ToString();
            if (selection == "Selectionnez")
            {
                return null;
            }

            var produit = SelectedProductId(selection);
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

            if (IsCommand)
            {
                if (!IsOkQuantityCmd(SelectedCommandId, produit.Nom, quantite))
                {
                    QuantityError = true;
                    return null;
                }
            }
            else
            {
                if (!IsOkLeftQuantity(produit.Nom, quantite))
                {
                    QuantityError = true;
                    return null;
                }
            }

            pv.ProduitId = produit.Id;
            pv.Quantite = quantite;
            pv.PrixUnitaire = produit.PrixUnitaire;
            ProdName.Add(produit);
            ProdQty.Add(quantite);
            pv.SetProduit(produit);

            return pv;
        }

        private EasyManagerLibrary.Produit SelectedProductId(string item)
        {
            var prod = DbManager.GetProduitByName(item);
            return prod;
        }

        private void FilledDropDown(List<string> lst)
        {
            if (CbProdList.ItemsSource != lst)
                CbProdList.ItemsSource = lst;
        }

        private void FilledDropDownCmd()
        {
            CbCmdList.ItemsSource = null;
            CbCmdList.ItemsSource = Commande;
        }

        private void FilledDropDownClient()
        {
            if (CbClientList.ItemsSource != ListClient)
                CbClientList.ItemsSource = ListClient;
        }

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

        private void Reset()
        {
            CbCmdList.SelectedIndex = 0;
            CbProdList.SelectedIndex = 0;
            CbClientList.SelectedIndex = 0;
            txtprod.Text = string.Empty;
            Montant = "0";
            DeleteProductList();
        }

        private void DeleteProductList()
        {
            while (CanBeDeleted)
                RemoveLine();
        }

        /// <summary>
        /// Récupère l'id de la commande selection basé sur le nom
        /// </summary>
        /// <param name="item">Identifiant de la commande</param>
        /// <returns></returns>
        private EasyManagerLibrary.VenteCredit SelectedCommandeId(string item)
        {
            var cmd = DbManager.GetClientByName(item);
            return null;
        }

        private void GetCommandeList()
        {
            Commande.Clear();
            Commande.Add(Properties.Resources.MakeSelection);
            string query = "SELECT * FROM VenteCredit WHERE Canceled=false";
            var rslt = DbManager.CustumQuery<VenteCredit>(query);
            foreach (var item in rslt)
            {
                Commande.Add(InfoChecker.FormatIdent(item.Id));
            }
        }

        private void GetClientList()
        {
            ListClient.Clear();
            ListClient.Add(Properties.Resources.MakeSelection);
            var rslt = DbManager.GetAll<EasyManagerLibrary.Client>();
            foreach (var item in rslt)
            {
                ListClient.Add(item.Nom);
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

                if (DbManager.UpDate(oldprod, ProdId))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
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

        private void CbCmdList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
            var item = e.AddedItems[0].ToString();
            if (item == Properties.Resources.MakeSelection)
            {
                SelectedCommandId = 0;
                // Active la selection de client
                FilledDropDown(ProduitList);
                CbClientList.IsEnabled = true;
                btnListProduitVendu.IsEnabled = false;
                btnvaluediscount.IsEnabled = true;
                IsCommand = false;
            }
            else
            {
                SelectedCommandId = int.Parse(item);
                // Désactive la selection de client
                CbClientList.IsEnabled = false;
                btnListProduitVendu.IsEnabled = true;
                CbClientList.SelectedIndex = 0;
                //desactiver la reduction par valeur
                btnvaluediscount.IsEnabled = false;
                Montant = "0";
                // Liste la list des produits commandés
                FilledDropDown(CommandedProduct(SelectedCommandId));
                IsCommand = true;
            }
        }

        private void CbClientList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                var item = e.AddedItems[0].ToString();
                if (item == Properties.Resources.MakeSelection)
                {
                    SelectedClientId = 0;
                }
                else
                {
                    var client = DbManager.GetClientByName(item);
                    SelectedClientId = client.Id;
                    // Désactive la selection de client
                }
            }
        }

        private bool IsOkLeftQuantity(string prodname, int quantite)
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

        private bool IsOkQuantityCmd(int CmdId, string prodname, int quantite)
        {
            try
            {
                var cmd = DbManager.GetById<EasyManagerLibrary.VenteCredit>(CmdId);
                //on récupère le produit selectioné
                var Prod = DbManager.GetProduitByName(prodname);
                //on récupère le produit commandé a payé
                var Prodcmd = DbManager.GetProductByIdCmdId(CmdId, Prod.Id);

                return Prodcmd.QuantiteRestante >= quantite;
            }
            catch
            {
                return false;
            }
        }

        private List<string> CommandedProduct(int cmdid)
        {
            var CP = DbManager.GetAllProductByCmd(cmdid);
            MaxRowCount = CP.Count;
            List<string> lst = new List<string>();
            lst.Add(Properties.Resources.MakeSelection);
            foreach (var item in CP)
            {
                lst.Add(item.GetProduit().Nom);
            }

            return lst;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ProduitList = ProdList();
            GetCommandeList();
            GetClientList();
            FilledDropDown(ProduitList);
            FilledDropDownCmd();
            FilledDropDownClient();
            GetTva();
            GetCompany();
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.Selle;
                GetHome.MainTitle.Text = Properties.Resources.Selle;
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = sender as VenteUC;
            var ah = window.ActualHeight;
            lstScroll.MaxHeight = ah / 2;
        }

        private bool Print()
        {
            return true;
        }

        private void Sample2_DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            //Console.WriteLine("SAMPLE 2: Closing dialog with parameter: " + (eventArgs.Parameter ?? ""));
        }

        private void PerformClick(Button btn)
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(btn);
            IInvokeProvider invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProvider.Invoke();
        }

        private void btnListProduitVendu_Click(object sender, RoutedEventArgs e)
        {
            ListeProduitCommende produitVendus = new ListeProduitCommende(SelectedCommandId);
            produitVendus.ShowDialog();
        }

        private void CbClientList_GotFocus(object sender, RoutedEventArgs e)
        {
            GetClientList();
            FilledDropDownClient();
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
    }
}
