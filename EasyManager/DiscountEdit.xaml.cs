using EasyManager.MenuItems;
using EasyManagerDb;
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
using System.Windows.Shapes;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for DiscountEdit.xaml
    /// </summary>
    public partial class DiscountEdit : Window
    {
        #region Variable declaration
        public Home GetHome { get; set; }
        public int TauxDisc { get; set; } = 0;
        private bool IsCat { get; set; }
        private bool IsClient { get; set; }
        private bool IsProd { get; set; }
        public bool IsUpade { get; set; }

        public int DiscountId { get; set; }

        public bool AppliquerCredit { get; set; }

        private int CatId { get; set; }

        private int ClientId { get; set; }

        private string ProdName { get; set; }

        #endregion
        public DiscountEdit()
        {
            InitializeComponent();
        }

        public DiscountEdit(int id)
        {
            InitializeComponent();
            DiscountId = id;
            var datacontext = new DiscountEditViewModel();
            datacontext.DiscountEdit = this;
            datacontext.DatePicker = new DatePickerViewModel();
            DataContext = datacontext;
        }

        private Discount SetDiscountData()
        {
            var Disc = new Discount();
            double taux = tauxslide.Value;
            string debut = DateDebut.Text;
            string fin = DateFin.Text;
            if (IsCat)
            {
                Disc.CategorieId = CatId;
                Disc.ProduitNom = null;
                Disc.ClientId = null;
            }
            else if (IsClient)
            {
                Disc.ClientId = ClientId;
                Disc.ProduitNom = null;
                Disc.CategorieId = null;
            }
            else
            {
                //Prod
                Disc.CategorieId = null;
                Disc.ProduitNom = ProdName;
                Disc.ClientId = null;
            }

            Disc.Taux = (decimal)taux / 100;
            Disc.DateDebut = DateTime.Parse(debut);
            Disc.DateFin = DateTime.Parse(fin);
            Disc.Canceled = false;
            Disc.IsValidForCredit = AppliquerCredit;

            return Disc;

        }

        public void CalendarDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            //Calendar.SelectedDate = ((DatePickerViewModel)DataContext).Date;
        }

        public void CalendarDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            /*if (!Equals(eventArgs.Parameter, "1")) return;

            if (!Calendar.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

            ((DatePickerViewModel)DataContext).Date = Calendar.SelectedDate.Value;*/
        }

        private void PresetTimePicker_SelectedTimeChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<System.DateTime?> e)
        {
            var oldValue = e.OldValue.HasValue ? e.OldValue.Value.ToLongTimeString() : "NULL";
            var newValue = e.NewValue.HasValue ? e.NewValue.Value.ToLongTimeString() : "NULL";

            // Debug.WriteLine($"PresentTimePicker's SelectedTime changed from {oldValue} to {newValue}");
        }

        public void CombinedDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            CombinedCalendar.SelectedDate = ((DiscountEditViewModel)DataContext).DatePicker.Date;
            CombinedClock.Time = ((DiscountEditViewModel)DataContext).DatePicker.Time;
        }

        public void CombinedFDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            CombinedCalendarF.SelectedDate = ((DiscountEditViewModel)DataContext).DatePicker.DateF;
            CombinedClockF.Time = ((DiscountEditViewModel)DataContext).DatePicker.TimeF;
        }

        public void CombinedDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, "1"))
            {
                var combined = CombinedCalendar.SelectedDate.Value.AddSeconds(CombinedClock.Time.TimeOfDay.TotalSeconds);
                ((DiscountEditViewModel)DataContext).DatePicker.Time = combined;
                ((DiscountEditViewModel)DataContext).DatePicker.Date = combined;
            }
        }

        public void CombinedFDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, "1"))
            {
                var combined = CombinedCalendarF.SelectedDate.Value.AddSeconds(CombinedClockF.Time.TimeOfDay.TotalSeconds);
                ((DiscountEditViewModel)DataContext).DatePicker.TimeF = combined;
                ((DiscountEditViewModel)DataContext).DatePicker.DateF = combined;
            }
        }

        private void CbProdLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            int selectedindex = cb.SelectedIndex;
            if (selectedindex > 0)
            {
                ProdName = cb.SelectedItem as string;
                CbCatLst.IsEnabled = false;
                CbClientLst.IsEnabled = false;
                IsCat = false;
                IsProd = true;
                IsClient = false;
            }
            else
                ActiveComboBox();

        }

        private void Reset()
        {
            tauxslide.Value = 0;
            var picher = ((DiscountEditViewModel)DataContext).DatePicker;
            picher.Date = DateTime.Now;
            picher.Time = DateTime.Now;
            picher.DateF = DateTime.Now;
            picher.TimeF = DateTime.Now;
            CatId = 0;
            ClientId = 0;
            ProdName = null;
            IsCat = false;
            IsClient = false;
            IsProd = false;
            CbCatLst.SelectedIndex = 0;
            CbClientLst.SelectedIndex = 0;
            CbProdLst.SelectedIndex = 0;
            ActiveComboBox();
        }

        private void ActiveComboBox()
        {
            CbCatLst.IsEnabled = true;
            CbClientLst.IsEnabled = true;
            CbProdLst.IsEnabled = true;
            IsCat = false;
            IsClient = false;
            IsProd = false;
        }

        private void CbCatLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            int selectedindex = cb.SelectedIndex;
            if (selectedindex > 0)
            {
                var item = cb.SelectedItem.ToString();
                var Cat = DbManager.GetCategorieByLibelle(item);
                CatId = Cat.Id;
                CbProdLst.IsEnabled = false;
                CbClientLst.IsEnabled = false;
                IsProd = false;
                IsClient = false;
                IsCat = true;
            }
            else
                ActiveComboBox();
        }

        private void CbClientLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            int selectedindex = cb.SelectedIndex;
            if (selectedindex > 0)
            {
                var item = cb.SelectedItem.ToString();
                var client = DbManager.GetClientByName(item);
                ClientId = client.Id;
                CbProdLst.IsEnabled = false;
                CbCatLst.IsEnabled = false;
                IsProd = false;
                IsClient = true;
                IsCat = false;
            }
            else
                ActiveComboBox();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Reset();
            if (IsUpade)
            {
                GetHome.MenuItemsListBox.SelectedIndex = 1;

            }


        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if (tauxslide.Value > 0)
            {
                if (CatId == 0 && ClientId == 0 && ProdName == null)
                {
                    MessageBox.Show(Properties.Resources.TauxCritere, Properties.Resources.MainTitle,
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    //check interval between start and end date
                    if (InfoChecker.DateDiff(DateTime.Parse(DateDebut.Text),
                            DateTime.Parse(DateFin.Text)) >= 0.5)
                    {
                        //update
                        Discount discount = new Discount();
                        discount = SetDiscountData();
                        discount.Id = DiscountId;
                        if (DbManager.UpDate(discount, DiscountId))
                        {
                            MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle,
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            //Reset();
                        }
                        else
                        {
                            MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle,
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.TauxIntervalError, Properties.Resources.MainTitle,
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.TauxError, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void FilledDropDown()
        {
            CbCatLst.ItemsSource = CatList();
        }

        private void FilledProdNameDD()
        {
            CbProdLst.ItemsSource = ProdList();
        }

        private void FilledClientNameDD()
        {
            CbClientLst.ItemsSource = ClientList();
        }

        private List<string> CatList()
        {
            List<string> categorielist = new List<string>();
            var lst = DbManager.GetAll<EasyManagerLibrary.Categorie>();

            categorielist.Add(Properties.Resources.MakeSelection);
            foreach (var item in lst)
            {
                categorielist.Add(item.Libelle);
            }
            return categorielist;
        }

        private List<string> ProdList()
        {
            var lst = DbManager.GetAll<EasyManagerLibrary.Produit>();
            var lstname = new List<string>();
            lstname.Add(Properties.Resources.MakeSelection);
            foreach (var item in lst)
            {
                lstname.Add(item.Nom);
            }
            return lstname;
        }

        private List<string> ClientList()
        {
            var lst = DbManager.GetAll<EasyManagerLibrary.Client>();
            var lstname = new List<string>();
            lstname.Add(Properties.Resources.MakeSelection);
            foreach (var item in lst)
            {
                lstname.Add(item.Nom);
            }
            return lstname;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            SetDropDown();
            var discount = DbManager.GetById<Discount>(DiscountId);
            var client = discount.ClientId == 0 ? null : DbManager.GetById<EasyManagerLibrary.Client>(discount.ClientId.Value);
            var cat = discount.CategorieId == 0 ? null : DbManager.GetById<EasyManagerLibrary.Categorie>(discount.CategorieId.Value);
            var product = discount.ProduitNom == null ? null : DbManager.GetProduitByName(discount.ProduitNom);
            if (client != null)
                CbClientLst.SelectedItem = client.Nom;
            else if (cat != null)
                CbCatLst.SelectedItem = cat.Libelle;
            else
                CbProdLst.SelectedItem = product.Nom;

            FillDiscount(discount);
        }

        private void FillDiscount(Discount disc)
        {
            tauxslide.Value = (double)disc.Taux * 100;
            var datacontext = (DiscountEditViewModel)DataContext;
            datacontext.DatePicker.Date = disc.DateDebut;
            datacontext.DatePicker.Time = disc.DateDebut;
            datacontext.DatePicker.DateF = disc.DateFin;
            datacontext.DatePicker.TimeF = disc.DateFin;
            Credit.IsChecked = disc.IsValidForCredit;

        }

        private void SetDropDown()
        {
            FilledDropDown();
            FilledProdNameDD();
            FilledClientNameDD();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            scroll.MaxHeight = Math.Abs(ActualHeight - 100);
        }
    }

    public class DiscountEditViewModel
    {
        public DatePickerViewModel DatePicker { get; set; }
        public DiscountEdit DiscountEdit { get; set; }
    }
}
