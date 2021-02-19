using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using EasyManagerLibrary;
using System.ComponentModel;

namespace EasyManager.Charts
{
    /// <summary>
    /// Interaction logic for PieChart.xaml
    /// </summary>
    public partial class PieChart : UserControl,INotifyPropertyChanged
    {
        private SeriesCollection _SeriesViews;
        public Func<ChartPoint, string> PointLabel { get; set; }
        public SeriesCollection SeriesViews 
        { 
            get=>_SeriesViews;
            set
            {
                _SeriesViews = value;
                OnPropertyChanged("SeriesViews");
            } 
        }
        public List<ChartDataPie> _Datas;
        public PieChart()
        {
            InitializeComponent();
            PointLabel = chartPoint =>string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
            //testData();
            //InitSerieCollection(Datas);
            DataContext = this;
        }

        public List<ChartDataPie> Datas
        {
            get { return _Datas; }
            set
            {
                _Datas = value;
                InitSerieCollection(_Datas);
                OnPropertyChanged("Datas");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PieSeries Init(ChartDataPie chartData)
        {            
            PieSeries series = new PieSeries();
            series.Title = chartData.Titre;
            series.Values = new ChartValues<double> { chartData.Valeur };
            series.DataLabels = chartData.LblDate;
            series.LabelPoint= chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
            return series;
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void InitSerieCollection(List<ChartDataPie> data)
        {
            SeriesViews = new SeriesCollection();
            foreach (var item in data)
            {
                SeriesViews.Add(Init(item));
            }
        }

        public void testData()
        {
            Datas = new List<ChartDataPie>();
            for (int i = 130; i < 137; i++)
            {
                var d = new ChartDataPie();
                d.Titre = $"Titre{i}";
                d.Valeur = i + 3;
                d.LblDate = true;
                Datas.Add(d);
            }
        }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            var title = selectedSeries.Title;
            var value = selectedSeries.Values[0];
            ContextMenu context = new ContextMenu();
            context.Items.Add(title);
            context.Items.Add(new System.Windows.Controls.Separator());
            context.Items.Add(value);
            context.IsOpen = true;
            
            selectedSeries.PushOut = 8;
        }

        private void PieChart_DataHover(object sender, ChartPoint chartPoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartPoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartPoint.SeriesView;
            var title = selectedSeries.Title;
            var value = selectedSeries.Values[0];
            ContextMenu context = new ContextMenu();
            context.Items.Add(title);
            context.Items.Add(new System.Windows.Controls.Separator());
            context.Items.Add(value);
            context.IsOpen = true;
            selectedSeries.PushOut = 8;
        }

    }
}
