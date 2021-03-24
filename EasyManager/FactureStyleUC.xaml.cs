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
    /// Interaction logic for FactureStyleUC.xaml
    /// </summary>
    public partial class FactureStyleUC : UserControl
    {
        FactureStyleViewModel factureStyleViewModel = new FactureStyleViewModel();
        public FactureStyleUC()
        {
            InitializeComponent();
            DataContext = factureStyleViewModel;
        }
        public string Title
        {
            get => factureStyleViewModel.Title;
            set
            {
                factureStyleViewModel.Title = value;
            }
        }

        public string Image
        {
            get => factureStyleViewModel.Image;
            set
            {
                factureStyleViewModel.Image = value;
            }
        }


    }
}
