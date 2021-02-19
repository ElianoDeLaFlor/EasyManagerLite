using EasyManager.ClientClass;
using EasyManagerLibrary;
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

namespace EasyManager.UserControls
{
    /// <summary>
    /// Interaction logic for Toggle.xaml
    /// </summary>
    public partial class ToggleControl : UserControl
    {

        private Module _module;
        private bool _state;
        public List<int> CheckedList { get; set; }

        public bool State
        {
            get { return _state; }
            set { _state = value; ChkBtn.IsChecked = value; }
        }

        public void SetCheckedList(ref List<int> lst)
        {
            CheckedList = lst;
        }

        public Module Module
        {
            get { return _module; }
            set { _module = value; ChkText.Text = value.Libelle; }
        }


        public ToggleControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ChkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (State == false)
            {
                State = true;
                CheckedList.Add(Module.Id);
            }
            else
            {
                State = false;
                CheckedList.Remove(Module.Id);
            }
        }
    }
}
