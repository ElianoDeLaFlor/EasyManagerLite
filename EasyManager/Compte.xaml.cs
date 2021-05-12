using EasyManagerDb;
using EasyManagerLibrary;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Compte : Window
    {
        public Utilisateur GetUtilisateur { get; set; } = new Utilisateur();
        public bool FirstReset { get; set; }
        public bool FirstShown { get; set; }
        public bool SecondShown { get; set; }
        public bool ThirdShown { get; set; }
        public Window Window { get; set; } = null;
        public bool IsFirstTime { get; set; }
        public bool IsCreation { get; set; } = false;
        public bool Refresh { get; set; } = true;
        public bool IsAdminEdit { get; set; } = false;
        public Utilisateur User { get; set; } = new Utilisateur();
        public UserListUC UserList { get; set; }
        public Compte()
        {
            InitializeComponent();
        }
        //90762233
        public Compte(Utilisateur user)
        {
            InitializeComponent();
            User = user;
        }
        public Compte(Utilisateur user, bool isadmin)
        {
            InitializeComponent();
            User = user;
            IsAdminEdit = isadmin;
        }

        public Compte(bool first)
        {
            InitializeComponent();
            IsFirstTime = first;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LstRoles.ItemsSource = RolesList();
            if (!IsCreation)
            {          
                LstRoles.SelectedValue = User.RoleLibelle;
                txtnom.Text = User.Nom;
                txtprenom.Text = User.Prenom;
                txtlogin.Text = User.Login;
                pnlconfirmation.Visibility = Visibility.Collapsed;
                pnlpasse.Visibility = Visibility.Collapsed;
                if (IsAdminEdit)
                {
                    // admin
                    LstRoles.IsEnabled = true;
                    txtnom.IsEnabled = false;
                    txtprenom.IsEnabled = false;
                    txtlogin.IsEnabled = false;
                }
                else
                {
                    LstRoles.IsEnabled = false;
                    txtnom.IsEnabled = true;
                    txtprenom.IsEnabled = true;
                    txtlogin.IsEnabled = true;
                }
            }        

        }

        private void CreateAppId()
        {

        }
        private List<string> RolesList()
        {
            var lst = new List<string>();
            lst.Add(Properties.Resources.MakeSelection);
            var roles = DbManager.GetAll<Role>();
            if (IsFirstTime)
                lst.Add(roles[0].Libelle);
            else
                foreach (var item in roles)
                    lst.Add(item.Libelle);

            return lst;
        }

        private void Txtnom_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void FillUser()
        {
            GetUtilisateur.Nom = txtnom.Text.ToLower();
            GetUtilisateur.Prenom = txtprenom.Text.ToLower();
            GetUtilisateur.Login = txtlogin.Text.ToLower();
        }
        private void FillUserInfo()
        {
            GetUtilisateur.Nom = txtnom.Text.ToLower();
            GetUtilisateur.Prenom = txtprenom.Text.ToLower();
            GetUtilisateur.Password = InfoChecker.Encrypt(txtpasse.Password);
            GetUtilisateur.Login = txtlogin.Text.ToLower();
            GetUtilisateur.PassDate = InfoChecker.NextDate(DateTime.Now, 45);
            GetUtilisateur.RoleLibelle = LstRoles.SelectedItem as string;
        }

        /// <summary>
        /// returns true if everyting is ok
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="lbl"></param>
        /// <returns></returns>
        private bool CheckName(TextBox tb)
        {
            if (!InfoChecker.IsNameOk(tb.Text))
            {
                //lbl.Visibility = Visibility.Visible;
                //lbl.Text = Properties.Resources.InvalidCaracters;
                //Height += 20;
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ResetInfo(TextBlock lbl)
        {

            lbl.Visibility = Visibility.Collapsed;
            //Height -= 20;
        }

        private void Btncreate_Click(object sender, RoutedEventArgs e)
        {
            if (IsCreation)
            {
                SaveUser();
            }
            else
            {
                Edition();
            }
            

        }

        private void SaveUser()
        {
            if (!CheckName(txtnom) || !CheckName(txtprenom))
            {
                MessageBox.Show(Properties.Resources.CheckEntre, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (IsExistingUser(txtlogin.Text))
            {
                MessageBox.Show(Properties.Resources.LoginExist, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (!CheckPass())
            {
                MessageBox.Show(Properties.Resources.PassError, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (LstRoles.SelectedIndex == -1 || LstRoles.SelectedIndex == 0)
            {
                MessageBox.Show(Properties.Resources.SelectRole, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            FillUserInfo();
            if (DbManager.Save(GetUtilisateur))
            {
                MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Information);
                if (IsFirstTime)
                    Close();
                ResetForm();
            }
            else
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);

        }

        private void Edition()
        {
            if (!CheckName(txtnom) || !CheckName(txtprenom))
            {
                MessageBox.Show(Properties.Resources.CheckEntre, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }


            FillUser();

            if (IsAdminEdit)
            {
                string queryy = $"UPDATE Utilisateur SET RoleLibelle='{LstRoles.SelectedValue as string}' WHERE Id={User.Id}";
                if (DbManager.UpdateCustumQuery(queryy))
                {
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    Close();
                }
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string query = $"UPDATE Utilisateur SET Nom='{GetUtilisateur.Nom}', Prenom='{GetUtilisateur.Prenom}', Login='{GetUtilisateur.Login}' WHERE Id={User.Id}";
            if (DbManager.UpdateCustumQuery(query))
            {
                MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Information);
                Close();
            }
            else
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Txtprenom_LostFocus(object sender, RoutedEventArgs e)
        {


        }

        private void Txtconfirmation_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!CheckPass())
                MessageBox.Show(Properties.Resources.ConfirmationError, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);

        }

        private bool CheckPass()
        {
            bool[] b = new bool[2];


            b[1] = InfoChecker.IsPassOk(txtpasse.Password);

            b[0] = InfoChecker.IsSame(txtpasse.Password, txtconfirmation.Password);

            return !b.Contains(false);
        }

        private void ResetForm()
        {
            txtnom.Clear();
            txtprenom.Clear();
            txtpasse.Clear();
            txtconfirmation.Clear();
            txtlogin.Clear();
        }

        private void Btncancel_Click(object sender, RoutedEventArgs e)
        {
            if (IsCreation)
            {
                if (Window != null)
                    Window.Show();
            }
            else
            {
                Refresh = false;
                Close();
            }
            
        }

        private bool IsExistingUser(string n, string p)
        {
            return DbManager.GetUserByNL(n, p) != null;
        }

        private bool IsExistingUser(string n)
        {
            return DbManager.GetUserByLogin(n) != null;
        }

        private void Txtlogin_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        private void PerformClick(Button btn)
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(btn);
            IInvokeProvider invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProvider.Invoke();
        }

        private void Messagee_Box(string msg, string actiontitle, SolidColorBrush colorBrush)
        {
            MsgContent.Content = msg;
            MsgContent.Foreground = colorBrush;
            MsgContent.FontSize = 20;
            btnOk.Content = actiontitle;
            PerformClick(btnmsgbox);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsAdminEdit && Refresh)
            {
                UserList.Load();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = sender as Compte;
            var ah = window.ActualHeight;
            scroll.Height = ah - ah / 5;
        }
    }
}
