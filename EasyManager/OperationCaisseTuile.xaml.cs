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
    /// Interaction logic for OperationCaisseTuile.xaml
    /// </summary>
    public partial class OperationCaisseTuile : UserControl
    {
        public EasyManagerLibrary.OperationCaisse GetOperationCaisse { get; set; }
        public string GetOperation { get => OperationById(GetOperationCaisse.OperationId).Libelle; }
        public string GetUtilisateur { get =>$" {UserById(GetOperationCaisse.UtilisateurId).Prenom} {UserById(GetOperationCaisse.UtilisateurId).Nom}"; }
        public OperationCaisseListeUC GetListeUC { get; set; }
        public OperationCaisseTuile()
        {
            InitializeComponent();
            DataContext = this;
        }
        public OperationCaisseTuile(OperationCaisseListeUC operationListUC)
        {
            InitializeComponent();
            GetListeUC = operationListUC;
            DataContext = this;

        }

        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            //get operation de caisse par operation type
            var prodbycat = DbManager.GetById<EasyManagerLibrary.OperationCaisse>(GetOperationCaisse.Id);
            //check if is more than one month
            var rslt = InfoChecker.DateDiff(prodbycat.Date, DateTime.UtcNow);
            if (rslt>=30)
            {
                //Delete
                if (!DbManager.Delete<EasyManagerLibrary.Operation>(GetOperationCaisse.Id))
                    MessageBox.Show(Properties.Resources.DeleteError, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    GetListeUC.Load();
                }
            }
            else
            {
                //Can't be delete
                MessageBox.Show(Properties.Resources.DeleteTime, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private EasyManagerLibrary.Operation OperationById(int id)
        {
            return DbManager.GetById<EasyManagerLibrary.Operation>(id);
        }
        private EasyManagerLibrary.Utilisateur UserById(int id)
        {
            return DbManager.GetById<EasyManagerLibrary.Utilisateur>(id);
        }

        private void btnedit_Click(object sender, RoutedEventArgs e)
        {
            OperationCaisseEdit opcaiss = new OperationCaisseEdit(GetOperationCaisse);
            opcaiss.ShowDialog();
            GetListeUC.Load();
        }
    }
}
