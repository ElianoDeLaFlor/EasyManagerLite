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
    /// Interaction logic for CompteUC.xaml
    /// </summary>
    public partial class CompteUC : UserControl
    {
        #region Variable declaration
        public Utilisateur GetUtilisateur { get; set; } = new Utilisateur();
        public Home GetHome { get; set; }

        public bool IsFirst { get; set; }

        public string RoleLibelle { get; set; }

        #endregion
        public CompteUC()
        {
            InitializeComponent();
        }

        public CompteUC(Home h)
        {
            InitializeComponent();
            GetHome = h;
            GetHome.SizeChanged += GetHome_SizeChanged;
        }
        public CompteUC(Home h,bool isfirsttime)
        {
            InitializeComponent();
            GetHome = h;
            container.MinWidth = GetHome.ActualWidth - 400;
            GetHome.SizeChanged += GetHome_SizeChanged;
            IsFirst = isfirsttime;
        }

        private void GetHome_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var home = sender as Home;
            //Width = home.ActualWidth-30;
            container.MinWidth =Math.Abs(GetHome.ActualWidth - 500);
        }

        private void FillUser()
        {
            GetUtilisateur.Nom = txtnom.Text.ToLower();
            GetUtilisateur.Prenom = txtprenom.Text.ToLower();
            GetUtilisateur.Password = InfoChecker.Encrypt(txtpasse.Password);
            GetUtilisateur.Login = txtlogin.Text.ToLower();
            GetUtilisateur.PassDate=InfoChecker.NextDate(DateTime.Now,45);
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
                return false;
            }
            else
            {
                return true;
            }
        }

        private void Btncreate_Click(object sender, RoutedEventArgs e)
        {
            SaveUser();
        }

        private void SaveUser()
        {
            if (!CheckName(txtnom) || !CheckName(txtprenom) || !InfoChecker.IsSame(txtconfirmation.Password, txtpasse.Password) || !CheckPass())
            {
                MessageBox.Show(Properties.Resources.CheckEntre, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (LstRoles.SelectedIndex == 0)
            {
                MessageBox.Show(Properties.Resources.SelectRole, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            FillUser();
            if (IsExistingUser(GetUtilisateur.Nom, GetUtilisateur.Prenom))
            {
                MessageBox.Show(Properties.Resources.UserExist, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (IsExistingUser(GetUtilisateur.Login))
            {
                MessageBox.Show(Properties.Resources.LoginExist, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                if (DbManager.Save(GetUtilisateur))
                {
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    if (IsFirst)
                        GetHome.MenuItemsListBox.SelectedIndex = 3;
                    else
                        ResetForm();
                }
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Txtprenom_LostFocus(object sender, RoutedEventArgs e)
        {


        }

        private void Txtconfirmation_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!CheckPass())
                MessageBox.Show(Properties.Resources.PassError, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);

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
            LstRoles.SelectedIndex = 0;
        }

        private bool IsExistingUser(string n, string p)
        {
            return DbManager.GetUserByNL(n, p) != null;
        }

        private bool IsExistingUser(string n)
        {
            return DbManager.GetUserByLogin(n) != null;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.CompteTitle;
                GetHome.MainTitle.Text = Properties.Resources.CompteTitle;
            }
            LstRoles.ItemsSource = RolesList();
        }

        private void OnlyAlphabet(TextBox tb)
        {
            tb.Text = InfoChecker.AlphabetOnlyRegExp(tb.Text);
            tb.SelectionStart = tb.Text.Length;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //var window = sender as CompteUC;
            //var ah = window.ActualHeight;
            //lstScroll.MaxHeight = ah - ah / 6;
            container.MinWidth = GetHome.ActualWidth - 400;
        }

        private List<string> RolesList()
        {
            var lst=new List<string>();
            lst.Add(Properties.Resources.MakeSelection);
            var roles = DbManager.GetAll<Role>();
            if (IsFirst)
                lst.Add(roles[0].Libelle);
            else
                foreach (var item in roles)
                    lst.Add(item.Libelle);

            return lst;
        }

        private void txtnom_KeyUp(object sender, KeyEventArgs e)
        {
            OnlyAlphabet(sender as TextBox);
        }

        private void txtprenom_KeyUp(object sender, KeyEventArgs e)
        {
            OnlyAlphabet(sender as TextBox);
        }

        private void Btncancel_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }
    }
}
