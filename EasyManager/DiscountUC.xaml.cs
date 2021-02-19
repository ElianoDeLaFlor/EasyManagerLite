using EasyManager.MenuItems;
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
using EasyManagerDb;
using EasyManagerLibrary;
using EasyManager.UserControls;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for DiscountUC.xaml
    /// </summary>
    public partial class DiscountUC : UserControl
    {
        #region Variable declaration
        public Home GetHome { get; set; }
        public int TauxDisc { get; set; } = 0;
        private bool IsCat { get; set; }
        private bool IsClient { get; set; }
        private bool IsProd { get; set; }
        public bool IsUpade { get; set; }
        List<ToogleCategorie> toogleCategories = new List<ToogleCategorie>();

        public int DiscountId { get; set; }

        public bool AppliquerCredit { get; set; }

        private int CatId { get; set; }

        private int ClientId { get; set; }

        private string ProdName { get; set; }

        public Discount Discount { get; set; }
        private List<int> _checkedList = new List<int>();
        public List<Module> Modules { get; set; } = new List<Module>();
        public List<int> AlreadySelectedCat { get; set; } = new List<int>();
        #endregion

        public List<int> CheckedList
        {
            get { return _checkedList; }
            set { _checkedList = value; }
        }


        public DiscountUC()
        {
            InitializeComponent();
        }

        public DiscountUC(Home h,int id)
        {
                InitializeComponent();
                GetHome = h;
                DiscountId = id;
                IsUpade = true;
                var datacontext = new DiscountViewModel();
                datacontext.DiscountUC = this;
                datacontext.DatePicker = new DatePickerViewModel();
                DataContext = datacontext;
        }

        public DiscountUC(Home h)
        {
            InitializeComponent();
            GetHome = h;
            IsUpade = false;
            AppliquerCredit = false;
            var datacontext= new DiscountViewModel();
            datacontext.DiscountUC = this;
            datacontext.DatePicker = new DatePickerViewModel();
            DataContext = datacontext;
        }

        private Discount SetDiscountData()
        {
            var Disc=new EasyManagerLibrary.Discount();
            double taux = tauxslide.Value;
            string debut = DateDebut.Text;
            string fin = DateFin.Text;
            if (IsCat)
            {
                Disc.CategorieId = CatId;
                Disc.ProduitNom = null;
                Disc.ClientId = 0;
            }
            else if(IsClient)
            {
                Disc.ClientId = ClientId;
                Disc.ProduitNom = null;
                Disc.CategorieId = 0;
            }
            else
            {
                //Prod
                Disc.CategorieId = 0;
                Disc.ProduitNom = ProdName;
                Disc.ClientId = 0;
            }

            Disc.Taux = (decimal)taux / 100;
            Disc.DateDebut=DateTime.Parse(debut);
            Disc.DateFin=DateTime.Parse(fin);
            Disc.Canceled = false;
            Disc.IsValidForCredit = AppliquerCredit;
            Disc.UserId = GetHome.DataContextt.ConnectedUser.Id;

            return Disc;

        }

        private List<Discount> SetDiscountData(List<int> catids)
        {
            List<Discount> discounts = new List<Discount>();
            foreach (var item in catids)
            {
                var Disc = new EasyManagerLibrary.Discount();
                double taux = tauxslide.Value;
                string debut = DateDebut.Text;
                string fin = DateFin.Text;
                Disc.CategorieId = item;
                Disc.ProduitNom = null;
                Disc.ClientId = 0;
                Disc.Taux = (decimal)taux / 100;
                Disc.DateDebut = DateTime.Parse(debut);
                Disc.DateFin = DateTime.Parse(fin);
                Disc.Canceled = false;
                Disc.IsValidForCredit = AppliquerCredit;
                Disc.UserId = GetHome.DataContextt.ConnectedUser.Id;

                discounts.Add(Disc);
            }
            return discounts;
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
            CombinedCalendar.SelectedDate = ((DiscountViewModel)DataContext).DatePicker.Date;
            CombinedClock.Time = ((DiscountViewModel)DataContext).DatePicker.Time;
        }

        public void CombinedFDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            CombinedCalendarF.SelectedDate = ((DiscountViewModel)DataContext).DatePicker.DateF;
            CombinedClockF.Time = ((DiscountViewModel)DataContext).DatePicker.TimeF;
        }

        public void CombinedDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, "1"))
            {
                var combined = CombinedCalendar.SelectedDate.Value.AddSeconds(CombinedClock.Time.TimeOfDay.TotalSeconds);
                ((DiscountViewModel)DataContext).DatePicker.Time = combined;
                ((DiscountViewModel)DataContext).DatePicker.Date = combined;
            }
        }

        public void CombinedFDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, "1"))
            {
                var combined = CombinedCalendarF.SelectedDate.Value.AddSeconds(CombinedClockF.Time.TimeOfDay.TotalSeconds);
                ((DiscountViewModel)DataContext).DatePicker.TimeF = combined;
                ((DiscountViewModel)DataContext).DatePicker.DateF = combined;
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
                btndialog.IsEnabled = false;
                IsCat = false;
                IsProd = true;
                IsClient = false;
            }
            else
                ActiveComboBox();

        }

        private void Reset(int i)
        {
            tauxslide.Value = 0;
            var picher = ((DiscountViewModel) DataContext).DatePicker;
            picher.Date=DateTime.Now;
            picher.Time=DateTime.Now;
            picher.DateF=DateTime.Now;
            picher.TimeF=DateTime.Now;
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

        private void Reset()
        {
            tauxslide.Value = 0;
            var picher = ((DiscountViewModel)DataContext).DatePicker;
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
            btndialog.IsEnabled = true;
            AlreadySelectedCat.Clear();
            CheckedList.Clear();
            StackOne.Children.Clear();
            StackTwo.Children.Clear();
            SetCheckBox(CategorieList(), StackOne, StackTwo);
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
                btndialog.IsEnabled = false;
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
                if (CheckedList.Count() == 0 && ClientId == 0 && ProdName == null)
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

                        if (IsUpade)
                        {
                            //update
                            var disc = SetDiscountData();
                            disc.Id = DiscountId;
                            if(DbManager.UpDate(disc,DiscountId))
                            {
                                MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle,
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                                Reset();
                            }
                            else
                            {
                                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle,
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            //Save
                            if (IsCat)
                            {
                                var lst = SetDiscountData(CheckedList);
                                List<bool> rslt = new List<bool>();
                                foreach(var item in lst)
                                {
                                    rslt.Add(DbManager.Save(item));
                                }
                                if (!rslt.Contains(false))
                                {
                                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle,
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                                    Reset();
                                }
                                else
                                {
                                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle,
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                if (DbManager.Save(SetDiscountData()))
                                {
                                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle,
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                                    Reset();
                                }
                                else
                                {
                                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle,
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            
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
            var lstname=new List<string>();
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
            var lstname=new List<string>();
            lstname.Add(Properties.Resources.MakeSelection);
            foreach (var item in lst)
            {
                lstname.Add(item.Nom);
            }
            return lstname;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.Discount;
                GetHome.MainTitle.Text = Properties.Resources.Discount;
            }
            
            SetDropDown();
            SetCheckBox(CategorieList(), StackOne, StackTwo);
            if (IsUpade)
            {
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
        }

        private void FillDiscount(EasyManagerLibrary.Discount disc)
        {
            tauxslide.Value = (double)disc.Taux * 100;
            var datacontext = (DiscountViewModel) DataContext;
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

        private void Sample1_DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            

            if (Equals(eventArgs.Parameter, true))
            {
                //validate
                if (CheckedList.Count() > 0)
                {
                    CbProdLst.IsEnabled = false;
                    CbClientLst.IsEnabled = false;
                    btndialog.IsEnabled = true;
                }
            }
            else
            {
                //Canceled
                CbProdLst.IsEnabled = true;
                CbClientLst.IsEnabled = true;
                btndialog.IsEnabled = true;
                Reset();
            }


        }

        private void SetImpairCheckBox(int cnt, List<EasyManagerLibrary.Categorie> lst_cat, int splitpoint, Panel pnl, Panel pnl2)
        {
            cnt = (lst_cat.Count - splitpoint) / 2;
            for (int i = 0; i < cnt + splitpoint; i++)
            {
                ToogleCategorie tc = new ToogleCategorie
                {
                    Categorie = lst_cat[i]
                };
                CheckTheCkeckBoxes(tc, lst_cat, i);
                tc.SetCheckedList(ref _checkedList);
                pnl.Children.Add(tc);
            }

            for (int i = cnt + splitpoint; i < lst_cat.Count; i++)
            {
                ToogleCategorie tc = new ToogleCategorie
                {
                    Categorie = lst_cat[i]
                };
                CheckTheCkeckBoxes(tc, lst_cat, i);
                tc.SetCheckedList(ref _checkedList);
                pnl2.Children.Add(tc);
            }
        }

        private void RestCheckBox()
        {

        }
        /// <param name="pnl2">Colonne deux</param>
        private void SetPairCheckBox(int cnt, List<EasyManagerLibrary.Categorie> lst_rm, int splitpoint, Panel pnl, Panel pnl2)
        {
            cnt = (lst_rm.Count - splitpoint) / 2;
            for (int i = 0; i < cnt + splitpoint; i++)
            {
                ToogleCategorie tc = new ToogleCategorie
                {
                    Categorie = lst_rm[i]
                };
                CheckTheCkeckBoxes(tc, lst_rm, i);
                tc.SetCheckedList(ref _checkedList);
                pnl.Children.Add(tc);
            }

            for (int i = cnt + splitpoint; i < lst_rm.Count; i++)
            {
                ToogleCategorie tc = new ToogleCategorie
                {
                    Categorie = lst_rm[i]
                };
                CheckTheCkeckBoxes(tc, lst_rm, i);
                tc.SetCheckedList(ref _checkedList);
                pnl2.Children.Add(tc);
            }
        }

        private void CheckTheCkeckBoxes(ToogleCategorie tc, List<EasyManagerLibrary.Categorie> lst_rm, int i)
        {
            if (AlreadySelectedCat.Count > 0)
            {
                tc.State = AlreadySelectedCat.Contains(lst_rm[i].Id);
            }
            else
                tc.State = false;
        }

        private List<EasyManagerLibrary.Categorie> CategorieList()
        {
            var rslt = DbManager.GetAll<EasyManagerLibrary.Categorie>();
            return rslt;
        }
        private void SetCheckBox(List<EasyManagerLibrary.Categorie> lst_cat, StackPanel pnl, StackPanel pnl2)
        {
            int cnt = 0;
            int splitpoint = lst_cat.Count % 2;
            if (splitpoint > 0)
            {
                SetImpairCheckBox(cnt, lst_cat, splitpoint, pnl, pnl);
            }
            else
            {
                SetPairCheckBox(cnt, lst_cat, splitpoint, pnl, pnl2);
            }
        }
        private void DialogHost_OnDialogOpend(object sender, DialogOpenedEventArgs eventArgs)
        {
            IsCat = true;
            IsProd = false;
            IsClient = false;
        }
    
    }

    public class DiscountViewModel
    {
        public DatePickerViewModel DatePicker { get; set; }
        public DiscountUC DiscountUC { get; set; }
    }


}
