using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyManager.MenuItems
{
    public class FactureStyleViewModel: INotifyPropertyChanged
    {
        private string _img;
        private string _title;

        public string Title
        {
            get { return _title; }
            set 
            {
                if (_title == value)
                    return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Image
        {
            get { return _img; }
            set 
            {
                if (_img == value)
                    return;
                _img = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
