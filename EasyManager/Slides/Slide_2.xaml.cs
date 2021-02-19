using EasyManager.ClientClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace EasyManager.Slides
{
    /// <summary>
    /// Interaction logic for Slide_2.xaml
    /// </summary>
    public partial class Slide_2 : UserControl
    {
        public string _couleur;
        string _contenu { get; set; }
        public string Couleur { get { return _couleur; } set { _couleur = value; }}

        public string Contenu { get { return _contenu; } set { _contenu = value; } }
        public Slide_2()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
