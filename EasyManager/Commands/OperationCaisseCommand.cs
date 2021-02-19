using EasyManager.MenuItems;
using EasyManagerDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ClassL = EasyManagerLibrary;

namespace EasyManager.Commands
{
    public class OperationCaisseCommand : ICommand
    {
        public OperationCaisseViewModel ViewModel { get; set; }

        public OperationCaisseCommand(OperationCaisseViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            decimal d;
            if (parameter != null)
            {
                if(decimal.TryParse(parameter as string,out d))
                {
                    return d > 0;
                }
                return false;              
            }
            return false;
        }

        public void Execute(object parameter)
        {
            string libelle = (parameter as string).ToLower();
            decimal montant = decimal.Parse(libelle);

            if (Save(montant))
            {
                MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool Save(decimal montant)
        {
            ClassL.OperationCaisse operationC = new ClassL.OperationCaisse();
            operationC.Montant = montant;
            operationC.OperationId = ViewModel.OperationItem(ViewModel.SelectedOperation).Id;
            operationC.Date = DateTime.UtcNow;
            operationC.UtilisateurId = EasyManagerLibrary.InfoChecker.ConnectedUser.Id;
            

            return DbManager.Save(operationC);

        }
    }
}
