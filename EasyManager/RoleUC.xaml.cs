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
    /// Interaction logic for RoleUC.xaml
    /// </summary>
    public partial class RoleUC : UserControl
    {
        #region Varaible declaration
        public Home GetHome { get; set; }
        #endregion
        public RoleUC()
        {
            InitializeComponent();
        }

        public RoleUC(Home h)
        {
            InitializeComponent();
            GetHome = h;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtrole.Text))
            {
                if (txtrole.Text.Length > 3)
                {
                    var role = new EasyManagerLibrary.Role(0, txtrole.Text);
                    //Check if the role already exist
                    if (!RoleExist(txtrole.Text))
                    {
                        //Role doen't exist
                        if (EasyManagerDb.DbManager.Save(role))
                        {
                            MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                            Reset();
                        }
                        else
                        {
                            MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        //Role exist
                        MessageBox.Show(Properties.Resources.RoleExist, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);

                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.ShortEnter, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool RoleExist(string rolelibelle)
        {
            var rol = EasyManagerDb.DbManager.GetByColumnName<EasyManagerLibrary.Role>("Libelle", rolelibelle);
            return rol != null;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void Reset()
        {
            txtrole.Text = String.Empty;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.RoleTitle;
                GetHome.MainTitle.Text = Properties.Resources.RoleTitle;
            }
        }
    }
}
