using EasyManagerDb;
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
    /// Interaction logic for CategorieUC.xaml
    /// </summary>
    public partial class CategorieUC : UserControl
    {
        public EasyManagerLibrary.Categorie categories { get; set; }
        public Home GetHome { get; set; }
        public CategorieUC()
        {
            InitializeComponent();
        }
        public CategorieUC(Home h)
        {
            InitializeComponent();
            GetHome = h;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string lblcontent = txtlabel.Text.ToLower();
            if (!Check(lblcontent))
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (txtlabel.Text.Length <= 2)
            {
                MessageBox.Show(Properties.Resources.ShortEnter, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (LibelleExist(lblcontent))
            {
                MessageBox.Show(Properties.Resources.LblExist, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            GetCategorie();
            if (DbManager.Save(categories))
            {
                MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                Reset();
            }
            else
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);

        }
        private void GetCategorie()
        {
            categories = new EasyManagerLibrary.Categorie();
            categories.Libelle = txtlabel.Text.ToLower();
            categories.Description = txtDescription.Text.Length <= 3 ? txtlabel.Text : txtDescription.Text;
        }

        private bool Check(string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        private bool LibelleExist(string str)
        {
            return DbManager.GetCategorieByLibelle(str) != null;
        }

        private void Reset()
        {
            txtlabel.Text = String.Empty;
            txtDescription.Text = String.Empty;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.Category;
                GetHome.MainTitle.Text = Properties.Resources.Category;
            }
        }
    }
}
