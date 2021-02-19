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
    /// Interaction logic for CategorieTuile.xaml
    /// </summary>
    public partial class CategorieTuile : UserControl
    {
        public EasyManagerLibrary.Categorie GetCategorie { get; set; }
        public CategorieListeUC GetListeUC { get; set; }
        public CategorieTuile()
        {
            InitializeComponent();
            DataContext = this;
        }

        public CategorieTuile(CategorieListeUC categorieListeUC)
        {
            InitializeComponent();
            GetListeUC = categorieListeUC;
            DataContext = this;

        }

        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            var prodbycat = DbManager.GetByColumnName<EasyManagerLibrary.Produit>("CategorieId", GetCategorie.Id.ToString());
            if (prodbycat.Count==0)
            {
                //Delete
                if (!DbManager.Delete<EasyManagerLibrary.Categorie>(GetCategorie.Id))
                    MessageBox.Show(Properties.Resources.DeleteError, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    GetListeUC.Load();
                }
            }
            else
            {
                //Can't be delete
                MessageBox.Show(Properties.Resources.CatCannotBeDeleted, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnedit_Click(object sender, RoutedEventArgs e)
        {
            Categorie categorie = new Categorie(GetCategorie.Id);
            categorie.ShowDialog();
            GetListeUC.Load();
        }
    }
}
