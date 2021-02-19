using EasyManager.MenuItems;
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
    /// Interaction logic for OperationUC.xaml
    /// </summary>
    public partial class OperationUC : UserControl
    {
        public GestionDeCaisse GetCaisseHome { get; set; }
        public OperationUC()
        {
            InitializeComponent();
        }

        public OperationUC(GestionDeCaisse CaisseHome)
        {
            InitializeComponent();
            GetCaisseHome = CaisseHome;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (GetCaisseHome != null)
            {
                GetCaisseHome.Title = Properties.Resources.AddOperation;
                GetCaisseHome.MainTitle.Text = Properties.Resources.AddOperation;
            }
        }
    }
}
