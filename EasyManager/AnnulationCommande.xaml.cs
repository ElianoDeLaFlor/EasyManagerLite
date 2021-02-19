using EasyManagerDb;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using ClassL = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for AnnulationCommande.xaml
    /// </summary>
    public partial class AnnulationCommande : Window,INotifyPropertyChanged
    {
        private int _index = 0;
        private List<ClassL.ProduitCredit> _productlist;
        public List<ClassL.ProduitCredit> ProductList
        {
            get => _productlist;
            set
            {
                if (_productlist == value)
                    return;
                _productlist = value;
                OnPropertyChanged();
            }
        }
        public ClassL.VenteCredit Vente { get; set; }
        public List<ClassL.ProduitCredit> Produits { get; set; }
        public int CommandeId { get; set; }
        public List<string> CommandeIdLst { get; set; } = new List<string>();
        public int Index
        {
            get => _index;
            set
            {
                if (_index == value)
                    return;
                _index = value;
                var rst = GetProduitCreditVendu(_index);
                if (rst != null)
                {
                    ProductList = rst.Item1;
                    Vente = rst.Item2;
                    SetResumeUC(rst.Item2);
                }
                OnPropertyChanged();
            }
        }
        public AnnulationCommande()
        {
            InitializeComponent();
            DataContext = this;
            ManageProgress(false);
            CommandeListe();
        }

        private void SetResumeUC(ClassL.VenteCredit vente)
        {
            ResumeStack.Children.Clear();
            VenteResumeUC vr = new VenteResumeUC(vente);
            ResumeStack.Children.Add(vr);

        }

        private void Reset()
        {
            CommandeListe();
            if (CommandeIdLst.Count > 2)
                Index = 1;
        }

        private Tuple<List<ClassL.ProduitCredit>, ClassL.VenteCredit> GetProduitCreditVendu(int id)
        {
            if (id <= 0)
                return null;
            int cmdid = int.Parse(CommandeIdLst[id]);
            var commande = DbManager.GetById<ClassL.VenteCredit>(cmdid);
            var ProductList = DbManager.GetByColumnName<ClassL.ProduitCredit>("CommandeId", cmdid.ToString());

            return new Tuple<List<ClassL.ProduitCredit>, ClassL.VenteCredit>(ProductList, commande);

        }

        private void CommandeListe()
        {
            string query = "SELECT * FROM VenteCredit WHERE Canceled=False";
            var lst = DbManager.CustumQuery<ClassL.ProduitCredit>(query);
            List<string> commandid = new List<string>();
            commandid.Add(Properties.Resources.MakeSelection);
            foreach (var item in lst)
            {
                commandid.Add(ClassL.InfoChecker.FormatIdent(item.Id));
            }
            CommandeIdLst = commandid;
            CbventeList.ItemsSource = commandid;
        }

        private void ManageProgress(bool b)
        {
            if (b)
                progress.Visibility = Visibility.Visible;
            else
                progress.Visibility = Visibility.Hidden;
        }

        private bool UndoOrder(ClassL.ProduitCredit vendu)
        {
            try
            {
                //mettre à jour la quantité restante du produit vendu
                double quantitevendu = vendu.Quantite;
                int produitid = vendu.ProduitId;
                //produit info
                var prod = DbManager.GetById<ClassL.Produit>(produitid);
                //repositionner la quantité
                string query = $"UPDATE Produit SET QuantiteRestante={prod.QuantiteRestante + quantitevendu} WHERE Id={produitid}";
                return DbManager.UpdateCustumQuery(query);
            }
            catch
            {
                return false;
            }
        }

        private bool CancelCommande(ClassL.VenteCredit ventecredit)
        {
            bool IsVenteCanceled = true;
            //On vérifie si une partie de la commande a été payé
            if (ventecredit.Montant != ventecredit.MontantRestant)
            {
                //Une partie a été payé
                //On annule la vente
                IsVenteCanceled=CancelSell(ventecredit.Id);
            }
            if (IsVenteCanceled)
            {
                string query = $"UPDATE VenteCredit SET Canceled=True,CanceledDate='{ClassL.InfoChecker.AjustDateDb(DateTime.UtcNow)}' WHERE Id={ventecredit.Id}";
                IsVenteCanceled= DbManager.UpdateCustumQuery(query);
            }

            return IsVenteCanceled;
        }

        private bool CancelOperation()
        {
            try
            {
                //repositionner la quantité vendue
                foreach (var item in ProductList)
                {
                    UndoOrder(item);
                }
                //marquer la vente comme annuler
                CancelCommande(Vente);
                return true;
            }
            catch
            {

                return false;
            }

        }

        private async Task<bool> CancelOperationAsync()
        {
            return await Task.Run(() => CancelOperation());
        }

        private string AjusteIdIn(List<ClassL.VenteCredit> lst)
        {
            int cnt = 1;
            string rslt = "";
            foreach (var item in lst)
            {
                if (cnt == 1)
                    rslt += "(" + item.Id.ToString();
                rslt += "," + item.Id.ToString();

                if (cnt == lst.Count)
                    rslt += "," + item.Id.ToString() + ")";
            }
            return rslt;
        }

        private bool CancelSell(int commandeid)
        {
            string query = $"UPDATE Vente SET Canceled=True,CanceledDate='{ClassL.InfoChecker.AjustDateDb(DateTime.UtcNow)}' WHERE CommandeId={commandeid}";
            return DbManager.UpdateCustumQuery(query);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void btncancel_Click(object sender, RoutedEventArgs e)
        {
            //canceletion process
            if (MessageBox.Show(Properties.Resources.AreYouSureToContinue, Properties.Resources.MainTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ManageProgress(true);
                if (await CancelOperationAsync())
                {
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    ManageProgress(false);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    ManageProgress(false);

                }

                Reset();
            }
        }

        
    }
}
