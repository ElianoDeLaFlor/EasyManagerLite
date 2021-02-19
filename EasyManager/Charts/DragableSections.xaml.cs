using LiveCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyManager.Charts
{
    /// <summary>
    /// Interaction logic for DragableSections.xaml
    /// </summary>
    public partial class DragableSections : UserControl, INotifyPropertyChanged
    {
        private double _xSection;
        private double _ySection;
        public Func<ChartPoint, string> PointLabel { get; set; }
        public List<string> Labels { get; set; } = new List<string>();
        private ChartValues<double> _xdata { get; set; }= new ChartValues<double> { 7, 2, 48, 2, 7, 4, 9, 4, 2, 8 };
        public DragableSections()
        {
            InitializeComponent();
            XSection = 6;
            YSection = 9;
            DataContext = this;
        }

        public double XSection
        {
            get { return _xSection; }
            set
            {
                _xSection = value;
                OnPropertyChanged("XSection");
            }
        }

        public ChartValues<double> XData
        {
            get { return _xdata; }
            set
            {
                _xdata = value;
                OnPropertyChanged("XData");
            }
        }

        public double YSection
        {
            get { return _ySection; }
            set
            {
                _ySection = value;
                OnPropertyChanged("YSection");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
