using EasyManagerDb;
using EasyManagerLibrary;
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
    /// Interaction logic for OperationCaisseEdit.xaml
    /// </summary>
    public partial class OperationCaisseEdit : Window
    {
        public OperationCaisse GetOperationCaisse { get; set; }
        private List<ClassL.Operation> ListeOperation;
        private List<String> ListeOperations;
        public OperationCaisseEdit()
        {
            InitializeComponent();
        }

        public OperationCaisseEdit(OperationCaisse caisse)
        {
            InitializeComponent();
            GetOperationCaisse = caisse;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtmontant.Text))
            {
                if (Converter.IsNumeric(txtmontant.Text))
                {
                    var op = new ClassL.OperationCaisse();
                    op.Id = GetOperationCaisse.Id;
                    op.Montant = decimal.Parse(txtmontant.Text);
                    op.Date = DateTime.UtcNow;
                    op.OperationId = (DbManager.GetByColumnName<ClassL.Operation>("Libelle", (string)Cbtype.SelectedValue)).First().Id;
                    op.UtilisateurId = InfoChecker.ConnectedUser==null?1:InfoChecker.ConnectedUser.Id;

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
                    MessageBox.Show(Properties.Resources.AmountError, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
            else
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool Update(ClassL.OperationCaisse operation)
        {
            return DbManager.UpDate(operation, operation.Id);
        }

        private void SetInfo(ClassL.OperationCaisse operationcaisse)
        {
            txtmontant.Text = operationcaisse.Montant.ToString();
            var operation = OperationById(operationcaisse.OperationId);
            Cbtype.SelectedValue = operation.Libelle;

        }

        public void OperationList()
        {
            ListeOperation = new List<ClassL.Operation>();
            ListeOperations = new List<string>();
            var lst = DbManager.GetAll<ClassL.Operation>();
            ListeOperation = lst;
            foreach (var item in lst)
            {
                ListeOperations.Add(item.Libelle);
            }
            
        }

        private ClassL.Operation OperationById(int id)
        {
            return DbManager.GetById<ClassL.Operation>(id);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OperationList();
            Cbtype.ItemsSource = ListeOperations;
            SetInfo(GetOperationCaisse);
        }

        
    }
}
