using EasyManager.Commands;
using EasyManagerDb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ClassL = EasyManagerLibrary;

namespace EasyManager.MenuItems
{
    public class OperationCaisseViewModel:INotifyPropertyChanged,INotifyDataErrorInfo
    {

        private string _montantt;
        private int _selectedoperation;
        public List<ClassL.Operation> ListeOperation;
        private List<string> ListeOperations;
        private List<string> _operations = new List<string>();

        private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();

        public string Montant
        {
            get => _montantt;
            set
            {
                if (_montantt == value)
                    return;
                _montantt = value;
                ValidateAmount();
                OnPropertyChanged();
            }
        }
        public List<string> Operations
        {
            get
            {
                //return OperationList();
                return _operations;
            }
            set
            {
                if (_operations == value)
                    return;
                _operations = value;
                OnPropertyChanged();
            }
        }

        public bool HasErrors => _errorsByPropertyName.Any();

        public int SelectedOperation
        {
            get=>_selectedoperation;
            set
            {
                if (_selectedoperation == value)
                    return;
                _selectedoperation = value;
                OnPropertyChanged();
            }
        }
        public OperationCaisseCommand SaveCommand { get; set; }
        public OperationCaisseCancelCommand CancelCommand { get; set; }

        public OperationCaisseViewModel()
        {
            SaveCommand = new OperationCaisseCommand(this);
            CancelCommand = new OperationCaisseCancelCommand(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsByPropertyName.ContainsKey(propertyName) ? _errorsByPropertyName[propertyName] : null;
        }
        private void OnErrorChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void AddError(string propertyName,string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
                _errorsByPropertyName[propertyName] = new List<string>();

            if(!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorChanged(propertyName);
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorChanged(propertyName);
            }
        }

        private void ValidateAmount()
        {
            ClearErrors(nameof(Montant));
            if (!double.TryParse(Montant, out _))
                AddError(nameof(Montant), Properties.Resources.AmountError);
        }

        public List<string> OperationList()
        {
            ListeOperation = new List<ClassL.Operation>();
            ListeOperations = new List<string>();
            var lst = DbManager.GetAll<ClassL.Operation>();
            ListeOperation = lst;
            foreach (var item in lst)
            {
                ListeOperations.Add(item.Libelle);
            }
            return ListeOperations;
        }
        public ClassL.Operation OperationItem(int index)
        {
            return ListeOperation[index];
        }

        

    }
}
