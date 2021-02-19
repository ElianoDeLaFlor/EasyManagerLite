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
    /// Interaction logic for ProduitStateInfo.xaml
    /// </summary>
    public partial class ProduitStateInfo : UserControl
    {
        public string Title { get; set; } = "Title";
        public string Quantity { get; set; } = "47";
        public string Couleur { get; set; } = "LightGreen";
        public ProduitStateInfo()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
