using EasyManagerDb;
using EasyManagerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for VenteCredit.xaml
    /// </summary>
    public partial class VenteACredit : Window
    {
        public List<string> ProduitList { get; set; } = new List<string>();
        public List<ProduitCredit> ListPC { get; set; } = new List<ProduitCredit>();
        public EasyManagerLibrary.Client SelectedClient { get; set; }
        public EasyManagerLibrary.Produit SelectedProduct { get; set; }
        public bool CanBeDeleted { get; set; } = true;
        public ComboBox CbFocusedId { get; set; } = null;
        public TextBox TxtFocusedId { get; set; } = null;
        public bool QuantityError { get; set; }

        public VenteACredit()
        {
            InitializeComponent();
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
                MessageBox.Show(Properties.Resources.QuantiteManquante, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Stop);
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
                MessageBox.Show(Properties.Resources.QuantiteManquante, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Stop);
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ListPC.Clear();
            bool stop = false;
            CanBeDeleted = true;
            var cmd = new EasyManagerLibrary.VenteCredit();
            cmd.Date = DateTime.UtcNow;
            if (SelectedClient == null)
            {
                //Une erreur est sur venue
                MessageBox.Show(Properties.Resources.ForgetClient, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            cmd.ClientId = SelectedClient.Id;
            cmd.UtilisateurId = 1; // connected user id
            decimal sommevente = 0;
            ProduitCredit vendu;
            List<bool> saverslt = new List<bool>();
            var children = ProdGrid.Children;
            for (int i = 0; i < children.Count; i += 2)
            {
                vendu = GetProduitCommande(children[i], children[i + 1]);
                if(vendu==null)
                {
                    stop = true;
                    break;
                }
                sommevente += vendu.Montant;
                ListPC.Add(vendu);
            }
            if (stop)
            {
                //Une erreur est sur venue
                if (QuantityError)
                    MessageBox.Show(Properties.Resources.QuantiteManquante, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Stop);
                else
                    MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            cmd.Montant = sommevente;
            cmd.MontantRestant = sommevente;
            int rslt = DbManager.SaveData(cmd);// save commande
            if (rslt > 0)
            {
                foreach (var item in ListPC)
                {
                    // ajout des produits commandés à la base de données
                    item.CommandeId = rslt;
                    saverslt.Add(DbManager.Save(item));
                }
                if (!saverslt.Contains(false))
                {
                    // tous les produits commandés ont bien été enregistrés

                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void PerformClick(Button btn)
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(btn);
            IInvokeProvider invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProvider.Invoke();
        }

        
        
        /// <summary>
        /// Effaces le formulaire
        /// </summary>
        private void Reset()
        {
            CbCatList.SelectedIndex = 0;
            CbProdList.SelectedIndex = 0;
            txtprod.Text = string.Empty;
            CbFocusedId = null;
            TxtFocusedId = null;
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
        private ProduitCredit GetProduitCommande(UIElement cbox, UIElement tb)
        {
            ProduitCredit pc = new ProduitCredit();
            ComboBox cb = cbox as ComboBox;
            if (cb.SelectedItem == null)
                return null;
            string selection = cb.SelectedItem.ToString();
            if(selection=="Selectionnez")
            {
                return null;
            }
            var produit = SelectedProductId(selection);
            int prodid = produit.Id;
            TextBox txt = tb as TextBox;
            if(txt.Text.Length==0)
            {
                return null;
            }
            int quantite = int.Parse(txt.Text);
            if(quantite==0)
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
            pc.PrixUnitaire = produit.PrixUnitaire;
            UpdateStockAfterSell(produit.Id, quantite);
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProduitList = ProdList();
            FilledDropDown();
            FilledDropDownClient();
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
            lstcat.Add(Properties.Resources.MakeSelection);//index 0
            foreach (var item in lst)
            {
                lstcat.Add(item.Nom);
            }
            return lstcat;
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
                oldprod.QuantiteRestante = newquantiterestante;
                return DbManager.UpDate(oldprod, ProdId);
            }
            catch
            {
                return false;
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

        private void Sample2_DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            //Console.WriteLine("SAMPLE 2: Closing dialog with parameter: " + (eventArgs.Parameter ?? ""));
        }

        private void btnNewClient_Click(object sender, RoutedEventArgs e)
        {
            Client client = new Client();
            client.ShowDialog();
            FilledDropDownClient();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = sender as VenteACredit;
            var ah = window.ActualHeight;
            //lstScroll.Height= ah - ah / 5;
            lstScroll.MaxHeight = ah - ah / 3;
        }
    }
}
