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
using EasyLib = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for StockStateUC.xaml
    /// </summary>
    public partial class StockStateUC : UserControl
    {
        public double TotalItem { get; set; }
        public double LeftItem { get; set; }
        public double SellItem { get; set; }
        public double ProgressValue { get; set; }

        public string Colors { get; set; }
        public EasyManagerLibrary.Categorie Categorie { get; set; }

        public string Rapport { get { return $"{SellItem} / {TotalItem}"; } }

        public StockStateUC()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void PopupBox_Opened(object sender, RoutedEventArgs e)
        {
            CatItemsStatusPnl.Children.Clear();
            foreach (var item in CatItemStatus(Categorie))
            {
                CatItemsStatusPnl.Children.Add(item);
                CatItemsStatusPnl.Children.Add(new Separator());
            }
        }

        /// <summary>
        /// Génère la carte de statu des items d'une catégorie
        /// </summary>
        private List<StockItemStateUC> CatItemStatus(EasyLib.Categorie categorie)
        {
            //Liste des tules
            List<StockItemStateUC> stockItemStates = new List<StockItemStateUC>();

            //les items de la catégorie
            string query = $"SELECT * FROM Produit WHERE CategorieId={categorie.Id}";
            //List<EasyLib.Produit> ProdList = DbManager.CustumQuery<EasyLib.Produit>(query);
            List<EasyLib.Produit> ProdList = DbManager.GetByColumnName<EasyLib.Produit>("CategorieId", categorie.Id.ToString());
            int count = 0;
            //Les items de la atégorie
            foreach (var item in ProdList)
            {
                //Liste des produits par catégorie
                double qty = 0;
                double rst = 0;
                double vendu;
                double progress;

                //La quantité de produit par catégorie
                qty += item.QuantiteTotale;
                //La quantité de produit restant par catégorie
                rst += item.QuantiteRestante;

                //La quantité de produit vendu
                vendu = qty - rst;
                progress = (rst / qty) * 100;

                StockItemStateUC stockItemState = new StockItemStateUC();
                stockItemState.TotalItem = qty;
                stockItemState.SellItem = vendu;
                stockItemState.LeftItem = rst;
                stockItemState.ProgressValue = Math.Floor(progress);
                stockItemState.Colors = count % 2 == 0 ? "DeepSkyBlue" : "CadetBlue";
                stockItemState.ItemName = item.Nom;
                stockItemStates.Add(stockItemState);
                count++;
            }
            return stockItemStates;
        }
    }
}
