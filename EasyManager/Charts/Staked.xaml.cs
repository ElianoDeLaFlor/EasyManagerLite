using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using EasyManagerLibrary;
using LiveCharts;
using LiveCharts.Wpf;

namespace EasyManager.Charts
{
    /// <summary>
    /// Interaction logic for Staked.xaml
    /// </summary>
    public partial class Staked : UserControl, INotifyPropertyChanged
    {
        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; } = new List<string>();
        private List<ChartDataPie> _Datas = new List<ChartDataPie>();
        private List<List<ChartDataPie>> _Datass = new List<List<ChartDataPie>>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public Func<double, string> Formatter { get; set; }

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

        public List<List<ChartDataPie>> Datass
        {
            get { return _Datass; }
            set
            {
                _Datass = value;
                InitSerieCollection(Datass);
                OnPropertyChanged("Datass");
            }
        }

        public void InitSerieCollection(List<ChartDataPie> data)
        {
            SeriesCollection = new SeriesCollection();
            foreach (var item in data)
            {
                SeriesCollection.Add(Init(item));

            }
        }

        public void InitSerieCollection(List<List<ChartDataPie>> data)
        {
            SeriesCollection = new SeriesCollection();
            SeriesCollection.AddRange(Init(data));
        }

        public void testData()
        {
            Datas = new List<ChartDataPie>();
            for (int i = 130; i < 137; i++)
            {
                var d = new ChartDataPie();
                d.Titre = $"Titre{i}";
                d.Valeurs.Add(i + 3);
                d.Valeurs.Add(i + 4);
                d.Valeurs.Add(i + 5);
                d.Valeurs.Add(i + 6);
                d.Valeurs.Add(i + 7);
                d.Valeurs.Add(i + 8);
                d.Valeurs.Add(i + 9);
                d.LblDate = true;
                Datas.Add(d);
            }
        }

        public StackedColumnSeries Init(ChartDataPie chartData)
        {
            StackedColumnSeries series = new StackedColumnSeries();
            //series.Title = chartData.Titre;
            Labels.Add(chartData.Titre);
            ChartValues<double> lstvalue = new ChartValues<double>();
            foreach (var item in chartData.Valeurs)
            {
                lstvalue.Add(item);
            }
            series.Values = lstvalue;
            series.DataLabels = chartData.LblDate;
            series.StackMode = StackMode.Percentage;

            //series.LabelPoint = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
            return series;
        }

        private StackedColumnSeries[] CreateStackedColumnSeriesCollection(int cnt)
        {
            StackedColumnSeries[] stackedColumnSeries = new StackedColumnSeries[cnt];
            return stackedColumnSeries;
        }

        public List<StackedColumnSeries> Init(List<List<ChartDataPie>> dataPies)
        {
            //size of the stackedcolumnserie value array
            int cnt = dataPies.Count;
            StackedColumnSeries series;
            List<StackedColumnSeries> lstseries=new List<StackedColumnSeries>();
            int pos = 1;
            foreach (var item in dataPies)
            {
                if (pos == 1)
                {
                    foreach (var chart in item)
                    {
                        series = new StackedColumnSeries();
                        series.Values=(new ChartValues<double> {chart.Valeur});
                        series.StackMode = StackMode.Percentage;
                        lstseries.Add(series);
                    }
                }
                else
                {

                }

            }
            return lstseries;

        }

        public Staked()
        {
            InitializeComponent();
            testData();
            InitSerieCollection(Datass);
            #region initfunc
            //SeriesCollection = new SeriesCollection
            //{
            //    new StackedColumnSeries
            //    {
            //        Values = new ChartValues<double> {4,3,0},
            //        StackMode = StackMode.Values, // this is not necessary, values is the default stack mode
            //        DataLabels = true
            //    },
            //    new StackedColumnSeries
            //    {
            //        Values = new ChartValues<double> {5,4,0},
            //        StackMode = StackMode.Values,
            //        DataLabels = true
            //    },
            //    new StackedColumnSeries
            //    {
            //        Values = new ChartValues<double> {6,7,0},
            //        StackMode = StackMode.Values, // this is not necessary, values is the default stack mode
            //        DataLabels = true
            //    },
            //    new StackedColumnSeries
            //    {
            //        Values = new ChartValues<double> {8,8,9},
            //        StackMode = StackMode.Values,
            //        DataLabels = true
            //    }

            //};

            ////adding series updates and animates the chart
            ////SeriesCollection.Add(new StackedColumnSeries
            ////{
            ////    Values = new ChartValues<double> { 6, 2, 7 },
            ////    StackMode = StackMode.Values
            ////});

            ////adding values also updates and animates
            ////SeriesCollection[2].Values.Add(4d);

            //Labels = new List<string> { "Chrome", "Mozilla", "Opera", "IE" };
            //Formatter = value => value + " Mill";
            #endregion

            DataContext = this;
        }

    }
}
