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
    /// Interaction logic for VenteResumeUC.xaml
    /// </summary>
    public partial class VenteResumeUC : UserControl
    {
        public static readonly DependencyProperty GetClasseProperty = DependencyProperty.Register(nameof(GetVente), typeof(ClassL.Vente), typeof(VenteResumeUC), new PropertyMetadata(ClasseChanged));

        public static readonly DependencyProperty GetCreditProperty = DependencyProperty.Register(nameof(GetVenteCredit), typeof(ClassL.VenteCredit), typeof(VenteResumeUC), new PropertyMetadata(ClasseChanged));
        public VenteResumeUC()
        {
            InitializeComponent();
        }

        public VenteResumeUC(ClassL.Vente vente)
        {
            InitializeComponent();
            DataContext = vente;
        }

        public VenteResumeUC(ClassL.VenteCredit ventecredit)
        {
            InitializeComponent();
            DataContext = ventecredit;
        }



        public ClassL.Vente GetVente
        {

            get => (ClassL.Vente)GetValue(GetClasseProperty);
            set
            {
                SetValue(GetClasseProperty, value);
            }
        }

        public ClassL.VenteCredit GetVenteCredit
        {

            get => (ClassL.VenteCredit)GetValue(GetClasseProperty);
            set
            {
                SetValue(GetClasseProperty, value);
            }
        }

        private static void ClasseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        protected virtual void GetClasseDependencyPropertyChanged(ClassL.Vente newvalue)
        {

        }
    }
}
