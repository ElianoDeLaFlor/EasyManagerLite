using EasyManager.Charts;
using EasyManagerLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyManager.MenuItems
{
    public class CaisseViewModel: INotifyPropertyChanged
    {
        public DatePickerViewModel DatePicker { get; set; }
        public CaisseViewModel()
        {
            DatePicker = new DatePickerViewModel();
        }

        List<ChartDataPie> _chartData { get; set; } = new List<ChartDataPie>();
        public Doughnutchart PieChart { get; set; }

        private string _DateDebut { get; set; }
        private string _DateFin { get; set; }

        private decimal _sortietotal;
        private decimal _entreetotal;
        private decimal _ventetotal;
        private decimal _somme_en_caisse;
        private decimal _recette;

        public decimal SortieTotale
        {
            get =>_sortietotal;
            set
            {
                if (_sortietotal == value)
                    return;
                _sortietotal = value;
                SommeEnCaisse = (VenteTotale + EntreTotale) - value;
                ChartDatas = ChartData();
                OnPropertyChanged();
            }
        }

        public decimal EntreTotale
        {
            get => _entreetotal;
            set
            {
                if (_entreetotal == value)
                    return;
                _entreetotal = value;
                SommeEnCaisse = (VenteTotale + value) - SortieTotale;
                Recette = (VenteTotale + value);
                ChartDatas = ChartData();
                OnPropertyChanged();
            }
        }

        public decimal VenteTotale
        {
            get => _ventetotal;
            set
            {
                if (_ventetotal == value)
                    return;
                _ventetotal = value;
                SommeEnCaisse = (EntreTotale + value) - SortieTotale;
                Recette = (value + EntreTotale);
                ChartDatas = ChartData();
                OnPropertyChanged();
            }
        }

        public decimal SommeEnCaisse
        {
            get => _somme_en_caisse;
            set
            {
                if (_somme_en_caisse == value)
                    return;
                _somme_en_caisse = value;
                
                OnPropertyChanged();
            }
        }

        public decimal Recette
        {
            get => _recette;
            set
            {
                if (_recette == value)
                    return;
                _recette = value;

                OnPropertyChanged();
            }
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
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                OnPropertyChanged();
            }
        }

        private ChartDataPie PieChartDateData(decimal montant,string title)
        {
            var d = new ChartDataPie();
            d.Titre = title;
            d.Valeur = (double)montant;
            d.LblDate = true;

            return d;
        }

        private List<ChartDataPie> ChartData()
        {
            var lstpiedata = new List<ChartDataPie>();
            lstpiedata.Add(PieChartDateData(EntreTotale,Properties.Resources.Entree));
            lstpiedata.Add(PieChartDateData(SortieTotale,Properties.Resources.Sortie));
            lstpiedata.Add(PieChartDateData(VenteTotale,Properties.Resources.Selle));
            return lstpiedata;
        }
    }
}
