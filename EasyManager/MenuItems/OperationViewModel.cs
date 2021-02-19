using EasyManager.Commands;
using EasyManagerLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyManager.MenuItems
{
    public class OperationViewModel: INotifyPropertyChanged
    {
        private string _libelle;
        private string _typeoperation;
        private int _operationtypeindex;

        public string Libelle {
            get => _libelle;
            set
            {
                if (_libelle == value)
                    return;
                _libelle = value;
                OnPropertyChanged();
            }
        }

        public string OperationType
        {
            get=>_typeoperation;
            set
            {
                if (_typeoperation == value)
                    return;
                _typeoperation = value;
                OnPropertyChanged();
            }
        }

        public int OperationTypeIndex
        {
            get => _operationtypeindex;
            set
            {
                if (_operationtypeindex == value)
                    return;
                _operationtypeindex = value;
                OnPropertyChanged();

            }
        }

        public OperationCommand SaveCommand { get; set; }
        public OperationCancelCommand CancelCommand { get; set; }

        public OperationViewModel()
        {
            SaveCommand = new OperationCommand(this);
            CancelCommand = new OperationCancelCommand(this);
        }

        public List<string> OperationTypeList { get => Optyp(); }

        public List<string> Optyp()
        {
            List<string> lst = new List<string>() { Properties.Resources.Sortie, Properties.Resources.Entree };
            return lst;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
