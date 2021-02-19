using EasyManager.MenuItems;
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
using ClassL = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for OperationCaisseUC.xaml
    /// </summary>
    public partial class OperationCaisseUC : UserControl
    {
        public OperationCaisseViewModel viewModel { get; set; } = new OperationCaisseViewModel();
        private List<ClassL.Operation> ListeOperation;
        public GestionDeCaisse GetCaisseHome { get; set; }

        public OperationCaisseUC()
        {
            InitializeComponent();
            DataContext = viewModel;
            
        }

        public OperationCaisseUC(GestionDeCaisse CaisseHome)
        {
            InitializeComponent();
            GetCaisseHome = CaisseHome;
            DataContext = viewModel;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.Operations = OperationList();
            viewModel.ListeOperation = ListeOperation;

            if (GetCaisseHome != null)
            {
                GetCaisseHome.Title = Properties.Resources.AddCheckOutOperation;
                GetCaisseHome.MainTitle.Text = Properties.Resources.AddCheckOutOperation;
            }
        }

        public List<string> OperationList()
        {
            ListeOperation = new List<ClassL.Operation>();
            List<string>  ListeOperations = new List<string>();
            var lst = DbManager.GetAll<ClassL.Operation>();
            ListeOperation = lst;
            foreach (var item in lst)
            {
                ListeOperations.Add(item.Libelle);
            }
            return ListeOperations;
        }
    }
}
