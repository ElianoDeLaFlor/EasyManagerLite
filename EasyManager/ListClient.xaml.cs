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
using ClassL = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for ListClient.xaml
    /// </summary>
    public partial class ListClient : Window
    {
        public object Selected { get; set; }
        public List<ClassL.Client> Clients { get; set; }
        /// <summary>
        /// Liste des clients à supprimer
        /// </summary>
        public List<ClassL.DeleteRef> DeleteList { get; set; } = new List<DeleteRef>();
        public List<ClassL.Client> GetClients { get; set; }
        private int RowIndex = 0;
        public bool IsFilter { get; set; }
        public ListClient()
        {
            InitializeComponent();
            //DataContext = GetClient();
            //GetClients = GetClient();
            //Clients = GetClient();
            GetClients = testList();
            Clients = testList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetData(Clients);
            btnSearch.IsEnabled = true;
            ChkFilter.IsChecked = false;
        }

        private List<ClassL.Client> testList()
        {
            List<ClassL.Client> l = new List<ClassL.Client>();
            for(int i=1;i<1000; i++)
            {
                l.Add(client(i));
            }
            return l;
        }
        private ClassL.Client client(int i)
        {
            ClassL.Client c = new ClassL.Client();
            c.Id = i;
            c.Nom = $"nom{i}";
            c.Prenom = $"prenom{i}";
            c.Contact = $"{i}";
            return c;
        }

        private List<ClassL.Client> GetClient()
        {
            return DbManager.GetAll<ClassL.Client>();
        }

        private void SetData(List<ClassL.Client> clients)
        {
            datagrid.ItemsSource = Clients;
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
            ManageGroupAction();
        }

        /// <summary>
        /// Affiche/Cache le button action de groupe
        /// </summary>
        private void ManageGroupAction()
        {
            if (DeleteList.Count > 1)
                GroupAction.Visibility = Visibility.Visible;
            else
                GroupAction.Visibility = Visibility.Hidden;
        }

        private void OnUnChecked(object sender, RoutedEventArgs e)
        {
            DeleteRef delete = new DeleteRef();
            delete.CanBeDelete = true;
            delete.RowIndex = RowIndex;
            var d = (from del in DeleteList where del.RowIndex == delete.RowIndex select del).Single();
            int deleteindex =DeleteList.IndexOf(d);
            DeleteList.RemoveAt(deleteindex);
            ManageGroupAction();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var row = sender as DataGridRow;
            var c = row.Item as ClassL.Client;
            RowIndex = c.Id;
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
            if (MessageBox.Show(Properties.Resources.DeleteDialog, Properties.Resources.MainTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
            Client client = new Client(RowIndex);
            client.ShowDialog();
            Clients = GetClient();
            SetData(Clients);
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
            var rslt = Search(TxtSearch.Text);
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
            if (rslt == null || rslt.Count==0)
            {
                MessageBox.Show(Properties.Resources.EmptyResult, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //Message_Box(Properties.Resources.EmptyResult, Properties.Resources.Accept, new SolidColorBrush(Colors.SteelBlue));
                ResetList();
                return;
            }
            
                Clients = null;
                Clients=(rslt);
                SetData(Clients);
            
        }

        private void ResetList()
        {
            //Clients = GetClient();
            Clients = testList();
            SetData(Clients);
        }
        private List<ClassL.Client> Search(string critere)
        {
             var rslt = from c in GetClients where c.Nom.Contains(critere) || c.Prenom.Contains(critere) || c.Id.ToString().Contains(critere) || c.Contact.Contains(critere) select c;
            return rslt.ToList();
        }

        private List<ClassL.Client> SingleSearch(string critere)
        {
            try
            {
                var rslt = from c in GetClients where c.Nom==(critere) || c.Prenom==(critere) || c.Id.ToString()==(critere) || c.Contact==(critere) select c;
                return rslt.ToList();
            }
            catch
            {
                return null;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = sender as ListClient;
            var ah = window.ActualHeight;
            datagrid.Height=ah-ah/3;
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

        private void PerformClick(Button btn)
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(btn);
            IInvokeProvider invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProvider.Invoke();
        }

        private void GroupDelete(object sender, RoutedEventArgs e)
        {
            foreach (var item in DeleteList)
            {
                Delete(item.RowIndex);
            }
            MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            ResetList();
            DeleteList.Clear();
            ManageGroupAction();
        }

        private bool Delete(int id)
        {
            bool rslt = DbManager.Delete<ClassL.Client>(id);
            return rslt;
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("test");
        }
    }

}
