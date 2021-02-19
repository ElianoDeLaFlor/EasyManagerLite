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

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for PassForget.xaml
    /// </summary>
    public partial class PassForget : Window
    {
        private string newpass { get; set; }
        private string login { get; set; }
        private string confirmation { get; set; }
        public PassForget()
        {
            InitializeComponent();
        }

        private void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtlogin.Text) || string.IsNullOrWhiteSpace(txtnewpass.Password))
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            GetEntries();

        }

        private void GetEntries()
        {
            login = txtlogin.Text;
            newpass = txtnewpass.Password;
            confirmation = txtconf.Password;
            if (!InfoChecker.IsPassFormatOk(newpass))
            {
                MessageBox.Show(Properties.Resources.PassError, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            if(!newpass.Equals(confirmation, StringComparison.CurrentCulture))
            {
                MessageBox.Show(Properties.Resources.ConfirmationError, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            //Get User by login
            var user = DbManager.GetUserByLogin(login);

            //Update password and passdate
            if (user != null)
            {
                var query = $"UPDATE Utilisateur SET Password='{InfoChecker.Encrypt(newpass)}',PassDate='{InfoChecker.AjustDateWithTimeYMDT(InfoChecker.NextDate(DateTime.Now, 45))}' WHERE Id={user.Id}";
                //UpdatePassword(user.Id, InfoChecker.Encrypt(newpass),
                //InfoChecker.AjustDateWithTimeYMDT(InfoChecker.NextDate(DateTime.Now, 45)))
                if (DbManager.UpdateCustumQuery(query))
                {
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    Close();
                }
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }
            else
                MessageBox.Show(Properties.Resources.LoginIncorrect, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }
    }
}
