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
using EasyManagerLibrary;

namespace EasyManager.UserControls
{
    /// <summary>
    /// Interaction logic for ToogleCategorie.xaml
    /// </summary>
    public partial class ToogleCategorie : UserControl
    {
        private EasyManagerLibrary.Categorie _categorie;
        private bool _state;
        public List<int> CheckedList { get; set; } = new List<int>();
        public List<int> CheckedList_;

        public ToogleCategorie()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ToogleCategorie(ref List<int> chklst)
        {
            InitializeComponent();
            DataContext = this;
             chklst=CheckedList;
           
        }


        public bool State
        {
            get { return _state; }
            set { _state = value; ChkBtn.IsChecked = value; }
        }

        public void SetCheckedList(ref List<int> lst)
        {
            CheckedList= lst;
        }

        public EasyManagerLibrary.Categorie Categorie
        {
            get { return _categorie; }
            set { _categorie = value; ChkText.Text = value.Libelle; }
        }

        private void ChkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (State == false)
            {
                State = true;
                CheckedList.Add(Categorie.Id);
            }
            else
            {
                State = false;
                CheckedList.Remove(Categorie.Id);
            }
        }
    }
}
