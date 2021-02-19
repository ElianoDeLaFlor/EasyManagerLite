using EasyManagerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasyManager.Commands
{
    class DicountValueCommand : ICommand
    {
        VenteUC VenteUC { get; set; }
        VenteCreditUC VenteCreditUC { get; set; }

        public DicountValueCommand(VenteUC venteUC)
        {
            VenteUC = venteUC;
        }
        public DicountValueCommand(VenteCreditUC ventecredit)
        {
            VenteCreditUC = ventecredit;
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
                if (decimal.TryParse(parameter as string, out d))
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
            //decimal montant = decimal.Parse(libelle);

            VenteUC.Montant = libelle;
        }
    }
}
