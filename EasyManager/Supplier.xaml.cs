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
using System.Windows.Shapes;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for Supplier.xaml
    /// </summary>
    public partial class Supplier : Window
    {
        public int SupplierId { get; set; } = 0;
        public Home GetHome { get; set; }
        public Supplier()
        {
            InitializeComponent();
        }

        public Supplier(Home home)
        {
            InitializeComponent();
            GetHome = home;
        }

        public Supplier(int supplierid)
        {
            InitializeComponent();
            SupplierId= supplierid;
        }

        public Supplier(int supplierid,Home home)
        {
            InitializeComponent();
            SupplierId = supplierid;
            GetHome = home;
        }

        private EasyManagerLibrary.Supplier GetSupplier(int id)
        {
            var sup = DbManager.GetById<EasyManagerLibrary.Supplier>(id);
            return sup;
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
            supplier.Nom = txtlabel.Text.Trim();
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
                if (InfoChecker.MailRegExp(txtmail.Text))
                {
                    if (!string.IsNullOrWhiteSpace(txtcontact.Text) || !string.IsNullOrWhiteSpace(txtlabel.Text))
                    {
                        if (DbManager.Save(GetSupplier()))
                        {
                            MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                            Reset();
                        }
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!CanAccess(GetHome.DataContextt.ConnectedUser))
            {
                IsEnabled = false;
                MessageBox.Show(Properties.Resources.CanAccess, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (SupplierId != 0)
                {
                    EasyManagerLibrary.Supplier supplier = new EasyManagerLibrary.Supplier();
                    supplier = GetSupplier(SupplierId);
                    txtcontact.Text = supplier.Contact;
                    txtlabel.Text = supplier.Nom;
                    txtmail.Text = supplier.Email;
                }
            }
            
        }

        private void Reset()
        {
            txtcontact.Text = string.Empty;
            txtlabel.Text = string.Empty;
            txtmail.Text = string.Empty;
        }

        private bool IsSupplierNameExist(string str)
        {
            var count = (DbManager.GetByColumnName<EasyManagerLibrary.Supplier>("Nom", str)).Count();
            return count > 0;
        }

        private Module SupplierModule()
        {
            return DbManager.GetByColumnName<Module>("Libelle", Properties.Resources.ModuleFournisseur)[0];
        }

        public Module GetModule { get => SupplierModule(); set { } }

        public bool CanAccess(Utilisateur utilisateur)
        {
            //UserRole
            var userrole = DbManager.GetByColumnName<Role>("Libelle", utilisateur.RoleLibelle)[0];
            //Role module
            var rolemodule = DbManager.GetByColumnName<RoleModule>("RoleId", userrole.Id.ToString());

            List<int> usermoduleid = new List<int>();
            foreach (var item in rolemodule)
                usermoduleid.Add(item.ModuleId);

            return usermoduleid.Contains(GetModule.Id);
        }
    }
}
