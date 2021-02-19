using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyManager.MenuItems
{
    public class DatePickerViewModel: INotifyPropertyChanged
    {
        private DateTime _date;
        private DateTime _dateF;
        private DateTime _time;
        private DateTime _timeF;
        private string _validatingTime;
        private DateTime? _futureValidatingDate;

        public DatePickerViewModel()
        {
            Date = DateTime.Now;
            Time = DateTime.Now;

            DateF = DateTime.Now;
            TimeF = DateTime.Now;
        }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }
        public DateTime DateF
        {
            get { return _dateF; }
            set
            {
                _dateF = value;
                OnPropertyChanged();
            }
        }

        public DateTime Time
        {
            get { return _time; }
            set
            {
                _time = value;
                OnPropertyChanged();
            }
        }

        public DateTime TimeF
        {
            get { return _timeF; }
            set
            {
                _timeF = value;
                OnPropertyChanged();
            }
        }

        public string ValidatingTime
        {
            get { return _validatingTime; }
            set
            {
                _validatingTime = value;
                OnPropertyChanged();
            }
        }

        public DateTime? FutureValidatingDate
        {
            get { return _futureValidatingDate; }
            set
            {
                _futureValidatingDate = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
