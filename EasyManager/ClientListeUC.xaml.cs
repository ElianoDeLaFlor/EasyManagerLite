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
using EasyManagerDb;
using EasyManagerLibrary;
using ClassL = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for ClientListeUC.xaml
    /// </summary>
    public partial class ClientListeUC : UserControl
    {
        #region Variable declaration

        private List<ClassL.Client> Clients { get; set; }

        /// <summary>
        /// Liste des Clients à supprimer
        /// </summary>
        private List<DeleteRef> DeleteList { get; set; } = new List<DeleteRef>();

        /// <summary>
        /// Liste des clients dans la base de données
        /// </summary>
        public List<ClassL.Client> GetClients { get; set; }

        /// <summary>
        /// Contient les données d'un row
        /// </summary>
        public ClassL.Client Client { get; set; }

        private int RowIndex = 0;
        public bool IsFilter { get; set; }
        public Home GetHome { get; set; }
        public string SaveLocation { get; set; }
        public ShowDocument showDocument { get; set; }

        #endregion

        public ClientListeUC()
        {
            InitializeComponent();
        }

        public ClientListeUC(Home h)
        {
            InitializeComponent();
            GetHome = h;
            GetClients = GetAllClients();
            Clients = GetAllClients();
            Datagrid.Width = Math.Abs((GetHome.ActualWidth) - 30);
            GetHome.SizeChanged += GetHome_SizeChanged;
        }

        private void GetHome_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Datagrid.Width = (GetHome.ActualWidth) - 30;      
        }

        private Utilisateur GetUtilisateur(int id)
        {
            return DbManager.GetById<Utilisateur>(id);
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

        private void Research()
        {
            var rslt = Search(TxtSearch.Text.ToLower());
            if (rslt == null)
                ResetList();
            if (rslt.Count > 0)
            {
                Clients = null;
                Clients = rslt;
                SetData(Clients);
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
                MessageBox.Show(Properties.Resources.EmptyResult, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                ResetList();
                return;
            }

            Clients = null;
            Clients = (rslt);
            SetData(Clients);
        }

        private void SetData(List<ClassL.Client> commande)
        {
            Datagrid.ItemsSource = commande;
        }

        private List<ClassL.Client> Search(string critere)
        {
            var rslt = from client in GetClients where client.Id.ToString().Contains(critere) || client.Nom.ToLower().Contains(critere) || client.Prenom.ToLower().Contains(critere) || client.Contact.Contains (critere) select client;
            return rslt.ToList();
        }

        private List<ClassL.Client> SingleSearch(string critere)
        {
            try
            {
                var rslt = from c in GetClients where c.Contact.Equals(critere,StringComparison.CurrentCultureIgnoreCase) || c.Nom.Equals(critere, StringComparison.CurrentCultureIgnoreCase) || c.Prenom.Equals(critere, StringComparison.CurrentCultureIgnoreCase) select c;
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
            Clients = GetAllClients();
            GetClients = Clients;
            SetData(Clients);
        }

        /// <summary>
        /// Retourne tous les clients de la base de données
        /// </summary>
        /// <returns></returns>
        private List<ClassL.Client> GetAllClients()
        {
            var rslt = DbManager.GetAll<ClassL.Client>();
            return rslt;
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

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var row = sender as DataGridRow;
            Client = row.Item as ClassL.Client;
            RowIndex = Client.Id;
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
            var cell = sender as DataGridCell;
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
            if (MessageBox.Show(Properties.Resources.DeleteNotification, Properties.Resources.MainTitle,
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (Delete(RowIndex))
                {
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    Clients = GetClient();
                    SetData(Clients);
                }
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK,
                        MessageBoxImage.Error);

            }
        }

        private bool Delete(int id)
        {
            var lst = DbManager.GetById<Client>(id);
            if (lst == null)
                return true;
            return DbManager.Delete<Client>(id);
        }


        private void btnINFO_Click(object sender, RoutedEventArgs e)
        {
            VenteCreditListe vcl=new VenteCreditListe(RowIndex);
            vcl.ShowDialog();
        }
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetData(GetAllClients());
            btnSearch.IsEnabled = true;
            ChkFilter.IsChecked = false;
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.ListClient;
                GetHome.MainTitle.Text = Properties.Resources.ListClient;
            }
        }

        private List<ClassL.Client> GetClient()
        {
            return DbManager.GetAll<ClassL.Client>();
        }

        private void btnedit_Click(object sender, RoutedEventArgs e)
        {
            Client client = new Client(RowIndex);
            client.ShowDialog();
            Clients = GetClient();
            SetData(Clients);
        }

        private void allClient_Click(object sender, RoutedEventArgs e)
        {
            var allclient = GetAllClients();
            GetClients = allclient;
            SetData(allclient);
        }

        private void clientDette_Click(object sender, RoutedEventArgs e)
        {
            var lstventecreditnonsolde = DbManager.GetByColumnNameNot<VenteCredit>("MontantRestant", "0.0");
            var lstclientDette = new List<ClassL.Client>();
            lstclientDette.Clear();
            //Liste des ventes non soldées
            foreach (var item in lstventecreditnonsolde)
            {
                ClassL.Client client = new ClassL.Client();
                client = DbManager.GetById<ClassL.Client>(item.ClientId);
                if(!lstclientDette.Contains(client,new ClientComparer()))
                    lstclientDette.Add(client);
            }
            GetClients = lstclientDette;
            SetData(lstclientDette);

        }

        private void ClientSansDette_Click(object sender, RoutedEventArgs e)
        {
            var lstventecreditsolde = DbManager.GetByColumnName<VenteCredit>("MontantRestant", "0.0");
            var lstventecreditnonsolde = DbManager.GetByColumnNameNot<VenteCredit>("MontantRestant", "0.0");
            var lstclientDette = new List<ClassL.Client>();
            var lstclientSansDette = new List<ClassL.Client>();
            var lstclient = new List<ClassL.Client>();

            lstclient.Clear();
            lstclientDette.Clear();
            lstclientSansDette.Clear();
            //Liste des ventes soldées
            foreach (var item in lstventecreditsolde)
            {
                ClassL.Client client = new ClassL.Client();
                client = DbManager.GetById<ClassL.Client>(item.ClientId);
                lstclientSansDette.Add(client);
            }

            //Liste des ventes non soldées
            foreach (var item in lstventecreditnonsolde)
            {
                ClassL.Client client = new ClassL.Client();
                client = DbManager.GetById<ClassL.Client>(item.ClientId);
                lstclientDette.Add(client);
            }

            //liste des client sans dette
            foreach (var item in lstclientSansDette)
            {
                if (!lstclientDette.Contains(item, new ClientComparer()))
                    lstclient.Add(item);
            }
            GetClients = lstclient;
            SetData(lstclient);
        }

        private async void Btnprint_Click(object sender, RoutedEventArgs e)
        {
            //var watch = System.Diagnostics.Stopwatch.StartNew();
            GetHome.DataContextt.Progress = Visibility.Visible;
            if (await PrintAsync(Clients))
            {
                showDocument = new ShowDocument(SaveLocation);
                GetHome.DataContextt.Progress = Visibility.Hidden;
                //watch.Stop();
                //Console.WriteLine($"premier arret: {watch.ElapsedMilliseconds}");
                showDocument.ShowDialog();
            }
            else
            {
                GetHome.DataContextt.Progress = Visibility.Hidden;
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool GenClientList(List<ClassL.Client> clients)
        {
            Office office = new Office();
            office.CompanyName = "DeLaFlor Corporation";
            office.CompanyTel = "Tel:+228 99 34 12 11";
            office.CompanyEmail = "Email:delaflor@flor.com";
            office.Code = Properties.Resources.Number;
            office.ClientContact = Properties.Resources.Contact;
            office.ClientFirstName = Properties.Resources.Prenom;
            office.ClientLastName= Properties.Resources.Nom;
            office.ClientList = Properties.Resources.ListClient;
            office.GetClientLists = clients;
            
            SaveLocation = InfoChecker.SaveLoc(Properties.Resources.Client, Properties.Resources.Client);
            return office.GenListeClient(System.IO.Path.GetFullPath("Files\\ListeClient_Prototype.dotx"), SaveLocation);
        }

        private bool Print(List<ClassL.Client> lst)
        {
            return GenClientList(lst);
        }

        private async Task<bool> PrintAsync(List<ClassL.Client> lst)
        {
            return await Task.Run(() => Print(lst));
        }
    }
}
