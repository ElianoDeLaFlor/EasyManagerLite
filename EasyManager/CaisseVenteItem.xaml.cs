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
    /// Interaction logic for CaisseVenteItem.xaml
    /// </summary>
    public partial class CaisseVenteItem : UserControl
    {
        public EasyManagerLibrary.ProduitVendu GetProduitVendu { get; set; }
        public string ProductName { get => GetProduitName(GetProduitVendu.ProduitId); }  
        public CaisseVenteItem()
        {
            InitializeComponent();
            DataContext = this;
        }

        private string GetProduitName(int id)
        {
            return DbManager.GetById<EasyManagerLibrary.Produit>(id).Nom;
        }
    }
}
