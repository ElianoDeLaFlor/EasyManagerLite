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
    /// Interaction logic for Vente.xaml
    /// </summary>
    public partial class Vente : Window
    {
        #region Déclaration des variables
        public Vente()
        {
            InitializeComponent();
        }
        public List<ProduitVendu> ListPV { get; set; } = new List<ProduitVendu>();
        public List<string> ProduitList { get; set; }
        public List<string> Commande { get; set; } = new List<string>();
        public int SelectedCommandId { get; set; }
        public int SelectedClientId { get; set; }
        public ComboBox CbFocusedId { get; set; } = null;
        public TextBox TxtFocusedId { get; set; } = null;
        public List<string> ListClient { get; set; } = new List<string>();
        public bool CanBeDeleted { get; set; } = true;
        public EasyManagerLibrary.VenteCredit SelectedVenteCredit { get; set; }
        public EasyManagerLibrary.Produit SelectedProduct { get; set; }
        public bool QuantityError { get; set; }
        public bool IsCommand { get; set; } = false;
        /// <summary>
        /// Liste du nom des produits
        /// </summary>
        public List<EasyManagerLibrary.Produit> ProdName { get; set; } = new List<EasyManagerLibrary.Produit>();
        /// <summary>
        /// Liste des quantités vendues
        /// </summary>
        public List<int> ProdQty { get; set; } = new List<int>();
        public bool IsOkToContinue { get; set; } = true;
        public int MaxRowCount { get; set; }
        public bool IsOkRowCount { get; set; } = false;
        #endregion

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
                    MessageBox.Show(Properties.Resources.VenteCreditQuantityCheck, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    CbFocusedId = null;
                }
            }
            else
            {
                if (!IsOkLeftQuantity(prodname, qty))
                {
                    MessageBox.Show(Properties.Resources.QuantiteManquante, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show(Properties.Resources.VenteCreditQuantityCheck, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);

                    //TxtFocusedId = null;
                }
            }
            else
            {
                if (!IsOkLeftQuantity(prodname, qty))
                {
                    MessageBox.Show(Properties.Resources.QuantiteManquante, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);

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
                    MessageBox.Show(Properties.Resources.VenteCreditQuantityCheck, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);

                    //TxtFocusedId = null;
                    return false;
                }
                return true;
            }
            else
            {
                if (!IsOkLeftQuantity(prodname, qty))
                {
                    MessageBox.Show(Properties.Resources.QuantiteManquante, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);

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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ListPV.Clear();
            CanBeDeleted = true;
            bool stop = false;
            var vente = new EasyManagerLibrary.Vente();
            vente.Date = DateTime.UtcNow;
            vente.UtilisateurId = InfoChecker.ConnectedUser.Id;
            vente.ClientId = SelectedClientId;
            vente.CommandeId = SelectedCommandId;
            decimal sommevente = 0;
            ProduitVendu vendu;
            List<bool> SaveResult = new List<bool>();
            var children = ProdGrid.Children;
            for (int i = 0; i < children.Count; i += 2)
            {
                vendu = GetProduitVendu(children[i], children[i + 1]);
                if (vendu == null)
                {
                    stop = true;
                    break;
                }
                sommevente += vendu.Montant;
                ListPV.Add(vendu);
            }
            if (stop)
            {
                //Une erreur est sur venue
                if (QuantityError)
                    MessageBox.Show(Properties.Resources.QuantiteManquante, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            vente.Montant = sommevente;
            //Save vente
            int Result = DbManager.SaveData(vente);// save vente
            if (Result > 0)
            {
                foreach (var item in ListPV)
                {
                    // ajout des produits vendus à la base de données
                    item.VenteId = Result;
                    SaveResult.Add(DbManager.Save(item));
                }
                if (!SaveResult.Contains(false))
                {
                    // tous les produits vendus ont bien été enregistré
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
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
                            cnt++;
                        }
                    }
                    Reset();
                }
                else
                    //Une erreur est sur venue
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
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

        public void Recu(EasyManagerLibrary.Client client, EasyManagerLibrary.Vente vente, List<ProduitVendu> vendu)
        {
            Office office = new Office();
            office.CompanyName = "DeLaFlor Corporation";
            office.CompanyInfo = "Tel:+228 99 34 12 11 Email:delaflor@flor.com";
            office.GetClient = client;
            office.GetVente = vente;
            office.ProduitVendus = vendu;

            if (office.GenFacture(System.IO.Path.GetFullPath("Files\\Facture_Prototype.dotx"), InfoChecker.SaveLoc("Eliano", "facture")))
                MessageBox.Show("Ok");
            else
                MessageBox.Show("Bad");
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

        private ProduitVendu GetProduitVendu(UIElement cbox, UIElement tb)
        {
            ProduitVendu pv = new ProduitVendu();
            ComboBox cb = cbox as ComboBox;
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
            if(!IsCommand)
                UpdateStockAfterSell(produit.Id, quantite);//// Mettre à jour la quantité des produits
            return pv;
        }

        private EasyManagerLibrary.Produit SelectedProductId(string item)
        {
            var prod = DbManager.GetProduitByName(item);
            return prod;
        }

        private void FilledDropDown(List<string> lst)
        {
            CbProdList.ItemsSource = lst;
        }

        private void FilledDropDownCmd()
        {
            CbCmdList.ItemsSource = Commande;
        }
        private void FilledDropDownClient()
        {
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProduitList = ProdList();
            GetCommandeList();
            GetClientList();
            FilledDropDown(ProduitList);
            FilledDropDownCmd();
            FilledDropDownClient();
        }

        private void Reset()
        {
            CbCmdList.SelectedIndex = 0;
            CbProdList.SelectedIndex = 0;
            CbClientList.SelectedIndex = 0;
            txtprod.Text = string.Empty;
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
            Commande.Add(Properties.Resources.MakeSelection);
            var rslt = DbManager.GetAll<EasyManagerLibrary.VenteCredit>();
            foreach (var item in rslt)
            {
                Commande.Add(InfoChecker.FormatIdent(item.Id));
            }
        }

        private void GetClientList()
        {
            ListClient.Add(Properties.Resources.MakeSelection);
            var rslt = DbManager.GetAll<EasyManagerLibrary.Client>();
            foreach (var item in rslt)
            {
                ListClient.Add(item.Nom);
            }
        }

        private bool UpdateStockAfterSell(int ProdId, int Quantitevendu)
        {
            try
            {
                var oldprod = DbManager.GetProduitById(ProdId);
                double newquantiterestante = Math.Round((oldprod.QuantiteRestante - Quantitevendu),2);
                // Update
                oldprod.QuantiteRestante = newquantiterestante;
                return DbManager.UpDate(oldprod, ProdId);
            }
            catch
            {
                return false;
            }
        }

        private void CbCmdList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems[0].ToString();
            if (item == Properties.Resources.MakeSelection)
            {
                SelectedCommandId = 0;
                // Active la selection de client
                FilledDropDown(ProduitList);
                CbClientList.IsEnabled = true;
                IsCommand = false;
            }
            else
            {
                SelectedCommandId = int.Parse(item);
                // Désactive la selection de client
                CbClientList.IsEnabled = false;
                CbClientList.SelectedIndex = 0;
                // Liste la list des produits commandés
                FilledDropDown(CommandedProduct(SelectedCommandId));
                IsCommand = true;
            }
        }

        private void CbClientList_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private bool IsOkQuantityCmd(int CmdId,string prodname,int quantite)
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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = sender as Vente;
            var ah = window.ActualHeight;
            //lstScroll.Height= ah - ah / 5;
            lstScroll.MaxHeight = ah - ah / 2;

        }
    }
}
