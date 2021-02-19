using EasyManager.MenuItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EasyManagerDb;
using ClassL = EasyManagerLibrary;
using System.Windows;

namespace EasyManager.Commands
{
    public class OperationCommand : ICommand
    {
        public OperationViewModel ViewModel { get; set; }
        public OperationCancelCommand CancelCommand { get; set; }


        public OperationCommand(OperationViewModel viewModel)
        {
            ViewModel = viewModel;
            CancelCommand = new OperationCancelCommand(viewModel);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        

        public bool CanExecute(object parameter)
        {
            if (parameter != null)
            {
                var p = parameter as string;
                if (string.IsNullOrWhiteSpace(p))
                    return false;
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            string libelle = (parameter as string).ToLower();

            if (IsOperationExist(libelle))
            {
                MessageBox.Show(Properties.Resources.OperationExist, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(Save(libelle))
            {
                MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                CancelCommand.CanExecute(libelle);
                CancelCommand.Execute(libelle);
            }
            else
            {
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool Save(string libelle)
        {
            ClassL.Operation operation = new ClassL.Operation();
            operation.Libelle = libelle;

            operation.TypeOperation=ViewModel.OperationTypeIndex == 0? ClassL.TypeOperation.Sortie:ClassL.TypeOperation.Entree;

            return DbManager.Save(operation);
            
        }

        private bool IsOperationExist(string libelle)
        {
            var lst = DbManager.GetByColumnName<ClassL.Operation>("Libelle", libelle);
            return lst.Count > 0;
        }
    }
}
