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
    /// Interaction logic for SupplierList.xaml
    /// </summary>
    public partial class SupplierList : Window
    {
        public List<SupplierTile> GetSupplierTiles { get; set; } = new List<SupplierTile>();
        public Home GetHome { get; set; }
        public SupplierList()
        {
            InitializeComponent();
        }

        public SupplierList(Home home)
        {
            InitializeComponent();
            GetHome = home;
        }
        private List<SupplierTile> SupplierTiles()
        {
            //Liste des tuiles
            List<SupplierTile> suppliertiles = new List<SupplierTile>();
            //Liste des suppliers
            List<EasyManagerLibrary.Supplier> SupList = DbManager.GetAll<EasyManagerLibrary.Supplier>();
            //Parcours la liste des suppliers
            foreach (var item in SupList)
            {
                SupplierTile SupplierTile = new SupplierTile(this,GetHome);
                SupplierTile.Supplier=item;
                suppliertiles.Add(SupplierTile);
            }
            return suppliertiles;
        }

        public void Reload()
        {
            SupplierListe.Children.Clear();
            GetSupplierTiles = SupplierTiles();
            if (GetSupplierTiles.Count == 0)
            {
                //
            }
            else
            {
                foreach (var item in GetSupplierTiles)
                {
                    SupplierListe.Children.Add(item);
                }
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
                SetScrollHeight();
                Reload();
            }
            
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetScrollHeight();
        }

        private void SetScrollHeight()
        {
            var ah = this.ActualHeight;
            lstScroll.MaxHeight = ah -( ah /7);
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

        private void SetData(List<SupplierTile> lst)
        {
            SupplierListe.Children.Clear();
            foreach (var item in lst)
            {
                SupplierListe.Children.Add(item);
            }
        }

        private List<SupplierTile> Search(string critere)
        {
            var rslt = from disc in GetSupplierTiles where disc.Supplier.Nom.Contains(critere) || disc.Supplier.Contact.Contains(critere) || disc.Supplier.Email.Contains(critere) || disc.Supplier.Id.ToString().Contains(critere) select disc;
            return rslt.ToList();
        }

        private void Research()
        {
            if (string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                ResetList();
                return;
            }
            var rslt = Search(TxtSearch.Text);
            if (rslt == null)
                ResetList();
            if (rslt.Count > 0)
            {
                GetSupplierTiles.Clear();
                GetSupplierTiles = rslt;
                //rslt.Clear();
                SetData(GetSupplierTiles);
            }
            else
            {
                //ResetList();
            }
        }

        private void ResetList()
        {
            GetSupplierTiles = SupplierTiles();
            SetData(GetSupplierTiles);
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            Research();
        }
    }
}
