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
    /// Interaction logic for AnnulationVente.xaml
    /// </summary>
    public partial class AnnulationVente : Window,INotifyPropertyChanged
    {
        
        private int _index = 0;
        private List<ClassL.ProduitVendu> _productlist;
        public List<ClassL.ProduitVendu> ProductList 
        { 
            get=>_productlist;
            set
            {
                if (_productlist == value)
                    return;
                _productlist = value;
                OnPropertyChanged();
            }
        }
        public ClassL.Vente Vente { get; set; }
        public List<ClassL.ProduitVendu> Produits { get; set; }
        public int VenteId { get; set; }
        public List<string> VenteIdLst { get; set; } = new List<string>();
        public int Index 
        { 
            get=>_index;
            set
            {
                if (_index == value)
                    return;
                _index = value;
                var rst = GetProduitVendu(_index);
                if (rst != null)
                {
                    ProductList = rst.Item1;
                    Vente = rst.Item2;
                    SetResumeUC(rst.Item2);
                }
                OnPropertyChanged();
            } 
        }
       
        public AnnulationVente()
        {
            InitializeComponent();
            DataContext = this;
            ManageProgress(false);
            VenteListe();
        }

        private void Reset()
        {
            VenteListe();
            if(VenteIdLst.Count>2)
                Index = 1;
        }

        private void ManageProgress(bool b)
        {
            if (b)
                progress.Visibility = Visibility.Visible;
            else
                progress.Visibility = Visibility.Hidden;
        }

        private void SetResumeUC(ClassL.Vente vente)
        {
            ResumeStack.Children.Clear();
            VenteResumeUC vr = new VenteResumeUC(vente);
            ResumeStack.Children.Add(vr);
            
        }

        private void VenteListe()
        {
            string query = "SELECT * FROM Vente WHERE Canceled=False";
            var lst = DbManager.CustumQuery<ClassL.Vente>(query);
            List<string> venteid = new List<string>();
            venteid.Add(Properties.Resources.MakeSelection);
            foreach (var item in lst)
            {
                venteid.Add(ClassL.InfoChecker.FormatIdent(item.Id));
            }
            VenteIdLst = venteid;
            CbventeList.ItemsSource= venteid;
        }
        
        private Tuple<List<ClassL.ProduitVendu>,ClassL.Vente> GetProduitVendu(int id)
        {
            if (id <= 0)
                return null;
            int venteid = int.Parse(VenteIdLst[id]);
            var Vente = DbManager.GetById<ClassL.Vente>(venteid);
            var ProductList = DbManager.GetByColumnName<ClassL.ProduitVendu>("VenteId", venteid.ToString());

            return new Tuple<List<ClassL.ProduitVendu>, ClassL.Vente>(ProductList, Vente);
            
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
                if (Vente.CommandeId.HasValue && Vente.CommandeId.Value>0)
                {
                    if (await CommandeCancelOperationAsync())
                    {
                        MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                        ManageProgress(false);
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                        ManageProgress(false);

                    }
                }
                else
                {
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
                }
                

                Reset();
            }
        }

        private bool CancelOperation()
        {
            try
            {
                //repositionner la quantité vendue
                foreach (var item in ProductList)
                {
                    UndoSell(item);
                }
                //marquer la vente comme annuler
                CancelSell(Vente);
                return true;
            }
            catch
            {

                return false;
            }

        }

        private bool CommandeCancelOperation()
        {
            try
            {
                //repositionner la quantité vendue
                foreach (var item in ProductList)
                {
                    UndoCommandPaied(item,Vente);
                }
                //Réinitialisé les infos la commande
                CancelCommandSell(Vente);
                return ResetCommandeInfo(Vente.CommandeId.GetValueOrDefault());
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

        private async Task<bool> CommandeCancelOperationAsync()
        {
            return await Task.Run(() => CommandeCancelOperation());
        }

        private bool UndoSell(ClassL.ProduitVendu vendu)
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

        private bool CancelSell(ClassL.Vente vente)
        {
            string query = $"UPDATE Vente SET Canceled=True,CanceledDate='{ClassL.InfoChecker.AjustDateDb(DateTime.UtcNow)}' WHERE Id={vente.Id}";
            return DbManager.UpdateCustumQuery(query);
        }

        private bool CancelCommandSell(ClassL.Vente vente)
        {
            string query = $"UPDATE Vente SET Canceled=True,CanceledDate='{ClassL.InfoChecker.AjustDateDb(DateTime.UtcNow)}' WHERE CommandeId={vente.CommandeId}";
            return DbManager.UpdateCustumQuery(query);
        }

        private void ShowCommandCancelationDialog()
        {
            AnnulationCommande annulationCommande = new AnnulationCommande();
            annulationCommande.ShowDialog();
        }
        
        private void btnorder_Click(object sender, RoutedEventArgs e)
        {
            ShowCommandCancelationDialog();
        }

        private void UndoCommandPaied(ClassL.ProduitVendu vendu,ClassL.Vente vente)
        {
            //mettre à jour la quantité restante du produit crédit vendu
            double quantitevendu = vendu.Quantite;
            int cmdid = vente.CommandeId.GetValueOrDefault();
            //produitcredit info
            string query = $"SELECT * FROM ProduitCredit WHERE ProduitId={vendu.ProduitId} AND CommandeId={cmdid}";
            var prodcredits = DbManager.CustumQuery<ClassL.ProduitCredit>(query);
            if (prodcredits.Count > 1)
            {
                // more than one match
                // Canceled the whole command instead
                CancelCommandSell(vente);
                ResetCommandeInfo(cmdid);
            }
            else
            {
                // only one match => Ok
                var prodcredit = prodcredits.FirstOrDefault();
                string query2 = $"UPDATE ProduitCredit SET QuantiteRestante={prodcredit.QuantiteRestante + quantitevendu} WHERE Id={prodcredit.Id}";
                var rslt = DbManager.UpdateCustumQuery(query2);
            }
        }

        private bool ResetCommandeInfo(int cmdid)
        {
            var cmd = DbManager.GetById<ClassL.VenteCredit>(cmdid);
            string query = $"UPDATE VenteCredit SET MontantRestant={cmd.Montant} WHERE Id={cmdid}";
            return DbManager.UpdateCustumQuery(query);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            scroll.MaxHeight = Math.Abs(ActualHeight - 100);
        }
    }
}
