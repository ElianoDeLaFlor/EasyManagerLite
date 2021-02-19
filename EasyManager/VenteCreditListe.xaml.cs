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
using EasyManagerDb;
using EasyManagerLibrary;
using ClassL = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for VenteCreditListe.xaml
    /// </summary>
    public partial class VenteCreditListe : Window
    {
        #region Variable declaration
        private List<ClassL.VenteCredit> Commandes { get; set; }
        /// <summary>
        /// Liste des commandes à supprimer
        /// </summary>
        private List<ClassL.DeleteRef> DeleteList { get; set; } = new List<DeleteRef>();
        /// <summary>
        /// Liste des commandes dans la base de données
        /// </summary>
        public List<ClassL.VenteCredit> GetCommandes { get; set; }
        /// <summary>
        /// Contient les données d'un row
        /// </summary>
        public ClassL.VenteCredit VenteCredit { get; set; }
        /// <summary>
        /// Id de l'élément dans le row selectionné
        /// </summary>
        private int RowIndex = 0;
        private int ClientId { get; set; }
        public bool IsFilter { get; set; }
        #endregion
        public VenteCreditListe()
        {
            InitializeComponent();

            GetCommandes = GetAllCommande();
            Commandes = GetAllCommande();
        }

        public VenteCreditListe(int clientid)
        {
            InitializeComponent();
            title.Text = Properties.Resources.ListeAchatClient;
            GetCommandes = GetAllCommande(clientid);
            Commandes = GetAllCommande(clientid);
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
            c.Montant = 123*i;
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
            var rslt = DbManager.GetAll<ClassL.VenteCredit>();
            var lst = new List<ClassL.VenteCredit>();
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

        private List<ClassL.VenteCredit> GetAllCommande(int clientid)
        {
            var rslt = DbManager.GetByColumnName<ClassL.VenteCredit>("ClientId", clientid.ToString());
            var lst = new List<ClassL.VenteCredit>();
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
            var datagridcell = sender as DataGridCell;
            var chk = datagridcell.Content as CheckBox;
            //Checked

            DeleteRef delete = new DeleteRef();
            delete.CanBeDelete = true;
            delete.RowIndex = RowIndex;
            if (!DeleteList.Contains(delete, new DeleteRefComparer()))
                DeleteList.Add(delete);
            //ManageGroupAction();
        }
        /// <summary>
        /// Affiche/Cache le menu des action de groupe
        /// </summary>
        
        private void OnUnChecked(object sender, RoutedEventArgs e)
        {
            DeleteRef delete = new DeleteRef();
            delete.CanBeDelete = true;
            delete.RowIndex = RowIndex;
            var d = (from del in DeleteList where del.RowIndex == delete.RowIndex select del).Single();
            int deleteindex = DeleteList.IndexOf(d);
            DeleteList.RemoveAt(deleteindex);
            //ManageGroupAction();
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
            VenteCredit = row.Item as ClassL.VenteCredit;
            RowIndex = VenteCredit.Id;
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

        private List<ClassL.VenteCredit> Search(string critere)
        {
            var rslt = from c in GetCommandes where c.NomClient.Contains(critere) || c.UserName.Contains(critere) || c.Id.ToString().Contains(critere) || c.Date.ToShortDateString().Contains(critere) || c.Montant.ToString().Contains(critere) || c.MontantRestant.ToString().Contains(critere) || c.GetSolde().Contains(critere) select c;
            return rslt.ToList();
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = sender as VenteCreditListe;
            var ah = window.ActualHeight;
            Datagrid.Height = ah - ah / 3;
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
        
        private List<ClassL.VenteCredit> SingleSearch(string critere)
        {
            try
            {
                var rslt = from c in GetCommandes where c.UserName.Contains(critere) || c.NomClient.Contains(critere) || c.Id.ToString() == (critere) || c.Date.ToString() == (critere) || c.Montant.ToString()==critere || c.MontantRestant.ToString()==critere || c.GetSolde()==critere select c;
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
            Commandes = GetAllCommande();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetData(Commandes);
            btnSearch.IsEnabled = true;
            ChkFilter.IsChecked = false;
        }

        private void btnINFO_Click(object sender, RoutedEventArgs e)
        {
            ListeProduitCommende listeProduit = new ListeProduitCommende(RowIndex);
            listeProduit.ShowDialog();

        }

        private void Allventecredit_OnClick(object sender, RoutedEventArgs e)
        {
            Commandes = null;
            Commandes = DbManager.GetAll<VenteCredit>();
            SetData(Commandes);
        }

        private void Ventecreditsolde_OnClick(object sender, RoutedEventArgs e)
        {
            Commandes = null;
            Commandes = Servante.VenteCreditSolde();
            SetData(Commandes);
        }

        private void Ventecreditnonsolde_OnClick(object sender, RoutedEventArgs e)
        {
            Commandes = null;
            Commandes = Servante.VenteCreditNonSolde();
            SetData(Commandes);
        }

        private void btnprint_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
