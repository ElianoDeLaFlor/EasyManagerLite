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
using EasyManagerDb;
using EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for PasseChange.xaml
    /// </summary>
    public partial class PasseChange : Window
    {
        private int UserId {get; set;}
        private string newpass { get; set; }
        private string oldpass { get; set; }
        private string confirmation { get; set; }
        private Utilisateur User { get; set; } = new Utilisateur();
        public PasseChange()
        {
            InitializeComponent();
        }

        public PasseChange(int userId)
        {
            InitializeComponent();
            UserId = userId;
        }

        public PasseChange(Utilisateur user)
        {
            InitializeComponent();
            User = user;
            UserId = User.Id;
        }

        private void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtoldpass.Password) || string.IsNullOrWhiteSpace(txtnewpass.Password))
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (txtoldpass.Password.Equals(txtnewpass.Password))
            {
                MessageBox.Show(Properties.Resources.OldPassEqualNewPass, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            GetEntries();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //User = DbManager.GetById<UtilisateurId>(UserId);
        }

        private void GetEntries()
        {
            oldpass = txtoldpass.Password;
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

            //Check existing password
            var isokoldpass = User.Password.Equals(InfoChecker.Encrypt(oldpass));

            //Update password and passdate
            if (isokoldpass)
            {
                var query = $"UPDATE Utilisateur SET Password='{InfoChecker.Encrypt(newpass)}',PassDate='{InfoChecker.AjustDateWithTimeYMDT(InfoChecker.NextDate(DateTime.Now, 45))}' WHERE Id={User.Id}";
                if (DbManager.UpdateCustumQuery(query))
                {
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    Reset();
                }
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }
            else
                MessageBox.Show(Properties.Resources.PasswordIncorrect, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }

        private void Reset()
        {
            txtconf.Clear();
            txtnewpass.Clear();
            txtoldpass.Clear();
        }
    }
}
