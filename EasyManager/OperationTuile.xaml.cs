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
    /// Interaction logic for OperationTuile.xaml
    /// </summary>
    public partial class OperationTuile : UserControl
    {
        public EasyManagerLibrary.Operation GetOperation { get; set; }
        public OperationListUC GetListeUC { get; set; }
        public OperationTuile()
        {
            InitializeComponent();
            DataContext = this;
        }
        public OperationTuile(OperationListUC operationListUC)
        {
            InitializeComponent();
            GetListeUC = operationListUC;
            DataContext = this;

        }

        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            //get operation de caisse par operation type
            var prodbycat = DbManager.GetByColumnName<EasyManagerLibrary.OperationCaisse>("OperationId", GetOperation.Id.ToString());
            if (prodbycat.Count == 0)
            {
                //Delete
                if (!DbManager.Delete<EasyManagerLibrary.Operation>(GetOperation.Id))
                    MessageBox.Show(Properties.Resources.DeleteError, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    GetListeUC.Load();
                }
            }
            else
            {
                //Can't be delete
                MessageBox.Show(Properties.Resources.OpCannotBeDeleted, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnedit_Click(object sender, RoutedEventArgs e)
        {
            OperationEdit operat = new OperationEdit(GetOperation);
            operat.ShowDialog();
            GetListeUC.Load();
        }
    }
}
