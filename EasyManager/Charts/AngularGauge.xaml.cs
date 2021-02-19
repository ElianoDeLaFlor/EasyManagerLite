using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace EasyManager.Charts
{
    /// <summary>
    /// Interaction logic for AngularGauge.xaml
    /// </summary>
    public partial class AngularGauge : UserControl, INotifyPropertyChanged
    {
        private double _value;
        public AngularGauge()
        {
            InitializeComponent();
            Value = 160;

            DataContext = this;
        }

        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        private void ChangeValueOnClick(object sender, RoutedEventArgs e)
        {
            Value = new Random().Next(50, 250);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
