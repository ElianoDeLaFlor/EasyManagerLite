using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EasyManagerLibrary;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace EasyManager.Charts
{
    /// <summary>
    /// Interaction logic for Doughnutchart.xaml
    /// </summary>
    public partial class Doughnutchart : UserControl, INotifyPropertyChanged
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
        private int _raduis=40;
        public Func<ChartPoint, string> PointLabel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                if (_title == value)
                    return;
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public Doughnutchart()
        {
            InitializeComponent();
            PointLabel = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
            //testData();
            //InitSerieCollection(Datas);
            DataContext = this;
        }

        public int Raduis
        {
            get=>_raduis;
            set
            {
                if (_raduis == value)
                    return;
                _raduis = value;
                OnPropertyChanged("Raduis");
            }
        }

        public PieSeries Init(ChartDataPie chartData)
        {
            PieSeries series = new PieSeries();
            series.Title = chartData.Titre;
            series.Values = new ChartValues<ObservableValue> { new ObservableValue(chartData.Valeur) };
            series.DataLabels = chartData.LblDate;
            series.LabelPoint = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
            return series;
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public List<ChartDataPie> Datas
        {
            get { return _Datas; }
            set
            {
                _Datas = value;
                InitSerieCollection(Datas);
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

        public void testData()
        {
            Datas = new List<ChartDataPie>();
            for (int i = 0; i < 2; i++)
            {
                var d = new ChartDataPie();
                if(i==0)
                    d.Titre = Properties.Resources.SellQuantity;
                else
                    d.Titre = Properties.Resources.LeftQuantity;
                d.Valeur = i + 3;
                d.LblDate = true;
                Datas.Add(d);
            }
        }

        private void UpdateAllOnClick(object sender, RoutedEventArgs e)
        {
            var r = new Random();

            foreach (var series in SeriesCollection)
            {
                foreach (var observable in series.Values.Cast<ObservableValue>())
                {
                    observable.Value = r.Next(0, 10);
                }
            }
        }

        private void AddSeriesOnClick(object sender, RoutedEventArgs e)
        {
            var r = new Random();
            var c = SeriesCollection.Count > 0 ? SeriesCollection[0].Values.Count : 5;

            var vals = new ChartValues<ObservableValue>();

            for (var i = 0; i < c; i++)
            {
                vals.Add(new ObservableValue(r.Next(0, 10)));
            }

            SeriesCollection.Add(new PieSeries
            {
                Values = vals
            });
        }

        private void RemoveSeriesOnClick(object sender, RoutedEventArgs e)
        {
            if (SeriesCollection.Count > 0)
                SeriesCollection.RemoveAt(0);
        }

        private void AddValueOnClick(object sender, RoutedEventArgs e)
        {
            var r = new Random();
            foreach (var series in SeriesCollection)
            {
                series.Values.Add(new ObservableValue(r.Next(0, 10)));
            }
        }

        private void RemoveValueOnClick(object sender, RoutedEventArgs e)
        {
            foreach (var series in SeriesCollection)
            {
                if (series.Values.Count > 0)
                    series.Values.RemoveAt(0);
            }
        }

        private void RestartOnClick(object sender, RoutedEventArgs e)
        {
            Chart.Update(true, true);
        }
    }
}
