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
    /// Interaction logic for SupplierUC.xaml
    /// </summary>
    public partial class SupplierUC : UserControl
    {
        public SupplierUC()
        {
            InitializeComponent();
        }

        private void txtlabel_KeyUp(object sender, KeyEventArgs e)
        {
            CheckEntres(txtlabel);
        }

        private void CheckEntres(TextBox tb)
        {
            tb.Text = InfoChecker.AlphabetOnlyRegExp(tb.Text);
            tb.SelectionStart = tb.Text.Length;
        }

        private void Check(TextBox tb)
        {
            tb.Text = InfoChecker.NumericOnlyRegExp(tb.Text);
            tb.SelectionStart = tb.Text.Length;
        }

        private EasyManagerLibrary.Supplier GetSupplier()
        {
            EasyManagerLibrary.Supplier supplier = new EasyManagerLibrary.Supplier();
            supplier.Contact = txtcontact.Text.Trim();
            supplier.Nom = txtlabel.Text.Trim().ToLower();
            supplier.Email = txtmail.Text.Trim();
            return supplier;
        }

        private void txtcontact_KeyUp(object sender, KeyEventArgs e)
        {
            Check(txtcontact);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSupplierNameExist(txtlabel.Text.ToLower()))
            {
                if (!InfoChecker.MailRegExp(txtmail.Text))
                {
                    if (!string.IsNullOrWhiteSpace(txtcontact.Text) || !string.IsNullOrWhiteSpace(txtlabel.Text))
                    {
                        if (DbManager.Save(GetSupplier()))
                            MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                        else
                            MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.EmailError, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.SupplierExist, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private bool IsSupplierNameExist(string str)
        {
            var count = (DbManager.GetByColumnName<EasyManagerLibrary.Supplier>("Nom", str)).Count();
            return count > 0; 
        }
    }
}
