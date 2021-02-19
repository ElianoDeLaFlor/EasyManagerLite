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
    /// Interaction logic for SupplierTile.xaml
    /// </summary>
    public partial class SupplierTile : UserControl
    {
        public EasyManagerLibrary.Supplier Supplier { get; set; }
        public SupplierList SupplierList { get; set; }
        public Home GetHome { get; set; }
        public SupplierTile()
        {
            InitializeComponent();
            DataContext = this;
        }
        public SupplierTile(SupplierList supplierList,Home home)
        {
            InitializeComponent();
            SupplierList = supplierList;
            GetHome = home;
            DataContext = this;
        }

        private void btnedit_Click(object sender, RoutedEventArgs e)
        {
            var editfrm = new Supplier(Supplier.Id,GetHome);
            editfrm.ShowDialog();
            SupplierList.Reload();
        }

        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            if (DbManager.Delete<EasyManagerLibrary.Supplier>(Supplier.Id))
                SupplierList.Reload();
        }

        private void Card_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void Card_MouseLeave(object sender, MouseEventArgs e)
        {

        }
    }
}
