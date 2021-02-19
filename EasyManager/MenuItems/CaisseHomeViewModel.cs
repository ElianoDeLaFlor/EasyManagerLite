using EasyManager.Charts;
using EasyManagerDb;
using EasyManagerLibrary;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EasyManager.MenuItems
{
    public class CaisseHomeViewModel : INotifyPropertyChanged
    {
        public DatePickerViewModel DatePicker { get; set; } //
        List<ChartDataPie> _chartData { get; set; } = new List<ChartDataPie>();
        public string _DateDebut { get; set; }
        public string _DateFin { get; set; }
        public List<ChartDataPie> TestData { get { return TestDatas(10); } }
        public PieChart PieChart { get; set; }
        public ColumnChart ColumnChart { get; set; }

        public CaisseHomeViewModel()
        {
            DatePicker = new DatePickerViewModel();
        }


        public string DateDebut
        {
            get => _DateDebut;
            set
            {
                if (_DateDebut == value)
                    return;
                _DateDebut = value;
                DatePicker.Date = DateTime.Parse(value);
                ChartDatas = ChartData();
                OnPropertyChanged();
            }
        }

        public string DateFin
        {
            get => _DateFin;
            set
            {
                if (_DateFin == value)
                    return;
                _DateFin = value;
                DatePicker.DateF = DateTime.Parse(value);
                ChartDatas = ChartData();
                OnPropertyChanged();
            }
        }

        public List<ChartDataPie> ChartDatas
        {
            get => _chartData;
            set
            {
                if (_chartData == value)
                    return;
                _chartData = value;
                PieChart.Datas = value;
                ColumnChart.Datas = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<ChartDataPie> ChartData()
        {
            var d1 = DateDebut ?? DateTime.UtcNow.ToShortDateString();
            var d2 = DateFin ?? DateTime.UtcNow.ToShortDateString();
            var dateTime = DateTime.Parse(d1);
            var dateTime2 = DateTime.Parse(d2);
            //liste des opérations de caisses
            var lstopcaisse = ListOperationCaisse(dateTime, dateTime2);
            var lst = OperationCaisse(lstopcaisse);
            var lstpiedata = new List<ChartDataPie>();
            foreach (var item in lst)
            {
                lstpiedata.Add(PieChartDateData(item));
            }
            return lstpiedata;
        }
        private List<OperationCaisse> OperationCaisse(List<OperationCaisse> lst)
        {
            List<OperationCaisse> operationcaisses = new List<OperationCaisse>();
            if (lst == null)
                return operationcaisses;
            foreach (var item in lst)
            {
                if (operationcaisses.Contains(item, new OperationCaisseComparer()))
                {
                    var rslt = (from p in operationcaisses where p.OperationId == item.OperationId select p).Single();
                    var prodindex = operationcaisses.IndexOf(rslt);
                    var prod = operationcaisses[prodindex];
                    var combine = CombineOperationCaisse(prod, item);
                    operationcaisses.RemoveAt(prodindex);
                    operationcaisses.Add(combine);
                }
                else
                {
                    operationcaisses.Add(item);
                }
            }
            return operationcaisses;
        }
        private OperationCaisse CombineOperationCaisse(OperationCaisse one, OperationCaisse two)
        {
            OperationCaisse opcaisse = new OperationCaisse();
            opcaisse = one;
            opcaisse.Montant += two.Montant;
            return opcaisse;
        }

        private List<OperationCaisse> ListOperationCaisse(DateTime dateTime, DateTime dateTime2)
        {
            return DbManager.GetDataByDate_<OperationCaisse>("Date", InfoChecker.AjustDate(dateTime), InfoChecker.AjustDate(dateTime2));
        }

        private ChartDataPie PieChartDateData(OperationCaisse operationCaisse)
        {
            var d = new ChartDataPie();
            var operation = GetOperationById(operationCaisse.OperationId);
            d.Titre = $"{char.ToUpper(operation.Libelle[0])}{operation.Libelle.Substring(1)}";
            d.Valeur = (double)operationCaisse.Montant;
            d.LblDate = true;

            return d;
        }

        /// <summary>
        /// Get an operation by it's id
        /// </summary>
        /// <param name="opid">Operation Id</param>
        /// <returns></returns>
        private EasyManagerLibrary.Operation GetOperationById(int opid)
        {
            return DbManager.GetById<EasyManagerLibrary.Operation>(opid);
        }

        public List<ChartDataPie> TestDatas(int k)
        {
            var Datas = new List<ChartDataPie>();
            for (int i = 0; i < k; i++)
            {
                var d = new ChartDataPie();
                d.Titre = $"Jour{i + 1}";
                d.Valeur = i + 3;
                d.LblDate = true;
                Datas.Add(d);
            }
            return Datas;
        }
    }
}
