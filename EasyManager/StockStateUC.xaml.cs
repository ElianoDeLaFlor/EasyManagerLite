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
    }
}
