using EasyManagerDb;
using EasyManagerLibrary;
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
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using ClassL = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for UserList.xaml
    /// </summary>
    public partial class UserList : Window
    {
        private List<Utilisateur> Users { get; set; }
        private List<DeleteRef> DeleteList { get; set; } = new List<DeleteRef>();
        public List<Utilisateur> GetUsers { get; set; }
        private int RowIndex = 0;
        public Utilisateur User { get; set; }
        public bool IsFilter { get; set; }
        public string SaveLocation { get; set; }
        public ShowDocument showDocument { get; set; }
        public CompanyInfo Company { get; set; } = new CompanyInfo();
        public bool Generated { get; set; }
        public bool IsChecked { get; set; } = true;
        public UserList()
        {
            InitializeComponent();
            GetUsers = GetAllUser();
            Users = GetAllUser();
        }

        private List<Utilisateur> GetAllUser()
        {
            return DbManager.GetAll<Utilisateur>();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var row = sender as DataGridRow;
            User = row.Item as Utilisateur;
            RowIndex = User.Id;
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

        private void btnINFO_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btndelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TxtSearch_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {

        }
        private void OnChecked(object sender, RoutedEventArgs e)
        {
            var datagridcell = sender as DataGridCell;
            var chk = datagridcell.Content as CheckBox;
            //Checked

            DeleteRef delete = new DeleteRef();
            delete.CanBeDelete = true;
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

        private void btnprint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btndeletechk_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
