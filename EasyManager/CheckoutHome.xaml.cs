using EasyManager.MenuItems;
using EasyManagerLibrary;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for CheckoutHome.xaml
    /// </summary>
    public partial class CheckoutHome : UserControl
    {
        public CaisseHomeViewModel ViewModel { get; set; } = new CaisseHomeViewModel();
        public GestionDeCaisse GetCaisseHome { get; set; }

        public CheckoutHome()
        {
            InitializeComponent();
            ViewModel.PieChart = piechart;
            ViewModel.ColumnChart = Columnchart;
            int day = (DateTime.Now.Day) - 1;
            int LeftDaysToTheEnd;
            if (DateTime.UtcNow.Month == 2)
            {
                //febrary
                //total day of the month => 28
                LeftDaysToTheEnd = 28 - (DateTime.Now.Day);
            }
            else
            {
                //total day of the month=>30
                LeftDaysToTheEnd = 30 - (DateTime.Now.Day);
            }

            ViewModel.DateDebut = InfoChecker.AjustDate(InfoChecker.DateArriere(day));
            ViewModel.DateFin = InfoChecker.AjustDate(InfoChecker.NextDate(DateTime.UtcNow, LeftDaysToTheEnd));


            DataContext = ViewModel;
        }

        public CheckoutHome(GestionDeCaisse CaisseHome)
        {
            InitializeComponent();
            ViewModel.PieChart = piechart;
            ViewModel.ColumnChart = Columnchart;
            int day = (DateTime.Now.Day) - 1;
            int LeftDaysToTheEnd;
            if (DateTime.UtcNow.Month == 2)
            {
                //febrary
                //total day of the month => 28
                LeftDaysToTheEnd = 28 - (DateTime.Now.Day);
            }
            else
            {
                //total day of the month=>30
                LeftDaysToTheEnd = 30 - (DateTime.Now.Day);
            }

            ViewModel.DateDebut = InfoChecker.AjustDate(InfoChecker.DateArriere(day));
            ViewModel.DateFin = InfoChecker.AjustDate(InfoChecker.NextDate(DateTime.UtcNow, LeftDaysToTheEnd));
            GetCaisseHome = CaisseHome;

            DataContext = ViewModel;
        }

        public void CalendarDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            Calendar.SelectedDate = ViewModel.DatePicker.Date;
        }

        public void CalendarDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!Calendar.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

            ViewModel.DatePicker.Date = Calendar.SelectedDate.Value;

            //update list on date change
            ViewModel.DateDebut = InfoChecker.AjustDate(ViewModel.DatePicker.Date);
            
        }

        public void CalendarDialogOpenedEventHandlerF(object sender, DialogOpenedEventArgs eventArgs)
        {
            CalendarF.SelectedDate = ViewModel.DatePicker.DateF;
        }

        public void CalendarDialogClosingEventHandlerF(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!CalendarF.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

            ViewModel.DatePicker.DateF = CalendarF.SelectedDate.Value;

            //update list on date change
            ViewModel.DateFin = InfoChecker.AjustDate(ViewModel.DatePicker.DateF);
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (GetCaisseHome != null)
            {
                GetCaisseHome.Title = Properties.Resources.MenuHome;
                GetCaisseHome.MainTitle.Text = Properties.Resources.MenuHome;
            }
        }
    }
}
