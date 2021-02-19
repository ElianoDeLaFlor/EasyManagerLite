using EasyManager.MenuItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasyManager.Commands
{
    public class OperationCaisseCancelCommand : ICommand
    {
        public OperationCaisseViewModel ViewModel { get; set; }
        public OperationCaisseCancelCommand(OperationCaisseViewModel viewModel)
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
            Empty();
        }

        private void Empty()
        {
            ViewModel.SelectedOperation = 0;
            ViewModel.Montant = "";
        }
    }
}
