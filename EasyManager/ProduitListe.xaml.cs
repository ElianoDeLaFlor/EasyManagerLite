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

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for ProduitListe.xaml
    /// </summary>
    public partial class ProduitListe : Window
    {
        public ProduitListe()
        {
            InitializeComponent();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TxtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            

        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void ChkFilter_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void ChkFilter_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
        }
        private void OnRowSelected(object sender, RoutedEventArgs e)
        {
            //var row = sender as DataGridRow;
            //row.Foreground = new SolidColorBrush(Colors.Green);
        }

        private void OnCellSellected(object sender, RoutedEventArgs e)
        {
            var cell = sender as DataGridCell;
            //cell.Background = "{DynamicResource SecondaryHueLightBrush}"
            //cell.Background= new SolidColorBrush(ColorZoneMode.Accent);
        }
    }
}
