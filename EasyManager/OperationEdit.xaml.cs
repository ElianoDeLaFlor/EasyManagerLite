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
using System.Windows.Shapes;
using ClassL = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for OperationEdit.xaml
    /// </summary>
    public partial class OperationEdit : Window
    {
        public ClassL.Operation GetOperation { get; set; }
        public OperationEdit()
        {
            InitializeComponent();
        }
        public OperationEdit(ClassL.Operation operation)
        {
            InitializeComponent();
            GetOperation = operation;
        }

        private bool Update(ClassL.Operation operation)
        {
            return DbManager.UpDate(operation, operation.Id);
        }

        private void SetInfo(ClassL.Operation operation)
        {
            txtlabel.Text = operation.Libelle;
            cbtype.SelectedIndex = operation.TypeOperation == ClassL.TypeOperation.Entree ? 1 : 0;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetInfo(GetOperation);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtlabel.Text))
            {
                var op = new ClassL.Operation();
                op.Id = GetOperation.Id;
                op.Libelle = txtlabel.Text;
                op.TypeOperation = cbtype.SelectedIndex == 0 ? ClassL.TypeOperation.Sortie : ClassL.TypeOperation.Entree;

                if (Update(op))
                {
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
