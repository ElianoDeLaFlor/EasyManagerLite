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
    /// Interaction logic for FactureDetail.xaml
    /// </summary>
    public partial class FactureDetail : UserControl
    {
        public static readonly DependencyProperty GetClasseProperty = DependencyProperty.Register(nameof(GetProduct), typeof(ClassL.Produit), typeof(FactureDetail), new PropertyMetadata(ClasseChanged));
        public ClassL.Produit Produit { get; set; }
        public FactureDetail()
        {
            InitializeComponent();
            //DataContext = Produit;
        }

        public ClassL.Produit GetProduct
        {

            get => (ClassL.Produit)GetValue(GetClasseProperty);
            set
            {
                SetValue(GetClasseProperty, value);
            }
        }

        private static void ClasseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (FactureDetail)d;
            ClassL.Produit newval = (ClassL.Produit)e.NewValue;
            target.GetClasseDependencyPropertyChanged(newval);
        }

        protected virtual void GetClasseDependencyPropertyChanged(ClassL.Produit newvalue)
        {
            
        }
    }
}
