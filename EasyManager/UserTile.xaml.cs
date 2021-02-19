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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for UserTile.xaml
    /// </summary>
    public partial class UserTile : UserControl
    {
        public Utilisateur User { get; set; }
        public Home GetHome { get; set; }
        public UserListUC UserList { get; set; }
        public UserTile()
        {
            InitializeComponent();
            DataContext = this;
        }
        public UserTile(UserListUC ul)
        {
            InitializeComponent();
            UserList = ul;
            DataContext = this;
        }

        private void Btnedit_Click(object sender, RoutedEventArgs e)
        {
            Compte compte = new Compte(User, true);
            compte.UserList = UserList;
            compte.ShowDialog();
        }

        private void Btndelete_Click(object sender, RoutedEventArgs e)
        {
            if (GetHome.DataContextt.ConnectedUser.Id == User.Id)
            {
                MessageBox.Show(Properties.Resources.UserIsConnected, Properties.Resources.MainTitle, MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                return;
            }
            if(MessageBox.Show(Properties.Resources.AreYouSureToContinue, Properties.Resources.MainTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                string query = $"UPDATE Utilisateur SET Deleted={true} WHERE Id={User.Id}";
                if (DbManager.UpdateCustumQuery(query))
                {
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    UserList.Load();
                }
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
