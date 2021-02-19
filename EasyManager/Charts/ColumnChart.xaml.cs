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
using EasyManagerLibrary;
using LiveCharts;
using LiveCharts.Wpf;

namespace EasyManager.Charts
{
    /// <summary>
    /// Interaction logic for ColumnChart.xaml
    /// </summary>
    public partial class ColumnChart : UserControl, INotifyPropertyChanged
    {
        private SeriesCollection _SeriesCollection;

        public SeriesCollection SeriesCollection 
        { 
            get=>_SeriesCollection;
            set
            {
                _SeriesCollection = value;
                OnPropertyChanged("SeriesCollection");
            }
        }
        private List<ChartDataPie> _Datas;
        private string _X_Title;
        private string _Y_Title;
        public Func<double, string> Formatter { get; set; }
        public List<string> Labels { get; set; }
        public ColumnChart()
        {
            InitializeComponent();
            //testData();
            //InitSerieCollection(Datas);
            //Labels = new List<string> { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Height" };
            
            DataContext = this;
        }

        public string XTitle
        {
            get=>_X_Title;
            set
            {
                if (_X_Title == value)
                    return;
                _X_Title = value;
                OnPropertyChanged("XTitle");
            }
        }

        public string YTitle
        {
            get => _Y_Title;
            set
            {
                if (_Y_Title == value)
                    return;
                _Y_Title = value;
                OnPropertyChanged("YTitle");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<ChartDataPie> Datas
        {
            get => _Datas;
            set
            {
                _Datas = value;
                InitSerieCollection(_Datas);
                OnPropertyChanged("Datas");
            }
        }

        public void InitSerieCollection(List<ChartDataPie> data)
        {
            SeriesCollection = new SeriesCollection();
            if (data == null)
                return;
            foreach (var item in data)
            {
                SeriesCollection.Add(Init(item));
            }
        }

        public ColumnSeries Init(ChartDataPie chartData)
        {
            ColumnSeries series = new ColumnSeries();
            series.Title = chartData.Titre;
            series.Values = new ChartValues<double> { chartData.Valeur };
            series.DataLabels = false;// chartData.LblDate;
            //series.LabelPoint = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
            
            return series;
        }

        public void testData()
        {
            Datas = new List<ChartDataPie>();
            for (int i = 130; i < 137; i++)
            {
                var d = new ChartDataPie();
                d.Titre = $"Titre{i}";
                d.Valeur = i + 3;
                d.LblDate = false;
                Datas.Add(d);
            }
        }

        
    }
}
